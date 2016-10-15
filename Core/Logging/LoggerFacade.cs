using System;
using System.ComponentModel.Composition;
using log4net;
using Prism.Logging;

namespace Core.Logging
{
  [Export(typeof(ILoggerFacade))]
  public class LoggerFacade : ILoggerFacade
  {
    private readonly ILog logger = LogManager.GetLogger(typeof(ILoggerFacade));


    public void Log(string message, Category category, Priority priority)
    {
      switch (category)
      {
        case Category.Debug:
          logger.Debug(message);
          break;
        case Category.Exception:
          logger.Error(message);
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
