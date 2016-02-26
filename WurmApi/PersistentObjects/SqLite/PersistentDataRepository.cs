using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi.PersistentObjects.SqLite.Migrations;
using AldursLab.WurmApi.PersistentObjects.SqLite.Model;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.PersistentObjects.SqLite
{
    public class PersistentDataRepository
    {
        readonly string connectionString;

        public PersistentDataRepository([NotNull] string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            this.connectionString = connectionString;
        }

        public PersistentData Read([NotNull] string collectionId, [NotNull] string objectId)
        {
            if (collectionId == null) throw new ArgumentNullException(nameof(collectionId));
            if (objectId == null) throw new ArgumentNullException(nameof(objectId));

            var data = GetData(collectionId, objectId);

            if (data.Rows.Count == 0)
            {
                return new PersistentData()
                {
                    CollectionId = collectionId,
                    ObjectId = objectId,
                    Data = string.Empty
                };
            }
            else
            {
                return new PersistentData()
                {
                    CollectionId = DecodeFromBase64(data.Rows[0][1].ToString()),
                    ObjectId = DecodeFromBase64(data.Rows[0][2].ToString()),
                    Data = DecodeFromBase64(data.Rows[0][3].ToString())
                };
            }
        }

        public void Update([NotNull] PersistentData persistentData)
        {
            if (persistentData == null) throw new ArgumentNullException(nameof(persistentData));

            using (var executor = new SqlExecutor(connectionString))
            {
                var result = executor.ExecQuery(string.Format(@"
SELECT 1 FROM PersistentData WHERE CollectionId = ""{0}"" AND ObjectId = ""{1}""",
EncodeAsBase64(persistentData.CollectionId),
EncodeAsBase64(persistentData.ObjectId)));

                if (result.Rows.Count == 0)
                {
                    CreateData(persistentData.CollectionId, persistentData.ObjectId, persistentData.Data);
                }
                else
                {
                    executor.ExecSql(string.Format(@"
UPDATE PersistentData SET Data = ""{2}"" WHERE CollectionId = ""{0}"" AND ObjectId = ""{1}""",
EncodeAsBase64(persistentData.CollectionId),
EncodeAsBase64(persistentData.ObjectId),
EncodeAsBase64(persistentData.Data)));
                }
            }
        }

        DataTable GetData(string collectionId, string objectId)
        {
            using (var executor = new SqlExecutor(connectionString))
            {
                return executor.ExecQuery(string.Format(@"
SELECT Id, CollectionId, ObjectId, Data FROM PersistentData WHERE CollectionId = ""{0}"" AND ObjectId = ""{1}""",
                    EncodeAsBase64(collectionId),
                    EncodeAsBase64(objectId)));
            }
        }

        void CreateData(string collectionId, string objectId, string data)
        {
            using (var executor = new SqlExecutor(connectionString))
            {
                executor.ExecSql(string.Format(@"
INSERT INTO PersistentData (CollectionId, ObjectId, Data) VALUES (""{0}"", ""{1}"", ""{2}"")"  ,
EncodeAsBase64(collectionId),
EncodeAsBase64(objectId),
EncodeAsBase64(data)));
            }
        }

        private string EncodeAsBase64(string source)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(source));
        }

        private string DecodeFromBase64(string base64String)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(base64String));
        }
    }
}
