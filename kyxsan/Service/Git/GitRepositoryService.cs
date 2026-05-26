//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using LibGit2Sharp;
using kyxsan.Core;
using kyxsan.Core.IO;
using kyxsan.Core.IO.Http.Proxy;
using kyxsan.Core.Setting;
using kyxsan.Service.BackgroundActivity;
using kyxsan.Web.kyxsan;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Response;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;

namespace kyxsan.Service.Git;

[Service(ServiceLifetime.Singleton, typeof(IGitRepositoryService))]
internal sealed partial class GitRepositoryService : IGitRepositoryService
{
    private readonly AsyncKeyedLock<string> repoLock = new();
    private readonly BackgroundActivityOptions backgroundActivityOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial GitRepositoryService(IServiceProvider serviceProvider);

    static GitRepositoryService()
    {
        GlobalSettings.SetConfigSearchPaths(ConfigurationLevel.ProgramData, string.Empty);
        GlobalSettings.SetConfigSearchPaths(ConfigurationLevel.Global, string.Empty);
        GlobalSettings.SetConfigSearchPaths(ConfigurationLevel.System, string.Empty);
        GlobalSettings.SetConfigSearchPaths(ConfigurationLevel.Xdg, string.Empty);
        GlobalSettings.SetOwnerValidation(false);
    }

