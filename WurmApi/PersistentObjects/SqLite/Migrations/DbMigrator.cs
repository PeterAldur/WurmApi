using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi.PersistentObjects.SqLite.Model;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.PersistentObjects.SqLite.Migrations
{
    public class DbMigrator
    {
        const int SchemaVersion = 1;

        readonly string connectionString;

        public DbMigrator([NotNull] string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            this.connectionString = connectionString;
        }

        public void Migrate()
        {
            using (var db = new SqlExecutor(connectionString))
            {
                db.ExecSql(@"
CREATE TABLE IF NOT EXISTS SchemaInfo
(
  Key TEXT,
  Value TEXT
)
");
                var schemainfo = db.ExecQuery(@"SELECT Key, Value FROM SchemaInfo");
                if (schemainfo.Rows.Count == 0)
                {
                    db.ExecSql(@"INSERT INTO SchemaInfo VALUES ('VERSION', '1')");
                }

                db.ExecQuery(@"
CREATE TABLE IF NOT EXISTS PersistentData
(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  CollectionId TEXT,
  ObjectId TEXT,
  Data TEXT
)
");
            }
        }
    }
}
