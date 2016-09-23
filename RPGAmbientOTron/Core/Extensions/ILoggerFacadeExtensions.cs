using System;
using Prism.Logging;

namespace Core.Extensions
{
    public static class ILoggerFacadeExtensions
    {
        public static void LogException(this ILoggerFacade logger, Exception ex)
        {
            logger.Log($"{ex.Message}\n{ex.StackTrace}", Category.Exception, Priority.High);
        }
    }
}