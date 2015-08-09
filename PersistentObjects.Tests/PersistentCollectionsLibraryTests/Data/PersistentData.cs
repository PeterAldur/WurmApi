namespace AldursLab.PersistentObjects.Tests.PersistentCollectionsLibraryTests.Data
{
    public class PersistentData : PersistentEntityBase<Dto>
    {
        public PersistentData(IPersistent<Dto> persistent) : base(persistent)
        {
        }

        public string Data
        {
            get { return Entity.Data; }
            set
            {
                Entity.Data = value;
                FlagAsChanged();
            }
        }
    }

    public class PersistentDataWithMigration : PersistentEntityBase<Dto>
    {
        public PersistentDataWithMigration(IPersistent<Dto> persistent)
            : base(persistent)
        {
            RunMigration(0, 1, dto =>
            {
                dto.Data = TestValues.ValueAfterMigration;
            },
                dto => dto.Data == TestValues.Value
                );
        }

        public string Data
        {
            get { return Entity.Data; }
            set
            {
                Entity.Data = value;
                FlagAsChanged();
            }
        }
    }

    public class PersistentDataWithMigrationNoFilter : PersistentEntityBase<Dto>
    {
        public PersistentDataWithMigrationNoFilter(IPersistent<Dto> persistent)
            : base(persistent)
        {
            RunMigration(0, 1, dto =>
            {
                dto.Data = TestValues.ValueAfterMigration;
            });
        }

        public string Data
        {
            get { return Entity.Data; }
            set
            {
                Entity.Data = value;
                FlagAsChanged();
            }
        }
    }
}