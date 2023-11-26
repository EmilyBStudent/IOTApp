﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic;
using MySqlConnector;

namespace IOTApp
{
    /// <summary>
    /// Manages the database connection used by the IOT application.
    /// </summary>
    public class Database
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

        /// <summary>
        /// Queries the employees table of the database with the given SQL query string
        /// and returns the results as a List of Employee objects.
        /// </summary>
        /// <param name="sql">The SQL query to run. Must return a list of
        /// employees from the employees table.</param>
        /// <returns>The query result as a List of Employee objects.</returns>
        public List<Employee> QueryEmployees(string sql)
        {
            List<Employee> employees = new();
            try
            {
                Open();
                MySqlCommand cmd = new(sql, _conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    // Read in the values from each field and parse as the appropriate
                    // type.
                    int id = int.Parse(rdr[0].ToString());
                    string givenName = rdr[1].ToString();
                    string familyName = rdr[2].ToString();
                    string gender = rdr[4].ToString();
                    int salary = int.Parse(rdr[5].ToString());

                    // Parse out the date portion of the date string.
                    string[] dateStrs;
                    dateStrs = rdr[3].ToString().Split(" ");
                    DateOnly dob = DateOnly.Parse(dateStrs[0]);

                    // Make sure the branch and supervisor are not null.
                    string branch = String.Empty;
                    if (rdr[6] != null)
                        branch = rdr[6].ToString();
                    string supervisor = String.Empty;
                    if (rdr[7] != null)
                        supervisor = rdr[7].ToString();

                    Employee emp = new(id, givenName, familyName, dob, gender, salary,
                        supervisor, branch);
                    employees.Add(emp);
                }
                return employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return employees;
            }
            finally
            {
                Close();
            }
        }
    }
}
