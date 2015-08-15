using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using AldursLab.WurmApi.Tests.TempDirs;
using NUnit.Framework;
using Telerik.JustMock;

namespace AldursLab.WurmApi.Tests.Tests.Modules.Wurm.LogReading
{
    class LogFileStreamReaderTests : TestsBase
    {
        LogFileStreamReaderFactory System;
        WurmApiConfig wurmApiConfig;

        [SetUp]
        public void Setup()
        {
            wurmApiConfig = new WurmApiConfig();
            wurmApiConfig.Platform = Platform.Linux;
            System = new LogFileStreamReaderFactory(wurmApiConfig);
        }

        [Test]
        public void LinuxLogs_ResolvesAndReadsCorrectly()
        {
            wurmApiConfig.Platform = Platform.Linux;

            using (var ubuntuDir =
                TempDirectoriesFactory.CreateByUnzippingFile(Path.Combine(TestPaksZippedDirFullPath,
                    "ubuntu-wurm-dir.7z")))
            {
                var sampleLogFilePath = Path.Combine(ubuntuDir.AbsolutePath,
                    "players",
                    "aldur",
                    "logs",
                    "_Event.2015-08.txt");
                using (var reader = System.Create(sampleLogFilePath))
                {
                    List<string> lines = new List<string>();
                    string line;
                    while ((line = reader.TryReadNextLine()) != null)
                    {
                        lines.Add(line);
                    }
                    Expect(lines[0], EqualTo("Logging started 2015-08-16"));
                    Expect(lines[1], EqualTo("[00:08:05] You will now fight normally."));
                    Expect(lines[27], EqualTo("[00:19:43] You are no longer invulnerable."));
                }

            }
        }
    }
}
