using System;
using System.Text;
using Prism.Logging;

namespace Core.Extensions
{
    public static class LoggerFacadeExtensions
    {
        public static void LogException(this ILoggerFacade logger, Exception ex, string message = null)
        {
            var s = new StringBuilder();

            if (message != null)
            {
                s.AppendLine(message);
            }

            s.AppendLine($"{ex.Message}");
            s.AppendLine($"{ex.StackTrace}");


            logger.Log(s.ToString(), Category.Exception, Priority.High);
        }
    }
}