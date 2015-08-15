using System;
using System.IO;
using AldursLab.WurmApi.Extensions.DotNet.Reflection;
using AldursLab.WurmApi.Modules.Wurm.InstallDirectory;
using JetBrains.Annotations;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Creates new instances of WurmApi systems.
    /// </summary>
    public static class WurmApiFactory
    {
        /// <summary>
        /// Creates new instance of WurmApiManager.
        /// All WurmApi services are thread safe and independent of execution context (i.e. synchronization context, async handling).
        /// Always call Dispose on the Manager, before closing app or dropping the instance. This will ensure proper cleanup and internal cache consistency.
        /// Not calling Dispose without terminating hosting process, may result in resource leaks.
        /// </summary>
        /// <param name="dataDirPath">
        /// Directory path, where this instance of WurmApiManager can store data caches. Defaults to \WurmApi in the library DLL location. 
        /// If relative path is provided, it will also be in relation to DLL location.
        /// </param>
        /// <param name="logger">
        /// An optional implementation of ILogger, where all WurmApi errors and warnings can be forwarded. 
        /// Defaults to no logging. 
        /// Providing this parameter is highly is recommended.
        /// </param>
        /// <param name="eventMarshaller">
        /// An optional event marshaller, used to marshal all events in a specific way (for example to GUI thread).
        /// Defaults to running events on ThreadPool threads.
        /// Providing this parameter will greatly simplify usage in any application, that has synchronization context.
        /// </param>
        /// <param name="installDirectory">
        /// An optional Wurm Game Client directory path provider.
        /// Defaults to autodetection.
        /// </param>
        /// <param name="config">
        /// Optional extra configuration options.
        /// </param>
        /// <returns></returns>
        public static IWurmApi Create(string dataDirPath = null, ILogger logger = null,
            IEventMarshaller eventMarshaller = null, IWurmInstallDirectory installDirectory = null, WurmApiConfig config = null)
        {
            if (dataDirPath == null)
            {
                dataDirPath = "WurmApi";
            }
            if (!Path.IsPathRooted(dataDirPath))
            {
                var codebase = typeof(WurmApiFactory).Assembly.GetAssemblyDllDirectoryAbsolutePath();
                dataDirPath = Path.Combine(codebase, dataDirPath);
            }
            if (logger == null)
            {
                logger = new LoggerStub();
            }
            if (installDirectory == null)
            {
                installDirectory = new WurmInstallDirectory();
            }
            if (config == null)
            {
                config = new WurmApiConfig();
            }
            return new WurmApiManager(new WurmApiDataDirectory(dataDirPath, true),
                installDirectory,
                logger,
                eventMarshaller,
                config);
        }
    }
}