using System;
using AldursLab.WurmApi.Modules.Events.Internal;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    class CharacterLogsMonitorEngineFactory
    {
        readonly ILogger logger;
        readonly SingleFileMonitorFactory singleFileMonitorFactory;
        readonly IWurmCharacterLogFiles wurmCharacterLogFiles;
        readonly IInternalEventAggregator internalEventAggregator;

        public CharacterLogsMonitorEngineFactory(
            ILogger logger,
            SingleFileMonitorFactory singleFileMonitorFactory,
            IWurmCharacterLogFiles wurmCharacterLogFiles, 
            [NotNull] IInternalEventAggregator internalEventAggregator)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (singleFileMonitorFactory == null) throw new ArgumentNullException("singleFileMonitorFactory");
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException("wurmCharacterLogFiles");
            if (internalEventAggregator == null) throw new ArgumentNullException("internalEventAggregator");
            this.logger = logger;
            this.singleFileMonitorFactory = singleFileMonitorFactory;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;
            this.internalEventAggregator = internalEventAggregator;
        }

        public CharacterLogsMonitorEngine Create(CharacterName characterName)
        {
            return new CharacterLogsMonitorEngine(characterName, logger, singleFileMonitorFactory, wurmCharacterLogFiles,
                internalEventAggregator);
        }
    }
}