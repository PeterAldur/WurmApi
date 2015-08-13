using System;
using AldursLab.WurmApi.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Utility
{
    class PersObjErrorHandlingStrategy : IObjectDeserializationErrorHandlingStrategy
    {
        readonly ILogger logger;

        public PersObjErrorHandlingStrategy([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        public void Handle(ErrorContext errorContext)
        {
            logger.Log(LogLevel.Error,
                "Persistent object deserialization error, will ignore and use defaults, error details: "
                + errorContext.GetErrorDetailsAsString(), "WurmApi", null);
            errorContext.Decision = Decision.IgnoreErrorsAndReturnDefaultsForMissingData;
        }
    }
}
