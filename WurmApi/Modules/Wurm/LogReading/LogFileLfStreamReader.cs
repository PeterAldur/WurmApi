namespace AldursLab.WurmApi.Modules.Wurm.LogReading
{
    class LogFileLfStreamReader : LogFileStreamReader
    {
        public LogFileLfStreamReader(string fileFullPath, long startPosition = 0, bool trackFileBytePositions = false)
            : base(fileFullPath, startPosition, trackFileBytePositions)
        {
        }

        protected override string ReadCharsForNextLine()
        {
            while (true)
            {
                CurrentResult = StreamReader.Read();
                if (CurrentResult == -1)
                {
                    return StringBuilder.ToString();
                }
                CurrentChar = (char) CurrentResult;
                if (CurrentChar == '\n')
                {
                    return StringBuilder.ToString();
                }
                else
                {
                    StringBuilder.Append(CurrentChar);
                }
            }
        }
    }
}