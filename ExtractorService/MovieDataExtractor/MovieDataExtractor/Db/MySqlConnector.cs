using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Db
{
    class MySqlConnector
    {
        string connectionString;
        MySqlConnection connection;

        public MySqlConnector()
        {
            connectionString = ConfigurationManager.ConnectionStrings["oscarsdb"].ConnectionString;
            connection = new MySqlConnection(connectionString);
        }

        public void Open()
        {
            try
            {
                connection.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void DDL(string sqlQuery)
        {
            MySqlCommand command = new MySqlCommand(sqlQuery, connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("QUERY : " + sqlQuery);
            }
        }

        public int DMLCount(string sqlQuery)
        {
            MySqlCommand command = new MySqlCommand(sqlQuery, connection);
            try
            {
                var countString = command.ExecuteScalar();

                if (countString == null) return 0;
                else return Convert.ToInt32(countString);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("QUERY : " + sqlQuery);
            }

            return -1;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
