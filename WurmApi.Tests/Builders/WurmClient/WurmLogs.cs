using System.IO;
using System.Text;

namespace AldursLab.WurmApi.Tests.Builders.WurmClient
{
    class WurmLogs
    {
        readonly DirectoryInfo logsDir;
        readonly Platform targetPlatform;

        public WurmLogs(DirectoryInfo logsDir, Platform targetPlatform)
        {
            this.logsDir = logsDir;
            this.targetPlatform = targetPlatform;
        }

        public WurmLogs CreateLogFile(string name, string content = null)
        {
            if (content == null) content = string.Empty;
            if (targetPlatform != Platform.Windows)
            {
                content = content.Replace("\r\n", "\n");
            }
            File.WriteAllText(Path.Combine(logsDir.FullName, name), content, Encoding.UTF8);
            return this;
        }
    }
}