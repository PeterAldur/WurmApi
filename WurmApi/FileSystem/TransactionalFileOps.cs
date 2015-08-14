using System;
using System.IO;
using System.Text;

namespace AldursLab.WurmApi.FileSystem
{
    static class TransactionalFileOps
    {
        private const string FileExtensionNew = ".new";
        private const string FileExtensionOld = ".old";

        /// <summary>
        /// Empty string if nothing found.
        /// </summary>
        public static string ReadFileContents(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                if (File.Exists(absolutePath + FileExtensionNew))
                {
                    File.Move(absolutePath + FileExtensionNew, absolutePath);
                }
                else if (File.Exists(absolutePath + FileExtensionOld))
                {
                    File.Move(absolutePath + FileExtensionOld, absolutePath);
                }
                else
                {
                    return string.Empty;
                }
            }
            using (FileStream fileStream = File.OpenRead(absolutePath))
            {
                using (var sr = new StreamReader(fileStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static void SaveFileContents(string absolutePath, string newContents)
        {
            var dir = Path.GetDirectoryName(absolutePath);
            if (dir == null)
            {
                throw new InvalidOperationException("Path.GetDirectoryName(absolutePath) returned null for filepath: " + absolutePath);
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(absolutePath + FileExtensionNew))
            {
                File.Delete(absolutePath + FileExtensionNew);
            }
            if (File.Exists(absolutePath + FileExtensionOld))
            {
                File.Delete(absolutePath + FileExtensionOld);
            }

            File.WriteAllText(absolutePath + FileExtensionNew, newContents, Encoding.UTF8);

            if (File.Exists(absolutePath))
            {
                File.Move(absolutePath, absolutePath + FileExtensionOld);
            }
            File.Move(absolutePath + FileExtensionNew, absolutePath);
            File.Delete(absolutePath + FileExtensionOld);
        }
    }
}
