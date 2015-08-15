using System;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics;
using AldursLab.WurmApi.Utility;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory
{
    class LogsScannerFactory
    {
        private readonly LogFileParserFactory logFileParserFactory;
        private readonly LogFileStreamReaderFactory streamReaderFactory;
        private readonly MonthlyLogFilesHeuristics heuristics;
        private readonly IWurmLogFiles wurmLogFiles;
        private readonly ILogger logger;

        public LogsScannerFactory(
            LogFileParserFactory logFileParserFactory,
            LogFileStreamReaderFactory streamReaderFactory,
            MonthlyLogFilesHeuristics heuristics,
            IWurmLogFiles wurmLogFiles,
            ILogger logger)
        {
            if (logFileParserFactory == null)
                throw new ArgumentNullException("logFileParserFactory");
            if (streamReaderFactory == null)
                throw new ArgumentNullException("streamReaderFactory");
            if (heuristics == null)
                throw new ArgumentNullException("heuristics");
            if (wurmLogFiles == null)
                throw new ArgumentNullException("wurmLogFiles");
            if (logger == null)
                throw new ArgumentNullException("logger");
            this.logFileParserFactory = logFileParserFactory;
            this.streamReaderFactory = streamReaderFactory;
            this.heuristics = heuristics;
            this.wurmLogFiles = wurmLogFiles;
            this.logger = logger;
        }

        /// <summary>
        /// Extracts all lines matching scan parameters.
        /// </summary>
        /// <returns></returns>
        public LogsScanner Create(LogSearchParameters logSearchParameters, JobCancellationManager cancellationManager)
        {
            return new LogsScanner(
                logSearchParameters,
                cancellationManager,
                wurmLogFiles,
                heuristics,
                streamReaderFactory,
                logger,
                logFileParserFactory);
        }
    }
}