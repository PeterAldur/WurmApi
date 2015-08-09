using System;

namespace AldursLab.PersistentObjects.Tests.PersistentCollectionsLibraryTests
{
    public class CustomDeserializationErrorHandler : IObjectDeserializationErrorHandlingStrategy
    {
        readonly Action<ErrorContext> action;

        public CustomDeserializationErrorHandler(Action<ErrorContext> action)
        {
            this.action = action;
        }

        public void Handle(ErrorContext errorContext)
        {
            action(errorContext);
        }
    }
}