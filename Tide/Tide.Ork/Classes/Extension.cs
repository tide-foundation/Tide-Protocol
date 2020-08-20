using System;
using Microsoft.AspNetCore.Mvc;

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
    }
}