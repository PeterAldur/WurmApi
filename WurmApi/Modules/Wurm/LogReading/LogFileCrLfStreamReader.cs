namespace AldursLab.WurmApi.Modules.Wurm.LogReading
{
    class LogFileCrLfStreamReader : LogFileStreamReader
    {
        public LogFileCrLfStreamReader(string fileFullPath, long startPosition = 0, bool trackFileBytePositions = false)
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
                CurrentChar = (char)CurrentResult;
                if (CurrentChar == '\r')
                {
                    NextResult = StreamReader.Read();
                    if (NextResult != -1)
                    {
                        NextChar = (char)NextResult;
                        if (NextChar == '\n')
                        {
                            return StringBuilder.ToString();
                        }
                        else
                        {
                            StringBuilder.Append(CurrentChar);
                            StringBuilder.Append(NextChar);
                        }
                    }
                    else
                    {
                        StringBuilder.Append(CurrentChar);
                    }
                }
                else
                {
                    StringBuilder.Append(CurrentChar);
                }
            }
        }
    }
}