using System.Diagnostics;

namespace kyxsan.Updater;

internal static class Program
{
    static int Main(string[] args)
    {
        int pid = 0;
        string source = "";
        string target = "";
        string exe = "";

        for (int i = 0; i < args.Length - 1; i++)
        {
            switch (args[i])
            {
                case "--pid": pid = int.Parse(args[++i]); break;
                case "--source": source = args[++i]; break;
                case "--target": target = args[++i]; break;
                case "--exe": exe = args[++i]; break;
            }
        }

        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target) || string.IsNullOrEmpty(exe))
        {
            // Invalid arguments
            return 1;
        }

        // Wait for main process to exit
        if (pid > 0)
        {
            try
            {
                using Process? proc = Process.GetProcessById(pid);
                if (proc is not null && !proc.HasExited)
                {

                    proc.WaitForExit(15000);
                }
            }
            catch (ArgumentException)
            {
                // Process already exited
            }
        }

        // Test write permission
        try
        {
            string testFile = Path.Combine(target, ".update_test");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
        }
        catch (UnauthorizedAccessException)
        {
            // Need elevation - relaunch self as admin

            try
            {
                string self = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName ?? "updater.exe";
                Process.Start(new ProcessStartInfo
                {
                    FileName = self,
                    Arguments = string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)),
                    UseShellExecute = true,
                    Verb = "runas",
                });
                return 0;
            }
            catch
            {

                return 2;
            }
        }

        // Copy files from source to target
        bool copySuccess = true;
        try
        {
            CopyDirectory(source, target);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to copy files: {ex.Message}");
            copySuccess = false;
        }

        if (!copySuccess)
        {
            // Do not launch the app in a potentially corrupted state
            return 3;
        }

        // Clean up source cache
        try
        {
            Directory.Delete(source, true);
        }
        catch
        {
            // Non-critical
        }

        // Restart main application
        string exePath = Path.Combine(target, exe);
        if (File.Exists(exePath))
        {

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = target,
                UseShellExecute = true,
            });
        }

        return 0;
    }

    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        foreach (string file in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceDir, file);
            string destPath = Path.Combine(targetDir, relativePath);

            string? destDir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // Retry up to 3 times for locked files
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    File.Copy(file, destPath, overwrite: true);
                    break;
                }
                catch (IOException) when (attempt < 2)
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
