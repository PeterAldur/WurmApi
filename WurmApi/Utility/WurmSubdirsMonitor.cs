using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Validation;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Utility
{
    /// <summary>
    /// Provides cached info about subdirectories and notifies when they change.
    /// </summary>
    abstract class WurmSubdirsMonitor : IDisposable
    {
        protected readonly string DirectoryFullPath;
        readonly TaskManager taskManager;
        readonly Action onChanged;
        readonly Action<string> validateDirectory;
        readonly ILogger logger;
        readonly FileSystemWatcher fileSystemWatcher;

        IReadOnlyDictionary<string, string> dirNameToFullPathMap = new Dictionary<string, string>();

        readonly TaskHandle task;

        readonly Blacklist<string> directoryBlacklist;

        public WurmSubdirsMonitor([NotNull] string directoryFullPath, [NotNull] TaskManager taskManager,
            [NotNull] Action onChanged, [NotNull] ILogger logger,
            [NotNull] Action<string> validateDirectory)
        {
            if (directoryFullPath == null) throw new ArgumentNullException("directoryFullPath");
            if (taskManager == null) throw new ArgumentNullException("taskManager");
            if (onChanged == null) throw new ArgumentNullException("onChanged");
            if (validateDirectory == null) throw new ArgumentNullException("validateDirectory");
            if (logger == null) throw new ArgumentNullException("logger");
            this.DirectoryFullPath = directoryFullPath;
            this.taskManager = taskManager;
            this.onChanged = onChanged;
            this.validateDirectory = validateDirectory;
            this.logger = logger;

            directoryBlacklist = new Blacklist<string>(logger, "Character directories blacklist");

            task = new TaskHandle(Refresh, "WurmSubdirsMonitor for path: " + directoryFullPath);
            taskManager.Add(task);

            try
            {
                Refresh();
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Error at initial Refresh of " + this.GetType().Name, this, exception);
            }

            fileSystemWatcher = new FileSystemWatcher(directoryFullPath) {NotifyFilter = NotifyFilters.DirectoryName};
            fileSystemWatcher.Created += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Renamed += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Deleted += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Changed += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.EnableRaisingEvents = true;

            task.Trigger();
        }

        private void DirectoryMonitorOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            task.Trigger();
        }

        private void Refresh()
        {
            List<Exception> exceptions = new List<Exception>();

            var di = new DirectoryInfo(DirectoryFullPath);
            var allDirs = di.GetDirectories();
            var newMap = new Dictionary<string, string>();

            foreach (var directoryInfo in allDirs)
            {
                if (directoryBlacklist.IsOnBlacklist(directoryInfo.FullName))
                {
                    continue;
                }
                try
                {
                    validateDirectory(directoryInfo.FullName);
                    newMap.Add(directoryInfo.Name.ToUpperInvariant(), directoryInfo.FullName);
                }
                catch (ValidationException exception)
                {
                    directoryBlacklist.ReportIssue(directoryInfo.FullName);
                    exceptions.Add(exception);
                }
            }
            
            var oldDirs = dirNameToFullPathMap.Select(pair => pair.Key).OrderBy(s => s).ToArray();
            var newDirs = newMap.Select(pair => pair.Key).OrderBy(s => s).ToArray();

            var changed = !oldDirs.SequenceEqual(newDirs);

            if (changed)
            {
                dirNameToFullPathMap = newMap;
                OnDirectoriesChanged();
            }

            if (exceptions.Any())
            {
                throw new AggregateException("At least one directory failed validation.", exceptions);
            }
        }

        private void OnDirectoriesChanged()
        {
            onChanged();
        }

        public IEnumerable<string> AllDirectoryNamesNormalized
        {
            get
            {
                return dirNameToFullPathMap.Keys;
            }
        }

        public IEnumerable<string> AllDirectoriesFullPaths
        {
            get
            {
                return this.dirNameToFullPathMap.Values;
            }
        }

        public void Dispose()
        {
            taskManager.Remove(task);
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }

        protected string GetFullPathForDirName([NotNull] string dirName)
        {
            if (dirName == null) throw new ArgumentNullException("dirName");

            string directoryFullPath;
            if (!dirNameToFullPathMap.TryGetValue(dirName.ToUpperInvariant(), out directoryFullPath))
            {
                throw new DataNotFoundException(dirName);
            }
            return directoryFullPath;
        }
    }
}
