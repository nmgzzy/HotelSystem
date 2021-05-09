using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;


namespace HotelSystem
{
    public class SqlManage
    {
        private SQLiteConnection m_dbConnection;
        private string dbName;

        public SqlManage(string name)
        {
            dbName = name;
        }
        //public void Test(string tableName, string colume, string value)
        //{
        //    CreateNewDatabase();
        //    ConnectToDatabase();
        //    CreateTable(tableName, colume);
        //    Insert(tableName, colume, value);
        //    m_dbConnection.GetSchema();
        //}
        public void CreateNewDatabase()
        {
            SQLiteConnection.CreateFile(dbName);
        }
        public void ConnectToDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source="+ dbName + ";Version=3;");
            m_dbConnection.Open();
        }
        public void DisonnectToDatabase()
        {
            m_dbConnection.Close();
        }
        public void CreateTable(string tableName, string colume)
        {
            string sql = "create table " + tableName + " (" + colume + ")";
            Command(sql);
        }
        public void Insert(string tableName, string colume, string value)
        {
            string sql = "insert into " + tableName + " (" + colume + ") values (" + value + ")";
            Command(sql);
        }
        public SQLiteDataReader Select(string colume, string tableName, string condition)
        {
            string sql = "select " + colume + " from " + tableName + " " + condition;
            return Query(sql);
        }
        public void Delete(string tableName, string condition)
        {
            string sql = "delete from " + tableName + " where " + condition;
            Command(sql);
        }
        public void Update(string tableName, string columeValue, string condition)
        {
            string sql = "update " + tableName + " set " + columeValue + " where " + condition;
            Command(sql);
        }
        public void Command(string sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }
        public DataTable QueryTable(string sql)
        {
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, m_dbConnection);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }
        public SQLiteDataReader Query(string sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }
    }
}
