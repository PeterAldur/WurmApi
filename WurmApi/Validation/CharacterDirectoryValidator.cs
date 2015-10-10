﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmApi.Validation
{
    static class CharacterDirectoryValidator
    {
        public static void ValidateFullPath(string directoryPath)
        {
            var dirInfo = new DirectoryInfo(directoryPath);
            if (!dirInfo.Exists)
            {
                throw new ValidationException("Directory does not exist: " + directoryPath);
            }
            var files = dirInfo.EnumerateFiles();
            var configFile =
                files.FirstOrDefault(info => info.Name.Equals("config.txt", StringComparison.InvariantCultureIgnoreCase));
            if (configFile == null)
            {
                throw new ValidationException("config.txt does not exist at: " + directoryPath);
            }
            var contents = File.ReadAllText(configFile.FullName);
            if (string.IsNullOrWhiteSpace(contents))
            {
                throw new ValidationException("config.txt is empty at: " + directoryPath);
            }
            var directories = dirInfo.EnumerateDirectories();
            var logsDir =
                directories.FirstOrDefault(info => info.Name.Equals("logs", StringComparison.InvariantCultureIgnoreCase));
            if (logsDir == null)
            {
                throw new ValidationException("logs subdir does not exist, character dir path: " + directoryPath);
            }
        }
    }
}
