using System;
using Microsoft.AspNetCore.Mvc;
using Tide.Encryption.Tools;

namespace Microsoft.Extensions.Logging
{
    public static class Extension
    {

        public static ActionResult Log(this ILogger logger, ObjectResult result)
        {
            logger.LogInformation($"{result.StatusCode}: {result.Value}");
            return result;
        }

        public static ActionResult Log(this ILogger logger, ObjectResult result, string message)
        {
            logger.LogInformation($"{result.StatusCode}: {message}");
            return result;
        }

        public static ActionResult Log(this ILogger logger, ActionResult result, string message)
        {
            logger.LogInformation(message);
            return result;
        }

        public static bool FromBase64UrlString(this string data, out byte[] bytes)
        {
            try
            {
                bytes = Convert.FromBase64String(data.DecodeBase64Url());
                return true;
            }
            catch
            {
                bytes = null;
                return false;
            }
        }        
    }
}