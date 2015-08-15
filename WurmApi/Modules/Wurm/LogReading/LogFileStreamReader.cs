using System;
using System.IO;
using System.Text;
using AldursLab.WurmApi.Extensions.DotNet;

namespace AldursLab.WurmApi.Modules.Wurm.LogReading
{
    abstract class LogFileStreamReader : IDisposable
    {
        private readonly long startPosition;
        private readonly bool trackFileBytePositions;
        protected readonly StreamReader StreamReader;

        protected int CurrentResult;
        protected int NextResult;
        protected char CurrentChar;
        protected char NextChar;

        protected readonly StringBuilder StringBuilder = new StringBuilder();

        public LogFileStreamReader(string fileFullPath, long startPosition = 0, bool trackFileBytePositions = false)
        {
            if (fileFullPath == null)
                throw new ArgumentNullException("fileFullPath");
            if (startPosition < 0)
                throw new ArgumentException("startPosition must be non-negative");
            this.startPosition = startPosition;
            this.trackFileBytePositions = trackFileBytePositions;

            var stream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader = new StreamReader(stream, Encoding.Default);
            if (startPosition != 0)
            {
                StreamReader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
            }
            LastReadLineIndex = -1;
        }

        /// <summary>
        /// After reaching end of file, this points to the last character.
        /// </summary>
        public long LastReadLineStartPosition { get; private set; }

        public void Seek(long offsetFromBeginning)
        {
            StreamReader.BaseStream.Seek(offsetFromBeginning, SeekOrigin.Begin);
        }

        private void UpdateCurrentLineStartPosition()
        {
            long charlen = GetCharLenAccessor(StreamReader);
            long charpos = GetCharPosAccessor(StreamReader);
            LastReadLineStartPosition = StreamReader.BaseStream.Position - charlen + charpos;
        }

        private static readonly Func<StreamReader, int> GetCharPosAccessor = ReflectionHelper.GetFieldAccessor<StreamReader, int>("charPos");
        private static readonly Func<StreamReader, int> GetCharLenAccessor = ReflectionHelper.GetFieldAccessor<StreamReader, int>("charLen");

        /// <summary>
        /// -1 if no lines read yet.
        /// After reaching end of file, this still indicates last line index.
        /// </summary>
        public int LastReadLineIndex { get; private set; }

        public long StreamPosition
        {
            get { return StreamReader.BaseStream.Position; }
        }

        private bool endOfStreamPositionUpdated = false;

        public string TryReadNextLine()
        {
            var pk = StreamReader.Peek();
            if (pk == -1)
            {
                if (trackFileBytePositions && !endOfStreamPositionUpdated)
                {
                    UpdateCurrentLineStartPosition();
                    endOfStreamPositionUpdated = true;
                }
                return null;
            }
            StringBuilder.Clear();
            LastReadLineIndex++;
            if (trackFileBytePositions)
            {
                UpdateCurrentLineStartPosition();
            }

            return ReadCharsForNextLine();
        }

        protected abstract string ReadCharsForNextLine();

        public void Dispose()
        {
            StreamReader.Dispose();
        }
    }
}