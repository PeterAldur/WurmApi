using AldursLab.WurmApi.PersistentObjects;

namespace AldursLab.WurmApi.Tests.Tests.PersistentCollectionsLibraryTests.Data
{
    public class Dto : Entity
    {
        public string Data { get; set; }

        public int OldField { get; set; }
    }

    public class DtoClone : Entity
    {
        public string Data { get; set; }

        public int NewField { get; set; }
    }

    public class DtoWithDefaults : Entity
    {
        public DtoWithDefaults()
        {
            Data = TestValues.Default;
        }
        
        public string Data { get; set; }

        public int NumericValue { get; set; }
    }
}