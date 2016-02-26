using System;
using System.IO;
using AldursLab.WurmApi.PersistentObjects.SqLite.Migrations;
using AldursLab.WurmApi.PersistentObjects.SqLite.Model;

namespace AldursLab.WurmApi.PersistentObjects.SqLite
{
    public class SqLitePersistenceStrategy : IPersistenceStrategy
    {
        readonly PersistentDataRepository repository;

        public SqLitePersistenceStrategy(string storageDirectoryAbsolutePath)
        {
            if (storageDirectoryAbsolutePath == null) throw new ArgumentNullException(nameof(storageDirectoryAbsolutePath));

            if (!Directory.Exists(storageDirectoryAbsolutePath))
            {
                Directory.CreateDirectory(storageDirectoryAbsolutePath);
            }

            string connectionString = string.Format(@"Data Source={0};Version=3;", Path.Combine(storageDirectoryAbsolutePath, "data.db"));

            var migrator = new DbMigrator(connectionString);
            migrator.Migrate();

            repository = new PersistentDataRepository(connectionString);
        }

        public string TryLoad(string objectId, string collectionId)
        {
            try
            {
                var data = repository.Read(collectionId, objectId);
                if (string.IsNullOrEmpty(data.Data))
                {
                    return null;
                }
                else
                {
                    return data.Data;
                }
            }
            catch (Exception exception)
            {
                throw new PersistenceException("Db load failed. See inner exception for details", exception);
            }
        }

        public void Save(string objectId, string collectionId, string content)
        {
            try
            {
                repository.Update(new PersistentData()
                {
                    ObjectId = objectId,
                    CollectionId = collectionId,
                    Data = content
                });
            }
            catch (Exception exception)
            {
                throw new PersistenceException("Db save failed. See inner exception for details", exception);
            }
        }
    }
}