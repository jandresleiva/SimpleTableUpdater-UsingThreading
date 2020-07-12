using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Update_Clients_CUIL.Data
{
    public class DBConnection
    {
        protected string connectionString;
        public DBConnection(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        //private static DBConnection _instance = null;
        //public static DBConnection Instance()
        //{
        //    if (_instance == null)
        //        _instance = new DBConnection();
        //    return _instance;
        //}

        public bool IsConnect(int dbase = 0)
        {
            if (Connection == null)
            {
                string connstring;
                connstring = string.Format(connectionString);

                connection = new MySqlConnection(connstring);
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return true;
        }

        public void Close()
        {
            connection.Close();
            connection = null;
        }
    }
}
