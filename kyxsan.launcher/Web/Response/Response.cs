//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Web.Bridge.Model;
using System.Runtime.CompilerServices;

namespace kyxsan.Web.Response;

internal class Response : ICommonResponse<Response>
{
    public const int InternalFailure = 0x26F19335;

    [JsonConstructor]
    public Response(int returnCode, string message)
    {
        ReturnCode = returnCode;
        Message = message;
    }

    [JsonPropertyName("retcode")]
    public int ReturnCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    public static implicit operator ValueResult<bool, string>(Response response)
    {
        return new(response.ReturnCode == 0, response.Message);
    }

    static Response ICommonResponse<Response>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message);
    }

    public static TResponse DefaultIfNull<TResponse>(TResponse? response, [CallerMemberName] string callerName = default!)
        where TResponse : ICommonResponse<TResponse>
    {
        DefaultIfNull(ref response, callerName);
        return response;
    }

    public static void DefaultIfNull<TResponse>([NotNull] ref TResponse? response, [CallerMemberName] string callerName = default!)
        where TResponse : ICommonResponse<TResponse>
    {
        string message = SH.FormatWebResponseRequestException(callerName, TypeNameHelper.GetTypeDisplayName(typeof(TResponse)));
        response ??= TResponse.CreateDefault(InternalFailure, message);

        switch ((KnownReturnCode)response.ReturnCode)
        {
            case KnownReturnCode.PleaseLogin:
            case KnownReturnCode.RET_TOKEN_INVALID:
                response.Message = SH.FormatWebResponseRefreshCookieHint(response.Message);
                break;
            case KnownReturnCode.InvalidAccountFormat:
                response.Message = SH.WebResponseAccountNotLoggedIn;
                break;
            case KnownReturnCode.SignInError:
                response.Message = SH.FormatWebResponseSignInErrorHint(response.Message);
                break;
            case KnownReturnCode.CODE5003:
                response.Message = SH.WebResponseAccountRisk;
                break;
        }
    }

    public static Response<TData> CloneReturnCodeAndMessage<TData, TOther>(Response<TOther> response)
    {
        return new(response.ReturnCode, response.Message, default);
    }

    public override string ToString()
    {
        return SH.FormatWebResponse(ReturnCode, Message);
    }
}

[SuppressMessage("", "SA1402")]
internal class Response<TData> : Response, ICommonResponse<Response<TData>>, IJsBridgeResult
{
    [JsonConstructor]
    public Response(int returnCode, string message, TData? data)
        : base(returnCode, message)
    {
        Data = data;
    }

    [JsonPropertyName("data")]
    public TData? Data { get; private init; }

    static Response<TData> ICommonResponse<Response<TData>>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message, default);
    }
}