using System;
using System.IO;
using System.Text;

namespace AldursLab.WurmApi.Tests.Builders.WurmClient
{
    class WurmLogs
    {
        readonly DirectoryInfo logsDir;
        readonly Platform targetPlatform;

        public LogSaveMode LogSaveMode { get; set; }

        public WurmLogs(DirectoryInfo logsDir, Platform targetPlatform)
        {
            this.logsDir = logsDir;
            this.targetPlatform = targetPlatform;
            LogSaveMode = LogSaveMode.Daily;
        }

        //todo: refactor log file methods

        public WurmLogs CreateCustomLogFile(string name, string content = null)
        {
            if (content == null) content = string.Empty;
            if (targetPlatform != Platform.Windows)
            {
                content = content.Replace("\r\n", "\n");
            }
            File.WriteAllText(Path.Combine(logsDir.FullName, name), content, Encoding.UTF8);
            return this;
        }

        public WurmLogs CreateEventLogFile()
        {
            var fileName = CreateCurrentEventLogFileName();
            CreateCustomLogFile(fileName, "Logging started " + Time.Get.LocalNow.ToString("yyyy-MM-dd") + "\r\n");
            return this;
        }

        public WurmLogs WriteEventLog(string content, string source = null)
        {
            var fileName = CreateCurrentEventLogFileName();
            WriteLogLine(fileName, content, null, Time.Get.LocalNow);
            return this;
        }

        void WriteLogLine(string fileName, string content, string source, DateTime now)
        {
            var file = GetLogFileInfo(fileName);
            if (!file.Exists)
            {
                CreateCustomLogFile(file.Name, "Logging started " + now.ToString("yyyy-MM-dd") + "\r\n");
            }
            string line = string.Format("[{0}] {1}{2}",
                now.ToString("HH:mm:ss"),
                source != null ? ("<" + source + "> ") : string.Empty,
                content);
            File.AppendAllLines(file.FullName, new[]{ line });
        }

        FileInfo GetLogFileInfo(string fileName)
        {
            return new FileInfo(Path.Combine(logsDir.FullName, fileName));
        }

        string CreateCurrentEventLogFileName()
        {
            var now = Time.Get.LocalNow;
            var datePart = LogSaveMode == LogSaveMode.Daily ? now.ToString("yyyy-MM-dd") : now.ToString("yyyy-mm");
            return string.Format("_Event.{0}.txt", datePart);
        }
    }
}