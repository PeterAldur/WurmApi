using System;

namespace AldursLab.WurmApi
{
    public class LoggerStub : ILogger
    {
        public void Log(LogLevel level, string message, object source, Exception exception)
        {
        }
    }
}