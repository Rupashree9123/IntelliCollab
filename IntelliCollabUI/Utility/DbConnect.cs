using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Utility
{
    public class DbConnect
    {
        public static string ConnectionString;
        private SqlConnection  connection;
        internal SqlTransaction transaction;

        public DbConnect()
        {
            connection = new SqlConnection(ConnectionString);
            connection.Open();
        }

        internal DataSet GetDataFromDB(string sqlCommand, SqlParameter[] dbParams = null)
        {
            DataSet ds = null;
            try
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.CommandType = CommandType.Text;
                    if (dbParams != null && dbParams.Count() > 0)
                    {
                        cmd.Parameters.AddRange(dbParams);
                    }
                    ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    adapter.Dispose();
                    adapter = null;
                }
            }
            catch (SqlException oraEx)
            {
                throw oraEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        internal bool ExecuteNonQueryCommands(string sqlCommand, SqlParameter[] dbParams)
        {
            int retValue = -1;
            try
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.CommandType = CommandType.Text;
                    if (dbParams != null && dbParams.Count() > 0)
                    {
                        cmd.Parameters.AddRange(dbParams);
                    }
                    retValue = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException oraEx)
            {
                throw oraEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retValue > 0;
        }

        internal void InsertBulkDataUsingSqlBulkCopy(DataTable dataTable, string destinationTableName)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = destinationTableName; // Replace with your actual destination table name
                connection.Open();
                bulkCopy.WriteToServer(dataTable);
            }
        }
        public static DataTable ConvertListToDataTable<T>(List<T> dataList)
        {
            DataTable dataTable = new DataTable();

            // Get all properties of the 'YourDataClass' type
            var properties = typeof(T).GetProperties();

            // Create columns in the DataTable for each property
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            // Populate the DataTable with data from the List
            foreach (var item in dataList)
            {
                DataRow row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public void BeginLocalTransaction()
        {
            transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitLocalTransaction()
        {
            transaction.Commit();
        }

        public void RollbackLocalTransaction()
        {
            transaction.Rollback();
        }


        public void CloseConnection()
        {
            if (connection != null)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Dispose();
            }
            connection = null;
        }
    }
}