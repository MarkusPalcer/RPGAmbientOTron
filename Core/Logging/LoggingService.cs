using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using log4net;

namespace Core.Logging
{
    [Export(typeof(ILoggingService))]
    public class LoggingService : ILoggingService
    {
        private readonly Dictionary<Type, ILog> loggers = new Dictionary<Type, ILog>();

        private ILog GetLogger<T>()
        {
            ILog logger;
            if (loggers.TryGetValue(typeof(T), out logger))
                return logger;

            logger = LogManager.GetLogger(typeof(T));
            loggers[typeof(T)] = logger;

            return logger;
        }

        #region Implementation of ILoggingService

        public void Debug<T>(object message)
        {
            GetLogger<T>().Debug(message);
        }

        public void Debug<T>(object message, Exception exception)
        {
            GetLogger<T>().Debug(message, exception);
        }

        public void Info<T>(object message)
        {
            GetLogger<T>().Info(message);
        }

        public void Info<T>(object message, Exception exception)
        {
            GetLogger<T>().Info(message, exception);
        }

        public void Warn<T>(object message)
        {
            GetLogger<T>().Warn(message);
        }

        public void Warn<T>(object message, Exception exception)
        {
            GetLogger<T>().Warn(message, exception);
        }

        public void Error<T>(object message)
        {
            GetLogger<T>().Error(message);
        }

        public void Error<T>(object message, Exception exception)
        {
            GetLogger<T>().Error(message, exception);
        }

        public void Fatal<T>(object message)
        {
            GetLogger<T>().Fatal(message);
        }

        public void Fatal<T>(object message, Exception exception)
        {
            GetLogger<T>().Fatal(message,exception);
        }
        #endregion
    }
}