using System;

namespace Core.Logging
{
    public interface ILoggingService
    {
        /// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Debug" /> level.</overloads>
        /// <summary>
        /// Log a message object with the <see cref="F:log4net.Core.Level.Debug" /> level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <remarks>
        /// <para>
        /// This method first checks if this logger is <c>DEBUG</c>
        /// enabled by comparing the level of this logger with the
        /// <see cref="F:log4net.Core.Level.Debug" /> level. If this logger is
        /// <c>DEBUG</c> enabled, then it converts the message object
        /// (passed as parameter) to a string by invoking the appropriate
        /// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then
        /// proceeds to call all the registered appenders in this logger
        /// and also higher in the hierarchy depending on the value of
        /// the additivity flag.
        /// </para>
        /// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" />
        /// to this method will print the name of the <see cref="T:System.Exception" />
        /// but no stack trace. To print a stack trace use the
        /// <see cref="M:Debug(object,Exception)" /> form instead.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Debug(object,Exception)" />
        /// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
        void Debug<T>(object message);

        /// <summary>
        /// Log a message object with the <see cref="F:log4net.Core.Level.Debug" /> level including
        /// the stack trace of the <see cref="T:System.Exception" /> passed
        /// as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        /// <remarks>
        /// <para>
        /// See the <see cref="M:Debug(object)" /> form for more detailed information.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Debug(object)" />
        /// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
        void Debug<T>(object message, Exception exception);
        
        /// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Info" /> level.</overloads>
        /// <summary>
        /// Logs a message object with the <see cref="F:log4net.Core.Level.Info" /> level.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method first checks if this logger is <c>INFO</c>
        /// enabled by comparing the level of this logger with the
        /// <see cref="F:log4net.Core.Level.Info" /> level. If this logger is
        /// <c>INFO</c> enabled, then it converts the message object
        /// (passed as parameter) to a string by invoking the appropriate
        /// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then
        /// proceeds to call all the registered appenders in this logger
        /// and also higher in the hierarchy depending on the value of the
        /// additivity flag.
        /// </para>
        /// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" />
        /// to this method will print the name of the <see cref="T:System.Exception" />
        /// but no stack trace. To print a stack trace use the
        /// <see cref="M:Info(object,Exception)" /> form instead.
        /// </para>
        /// </remarks>
        /// <param name="message">The message object to log.</param>
        /// <seealso cref="M:Info(object,Exception)" />
        /// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
        void Info<T>(object message);

        /// <summary>
        /// Logs a message object with the <c>INFO</c> level including
        /// the stack trace of the <see cref="T:System.Exception" /> passed
        /// as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        /// <remarks>
        /// <para>
        /// See the <see cref="M:Info(object)" /> form for more detailed information.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Info(object)" />
        /// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
        void Info<T>(object message, Exception exception);

        /// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Warn" /> level.</overloads>
        /// <summary>
        /// Log a message object with the <see cref="F:log4net.Core.Level.Warn" /> level.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method first checks if this logger is <c>WARN</c>
        /// enabled by comparing the level of this logger with the
        /// <see cref="F:log4net.Core.Level.Warn" /> level. If this logger is
        /// <c>WARN</c> enabled, then it converts the message object
        /// (passed as parameter) to a string by invoking the appropriate
        /// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then
        /// proceeds to call all the registered appenders in this logger
        /// and also higher in the hierarchy depending on the value of the
        /// additivity flag.
        /// </para>
        /// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" />
        /// to this method will print the name of the <see cref="T:System.Exception" />
        /// but no stack trace. To print a stack trace use the
        /// <see cref="M:Warn(object,Exception)" /> form instead.
        /// </para>
        /// </remarks>
        /// <param name="message">The message object to log.</param>
        /// <seealso cref="M:Warn(object,Exception)" />
        /// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
        void Warn<T>(object message);

        /// <summary>
        /// Log a message object with the <see cref="F:log4net.Core.Level.Warn" /> level including
        /// the stack trace of the <see cref="T:System.Exception" /> passed
        /// as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        /// <remarks>
        /// <para>
        /// See the <see cref="M:Warn(object)" /> form for more detailed information.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Warn(object)" />
        /// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
        void Warn<T>(object message, Exception exception);

        /// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Error" /> level.</overloads>
        /// <summary>
        /// Logs a message object with the <see cref="F:log4net.Core.Level.Error" /> level.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <remarks>
        /// <para>
        /// This method first checks if this logger is <c>ERROR</c>
        /// enabled by comparing the level of this logger with the
        /// <see cref="F:log4net.Core.Level.Error" /> level. If this logger is
        /// <c>ERROR</c> enabled, then it converts the message object
        /// (passed as parameter) to a string by invoking the appropriate
        /// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then
        /// proceeds to call all the registered appenders in this logger
        /// and also higher in the hierarchy depending on the value of the
        /// additivity flag.
        /// </para>
        /// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" />
        /// to this method will print the name of the <see cref="T:System.Exception" />
        /// but no stack trace. To print a stack trace use the
        /// <see cref="M:Error(object,Exception)" /> form instead.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Error(object,Exception)" />
        /// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
        void Error<T>(object message);

        /// <summary>
        /// Log a message object with the <see cref="F:log4net.Core.Level.Error" /> level including
        /// the stack trace of the <see cref="T:System.Exception" /> passed
        /// as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        /// <remarks>
        /// <para>
        /// See the <see cref="M:Error(object)" /> form for more detailed information.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Error(object)" />
        /// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
        void Error<T>(object message, Exception exception);

        /// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Fatal" /> level.</overloads>
        /// <summary>
        /// Log a message object with the <see cref="F:log4net.Core.Level.Fatal" /> level.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method first checks if this logger is <c>FATAL</c>
        /// enabled by comparing the level of this logger with the
        /// <see cref="F:log4net.Core.Level.Fatal" /> level. If this logger is
        /// <c>FATAL</c> enabled, then it converts the message object
        /// (passed as parameter) to a string by invoking the appropriate
        /// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then
        /// proceeds to call all the registered appenders in this logger
        /// and also higher in the hierarchy depending on the value of the
        /// additivity flag.
        /// </para>
        /// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" />
        /// to this method will print the name of the <see cref="T:System.Exception" />
        /// but no stack trace. To print a stack trace use the
        /// <see cref="M:Fatal(object,Exception)" /> form instead.
        /// </para>
        /// </remarks>
        /// <param name="message">The message object to log.</param>
        /// <seealso cref="M:Fatal(object,Exception)" />
        /// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
        void Fatal<T>(object message);

        /// <summary>
        /// Log a message object with the <see cref="F:log4net.Core.Level.Fatal" /> level including
        /// the stack trace of the <see cref="T:System.Exception" /> passed
        /// as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        /// <remarks>
        /// <para>
        /// See the <see cref="M:Fatal(object)" /> form for more detailed information.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Fatal(object)" />
        /// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
        void Fatal<T>(object message, Exception exception);
    }
}