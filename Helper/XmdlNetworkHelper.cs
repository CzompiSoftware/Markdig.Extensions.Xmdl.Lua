using System;
using System.Net.Http;
using CzomPack.Network;

namespace Markdig.Extensions.Xmdl.Lua.Helper;

public class XmdlNetworkHelper
{
    public ResponseData Send(string url, string method = "GET", string userAgent = null, string contentType = "text/plain; charset=utf-8", string payload = null, params string[] extraHeaders)
    {
        RequestMethod requestMethod = RequestMethod.GET;
        try
        {
            requestMethod = Enum.Parse<RequestMethod>(method);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
        }
        return NetHandler.SendRequest(url, requestMethod, userAgent, contentType, payload, extraHeaders);
    }
}