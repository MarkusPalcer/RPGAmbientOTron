using System;
using System.ComponentModel.Composition;
using log4net;
using Prism.Logging;

namespace Core
{
    [Export(typeof(ILoggerFacade))]
    public class LoggerFacade : ILoggerFacade
    {
        private readonly ILog logger = log4net.LogManager.GetLogger("LoggerFacade");


        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    logger.Debug(message);
                    break;
                case Category.Exception:
                    logger.Fatal(message);
                    break;
                case Category.Info:
                    logger.Info(message);
                    break;
                case Category.Warn:
                    logger.Warn(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}