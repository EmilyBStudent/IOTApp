using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using MySqlConnector;

namespace IOTApp
{
    /// <summary>
    /// Manages the database connection used by the IOT application.
    /// </summary>
    class Database
    {
        // Define the connection details.
        private string _dbName = "egb_ictprg431";
        private string _dbUser = "egb_ictprg431_user1";
        private string _dbPassword = "S3cur1ty!";
        private int _dbPort = 3306;
        private string _dbServer = "localhost";

        // Initialise the connection string and MySQL connection.
        private string _dbConnectionString = "";
        private MySqlConnection _conn;

        /// <summary>
        /// Create a connection to the database using the stored connection details.
        /// </summary>
        public Database()
        {
            _dbConnectionString = $"server={_dbServer}; user={_dbUser}; " +
                $"database={_dbName}; port={_dbPort}; password={_dbPassword}";
            _conn = new MySqlConnection(_dbConnectionString);
        }

        /// <summary>
        /// Open a connection to the database.
        /// </summary>
        public void Open()
        {
            _conn.Open();
        }

        /// <summary>
        /// Close the connection to the database.
        /// </summary>
        public void Close()
        {
            _conn.Close();
        }
    }
}
