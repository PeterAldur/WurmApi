using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.FileSystem;

namespace AldursLab.PersistentObjects.FlatFiles
{
    public class FlatFilesPersistenceStrategy : IPersistenceStrategy
    {
        readonly string storageDirectoryAbsolutePath;

        public FlatFilesPersistenceStrategy(string storageDirectoryAbsolutePath)
        {
            if (storageDirectoryAbsolutePath == null) throw new ArgumentNullException("storageDirectoryAbsolutePath");
            this.storageDirectoryAbsolutePath = storageDirectoryAbsolutePath;

            if (!Directory.Exists(storageDirectoryAbsolutePath))
            {
                Directory.CreateDirectory(storageDirectoryAbsolutePath);
            }
        }

        public string Load(string objectId, string collectionId)
        {
            try
            {
                var filepath = Path.Combine(storageDirectoryAbsolutePath, collectionId, objectId + ".json");
                if (!File.Exists(filepath))
                {
                    return null;
                }
                var data = TransactionalFileOps.ReadFileContents(filepath);
                return data;
            }
            catch (Exception exception)
            {
                throw new PersistenceException("Flat File loading failed. See inner exception for details", exception);
            }
        }

        public void Save(string objectId, string collectionId, string content)
        {
            try
            {
                var filepath = Path.Combine(storageDirectoryAbsolutePath, collectionId, objectId + ".json");
                TransactionalFileOps.SaveFileContents(filepath, content);
            }
            catch (Exception exception)
            {
                throw new PersistenceException("Flat File saving failed. See inner exception for details", exception);
            }
        }
    }
}
