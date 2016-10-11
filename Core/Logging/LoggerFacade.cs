using System;
using System.ComponentModel.Composition;
using Prism.Logging;

namespace Core.Logging
{
    [Export(typeof(ILoggerFacade))]
    public class LoggerFacade : ILoggerFacade
    {
        private readonly ILoggingService loggingService;
        public LoggerFacade(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    loggingService.Debug<ILoggerFacade>(message);
                    break;
                case Category.Exception:
                    loggingService.Error<ILoggerFacade>(message);
                    break;
                case Category.Info:
                    loggingService.Info<ILoggerFacade>(message);
                    break;
                case Category.Warn:
                    loggingService.Warn<ILoggerFacade>(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}