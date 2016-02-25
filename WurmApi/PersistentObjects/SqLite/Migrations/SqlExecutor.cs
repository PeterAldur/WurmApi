using System;
using System.Data;
using System.Data.SQLite;

namespace AldursLab.WurmApi.PersistentObjects.SqLite.Migrations
{
    public class SqlExecutor : IDisposable
    {
        readonly SQLiteConnection connection;
        SQLiteTransaction transaction;

        public SqlExecutor(string connectionString)
        {
            connection = new SQLiteConnection(connectionString);
            connection.Open();
        }

        public void ExecSql(string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        public DataTable ExecQuery(string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sql;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    DataTable result = new DataTable();
                    result.Load(reader);
                    return result;
                }
            }
        }

        public void BeginTran()
        {
            transaction = connection.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}