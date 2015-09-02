namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    public struct DayInfo
    {
        private readonly long startPositionInBytes;
        private readonly int linesLength;
        private readonly int totalLinesSinceBeginFile;

        public DayInfo(long startPositionInBytes, int linesLength, int totalLinesSinceBeginFile)
        {
            this.startPositionInBytes = startPositionInBytes;
            this.linesLength = linesLength;
            this.totalLinesSinceBeginFile = totalLinesSinceBeginFile;
        }

        public long StartPositionInBytes
        {
            get { return startPositionInBytes; }
        }

        public int LinesLength
        {
            get { return linesLength; }
        }

        public int TotalLinesSinceBeginFile
        {
            get { return totalLinesSinceBeginFile; }
        }
    }
}