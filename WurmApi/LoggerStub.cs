using System;

namespace AldursLab.WurmApi
{
    public class LoggerStub : IWurmApiLogger
    {
        public void Log(LogLevel level, string message, object source, Exception exception)
        {
        }
    }
}