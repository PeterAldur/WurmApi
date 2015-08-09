using AldursLab.PersistentObjects.FlatFiles;
using AldursLab.Testing;
using NUnit.Framework;

namespace AldursLab.PersistentObjects.Tests.PersistentCollectionsLibraryTests
{
    public class TestsBase : AssertionHelper
    {
        DirectoryHandle dir;

        [SetUp]
        public virtual void Setup()
        {
            dir = TempDirectoriesFactory.CreateEmpty();
        }

        [TearDown]
        public virtual void TearDown()
        {
            dir.Dispose();
        }

        protected PersistentCollectionsLibrary CreateLibrary(CustomDeserializationErrorHandler customDeserializationErrorHandler = null)
        {
            return new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(dir.AbsolutePath), customDeserializationErrorHandler);
        }
    }
}