    public async ValueTask<ValueResult<bool, ValueDirectory>> EnsureRepositoryAsync(string name)
    {
        if (LocalSetting.Get("kyxsan::Git::Repository::Override", false))
        {
            return new(true, Path.GetFullPath(Path.Combine(kyxsanRuntime.GetDataRepositoryDirectory(), name)));
        }

        using (await repoLock.LockAsync(name).ConfigureAwait(false))
        {
            string directory = Path.GetFullPath(Path.Combine(kyxsanRuntime.GetDataRepositoryDirectory(), name));
            bool hasValidLocalRepo = HasUsableRepository(directory);

            if (hasValidLocalRepo)
            {
                return new(true, directory);
            }

            ImmutableArray<GitRepository> infos;
            try
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    kyxsanInfrastructureClient infrastructureClient = scope.ServiceProvider.GetRequiredService<kyxsanInfrastructureClient>();
                    kyxsanResponse<ImmutableArray<GitRepository>> response = await infrastructureClient.GetGitRepositoryAsync(name).ConfigureAwait(false);
                    if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out infos))
                    {
                        if (hasValidLocalRepo)
                        {
                            return new(true, directory);
                        }

                        infos = GetFallbackRepositories(name);
                        if (infos.IsDefaultOrEmpty)
                        {
                            return new(false, default);
                        }
                    }
                }
            }
            catch (Exception)
            {
                infos = GetFallbackRepositories(name);
                if (infos.IsDefaultOrEmpty)
                {
                    return new(false, default);
                }
            }

            BackgroundActivity.BackgroundActivity activity = GetActivityByName(name);

            bool failed = false;
            List<Exception> exceptions = [];
            try
            {
                await activity.NotifyAsync(taskContext).ConfigureAwait(false);
                await activity.UpdateAsync(taskContext, SH.ServiceBackgroundActivityDefaultDescription, false, false, false, false).ConfigureAwait(false);

                foreach (GitRepository info in RepositoryAffinity.Sort(infos))
                {
                    try
                    {
                        try
                        {
                            return EnsureRepository(activity, directory, info, false);
                        }
                        catch (Exception first)
                        {
                            exceptions.Add(first);
                            if (!hasValidLocalRepo)
                            {
                                return EnsureRepository(activity, directory, info, true);
                            }
                        }
                    }
                    catch (Exception second)
                    {
                        exceptions.Add(second);
                    }
                }
            }
            catch (Exception)
            {
                failed = true;
                throw;
            }
            finally
            {
                if (!failed)
                {
                    await activity.NotifyAsync(taskContext).ConfigureAwait(false);
                    await activity.UpdateAsync(taskContext, SH.ServiceGitRepositoryOperationCompleted, true, false, false, false).ConfigureAwait(false);
                }
            }

            if (hasValidLocalRepo)
            {
                await activity.NotifyAsync(taskContext).ConfigureAwait(false);
                await activity.UpdateAsync(taskContext, SH.ServiceGitRepositoryOperationCompleted, true, false, false, false).ConfigureAwait(false);
                return new(true, directory);
            }

            if (await TryDownloadAndExtractZipAsync(name, directory, activity).ConfigureAwait(false))
            {
                await activity.NotifyAsync(taskContext).ConfigureAwait(false);
                await activity.UpdateAsync(taskContext, SH.ServiceGitRepositoryOperationCompleted, true, false, false, false).ConfigureAwait(false);
                return new(true, directory);
            }

            await activity.NotifyAsync(taskContext).ConfigureAwait(false);
            await activity.UpdateAsync(taskContext, SH.ServiceGitRepositoryOperationFailed, false, true, false, false).ConfigureAwait(false);
            throw new GitRepositoryException(SH.ServiceGitRepositoryOperationFailed, exceptions);
        }
    }

    private ValueResult<bool, ValueDirectory> EnsureRepository(BackgroundActivity.BackgroundActivity activity, string directory, GitRepository info, bool forceInvalid)
    {
        // Increase & decrease count in the same method, so that crash in the middle can correctly count as failure.
        RepositoryAffinity.IncreaseFailure(info);
        FetchOptions fetchOptions = new()
        {
            Depth = 1,
            Prune = true,
            TagFetchMode = TagFetchMode.None,
            ProxyOptions =
            {
                ProxyType = ProxyType.Auto,
                Url = HttpProxyUsingSystemProxy.Instance.CurrentProxyUri,
            },
            CredentialsProvider = (url, user, types) => string.IsNullOrEmpty(info.Token)
                ? default
                : new UsernamePasswordCredentials
                {
                    Username = info.Username,
                    Password = info.Token,
                },
            OnProgress = output =>
            {
                int idx = output.AsSpan().IndexOfAny("\r\n");
                activity.Update(taskContext, idx > 0 ? output.Substring(0, idx) : output, false, false, false, false);
                return true;
            },
            OnTransferProgress = progress =>
            {
                double progressValue = progress.TotalObjects == 0 ? 0 : (double)progress.ReceivedObjects / progress.TotalObjects;
                activity.Update(taskContext, $"{progress.ReceivedObjects}/{progress.TotalObjects}, {Converters.ToFileSizeString(progress.ReceivedBytes)}", false, false, true, false, progressValue);
                return true;
            },
            CertificateCheck = static (cert, valid, host) => true,
        };

        if (forceInvalid || !Repository.IsValid(directory))
        {
            if (!forceInvalid && Directory.Exists(Path.Combine(directory, "Genshin")))
            {
                RepositoryAffinity.DecreaseFailure(info);
                return new(true, directory);
            }

            if (Directory.Exists(directory))
            {
                Directory.SetReadOnly(directory, false);
                Directory.Delete(directory, true);
            }

            Repository.AdvancedClone(info.HttpsUrl.OriginalString, directory, new(fetchOptions)
            {
                Checkout = true,
            });
        }
        else
        {
            // We need to ensure local repo is up to date
            using (Repository repo = new(directory))
            {
                Configuration config = repo.Config;
                config.Set("core.longpaths", true);
                config.Set("safe.directory", true);
                if (string.IsNullOrEmpty(fetchOptions.ProxyOptions.Url))
                {
                    config.Unset("http.proxy");
                    config.Unset("https.proxy");
                }
                else
                {
                    config.Set("http.proxy", fetchOptions.ProxyOptions.Url);
                    config.Set("https.proxy", fetchOptions.ProxyOptions.Url);
                }

                repo.Network.Remotes.Update("origin", remote => remote.Url = info.HttpsUrl.OriginalString);
                repo.RemoveUntrackedFiles();
                fetchOptions.UpdateFetchHead = false;
                Commands.Fetch(repo, repo.Head.RemoteName, Array.Empty<string>(), fetchOptions, default);

                // Manually patch .git/shallow file
                File.WriteAllText(Path.Combine(directory, ".git//shallow"), string.Join("", repo.Branches.Where(static branch => branch.IsRemote).Select(static branch => $"{branch.Tip.Sha}\n")));

                Branch remoteBranch = repo.Branches["origin/main"];
                Branch localBranch = repo.Branches["main"] ?? repo.CreateBranch("main", remoteBranch.Tip);
                repo.Branches.Update(localBranch, b => b.TrackedBranch = remoteBranch.CanonicalName);
                repo.Reset(ResetMode.Hard, remoteBranch.Tip);
                repo.RemoveUntrackedFiles();
            }
        }

        RepositoryAffinity.DecreaseFailure(info);
        return new(true, directory);
    }

    private static bool HasUsableRepository(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return false;
        }

        if (Repository.IsValid(directory))
        {
            try
            {
                using Repository repo = new(directory);
                if (repo.Head?.Tip is not null)
                {
                    return true;
                }
            }
            catch
            {
            }
        }

        return Directory.Exists(Path.Combine(directory, "Genshin"));
    }

    private async ValueTask<bool> TryDownloadAndExtractZipAsync(string name, string directory, BackgroundActivity.BackgroundActivity activity)
    {
        ImmutableArray<string> zipUrls = GetFallbackZipUrls(name);
        if (zipUrls.IsDefaultOrEmpty)
        {
            return false;
        }

        string? proxyUrl = HttpProxyUsingSystemProxy.Instance.CurrentProxyUri;
        HttpClientHandler handler = new();
        if (!string.IsNullOrEmpty(proxyUrl))
        {
            handler.Proxy = new WebProxy(proxyUrl);
            handler.UseProxy = true;
        }

        using HttpClient httpClient = new(handler);
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        foreach (string zipUrl in zipUrls)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    Directory.SetReadOnly(directory, false);
                    Directory.Delete(directory, true);
                }

                string tempZip = Path.GetTempFileName();
                try
                {
                    using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(zipUrl), HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        long? totalBytes = response.Content.Headers.ContentLength;
                        using Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        using FileStream fileStream = File.Create(tempZip);

                        byte[] buffer = new byte[81920];
                        long totalRead = 0;
                        int bytesRead;
                        while ((bytesRead = await responseStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
                        {
                            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                            totalRead += bytesRead;
                            if (totalBytes > 0)
                            {
                                double progress = (double)totalRead / totalBytes.Value;
                                activity.Update(taskContext, $"{Converters.ToFileSizeString(totalRead)}/{Converters.ToFileSizeString(totalBytes.Value)}", false, false, true, false, progress);
                            }
                        }
                    }

                    string tempExtract = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    try
                    {
                        ZipFile.ExtractToDirectory(tempZip, tempExtract);
                        string[] extractedDirs = Directory.GetDirectories(tempExtract);
                        string sourceDir = extractedDirs.Length == 1 ? extractedDirs[0] : tempExtract;
                        Directory.Move(sourceDir, directory);
                    }
                    finally
                    {
                        if (Directory.Exists(tempExtract))
                        {
                            Directory.Delete(tempExtract, true);
                        }
                    }
                }
                finally
                {
                    if (File.Exists(tempZip))
                    {
                        File.Delete(tempZip);
                    }
                }

                return true;
            }
            catch
            {
                continue;
            }
        }

        return false;
    }

    private static ImmutableArray<GitRepository> GetFallbackRepositories(string name)
    {
        return name switch
        {
            "Snap.Metadata" =>
            [
                new() { Name = "ky3-git", HttpsUrl = new("https://github.com/ky3-git/Snap.Metadata.git"), WebUrl = new("https://github.com/ky3-git/Snap.Metadata"), Type = GitRepositoryType.Public },
                new() { Name = "github", HttpsUrl = new("https://github.com/wangdage12/Snap.Metadata.git"), WebUrl = new("https://github.com/wangdage12/Snap.Metadata"), Type = GitRepositoryType.Public },
            ],
            _ => [],
        };
    }

    private static ImmutableArray<string> GetFallbackZipUrls(string name)
    {
        return name switch
        {
            "Snap.Metadata" =>
            [
                "https://github.com/ky3-git/Snap.Metadata/archive/refs/heads/main.zip",
                "https://github.com/wangdage12/Snap.Metadata/archive/refs/heads/main.zip",
            ],
            _ => [],
        };
    }

    private BackgroundActivity.BackgroundActivity GetActivityByName(string name)
    {
        return name switch
        {
            "Snap.Metadata" => backgroundActivityOptions.MetadataInitialization,
            "Snap.ContentDelivery" => backgroundActivityOptions.FullTrustInitialization,
            _ => backgroundActivityOptions.Default,
        };
    }
}