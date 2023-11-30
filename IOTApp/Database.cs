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
        private void Open()
        {
            _conn.Open();
        }

        /// <summary>
        /// Close the connection to the database.
        /// </summary>
        private void Close()
        {
            _conn.Close();
        }

        /// <summary>
        /// Queries the employees table of the database, using the given WHERE clause in
        /// the SQL query, and returns the results as a List of Employee objects.
        /// </summary>
        /// <param name="whereClause">The WHERE clause to insert into the SQL
        /// query. Table names must be abbreviated as follows: employees = e;
        /// branches = b.</param>
        /// <returns>The query result as a List of Employee objects.</returns>
        public List<Employee> QueryEmployees(string whereClause = "")
        {
            string sql = "SELECT e.id, e.given_name, e.family_name, e.date_of_birth, " +
                "e.gender_identity, e.gross_salary, e.branch_id, b.branch_name, " +
                "e.supervisor_id, CONCAT(s.given_name, ' ', s.family_name) AS supervisor_name " +
                "FROM employees AS e " +
                "LEFT JOIN branches AS b ON e.branch_id = b.id " +
                "LEFT JOIN employees AS s ON e.supervisor_id = s.id " +
                whereClause + " " +
                "ORDER BY e.family_name, e.given_name, e.id;";

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
                    int id = int.Parse(rdr["id"].ToString());
                    string givenName = rdr["given_name"].ToString();
                    string familyName = rdr["family_name"].ToString();
                    string gender = rdr["gender_identity"].ToString();
                    int salary = int.Parse(rdr["gross_salary"].ToString());

                    // Parse out the date portion of the date string.
                    string[] dateStrs;
                    dateStrs = rdr["date_of_birth"].ToString().Split(" ");
                    DateOnly dob = DateOnly.Parse(dateStrs[0]);

                    // Make sure the branch and supervisor are not null.
                    string branchIdStr = String.Empty;
                    int? branchId = null;
                    if (rdr["branch_id"] != null)
                    {
                        branchIdStr = rdr["branch_id"].ToString();
                        int.TryParse(branchIdStr, out int branchIdInt);
                        branchId = (int?)branchIdInt;
                    }
                    string branch = String.Empty;
                    if (rdr["branch_name"] != null)
                        branch = rdr["branch_name"].ToString();
                    string superIdStr = String.Empty;
                    int? supervisorId = null;
                    if (rdr["supervisor_id"] != null)
                    {
                        branchIdStr = rdr["supervisor_id"].ToString();
                        int.TryParse(superIdStr, out int superIdInt);
                        supervisorId = (int?)superIdInt;
                    }
                    string supervisor = String.Empty;
                    if (rdr["supervisor_name"] != null)
                        supervisor = rdr["supervisor_name"].ToString();

                    Employee emp = new(id, givenName, familyName, dob, gender, salary,
                        supervisorId, supervisor, branchId, branch);
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

        /// <summary>
        /// Query the branches table using the given SQL WHERE clause, if any.
        /// </summary>
        /// <param name="whereClause">The WHERE clause to append to the SQL
        /// query.</param>
        /// <returns></returns>
        public List<Branch> QueryBranches(string whereClause = "")
        {
            string sql = $"SELECT id, branch_name FROM branches {whereClause};";
            List<Branch> branches = new();
            try
            {
                Open();
                MySqlCommand cmd = new(sql, _conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    int id = int.Parse(rdr["id"].ToString());
                    string name = rdr["branch_name"].ToString();
                    Branch branch = new(id, name);
                    branches.Add(branch);
                }
                return branches;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return branches;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Query the Working With table to get a list of sales records, itemised by
        /// client and employee.
        /// </summary>
        /// <param name="whereClause">The WHERE clause to include in the SQL query.
        /// Table names should be abbreviated as follows: working_with = ww; 
        /// employees = e; clients = c.</param>
        /// <returns>A list of sales records itemised by client and employee.</returns>
        public List<SalesRecord> QuerySalesByClient(string whereClause = "")
        {
            string sql = "SELECT ww.employee_id, CONCAT(e.given_name, ' ', e.family_name) AS employee_name, " +
                "ww.client_id, c.client_name, ww.total_sales FROM working_with AS ww " +
                "LEFT JOIN employees AS e ON ww.employee_id = e.id " +
                "LEFT JOIN clients AS c ON ww.client_id = c.id " +
                whereClause + " " +
                "ORDER BY c.client_name;";
            List<SalesRecord> sales = new();
            try
            {
                Open();
                MySqlCommand cmd = new(sql, _conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string employeeIDStr = rdr["employee_id"].ToString();
                    int.TryParse(employeeIDStr, out int employeeID);
                    string employeeName = rdr["employee_name"].ToString();
                    string clientIDStr = rdr["client_id"].ToString();
                    int.TryParse(clientIDStr, out int clientID);
                    string clientName = rdr["client_name"].ToString();
                    string salesStr = rdr["total_sales"].ToString();
                    int.TryParse(salesStr, out int totalSales);

                    SalesRecord salesRec = new(clientID, clientName, employeeID,
                        employeeName, totalSales);
                    sales.Add(salesRec);
                }
                return sales;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return sales;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Gets a list of total sales across all clients, grouped by employee.
        /// </summary>
        /// <param name="whereClause">The WHERE clause to include in the SQL query - 
        /// optional. Table names should be abbreviated as follows: working_with = ww; 
        /// employees = e.</param>
        /// <returns>A list of sales records itemised by employee.</returns>
        public List<SalesRecord> QueryEmployeeTotalSales(string whereClause = "")
        {
            string sql = "SELECT e.id AS employee_id, CONCAT(e.given_name, ' ', e.family_name) AS employee_name, " +
                "SUM(ww.total_sales) AS total_sales FROM working_with AS ww " +
                "LEFT JOIN employees AS e ON ww.employee_id = e.id " +
                whereClause + " " +
                "GROUP BY ww.employee_id;";
            List<SalesRecord> sales = new();
            try
            {
                Open();
                MySqlCommand cmd = new(sql, _conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string employeeIdStr = rdr["employee_id"].ToString();
                    int.TryParse(employeeIdStr, out int employeeId);
                    string employeeName = rdr["employee_name"].ToString();
                    string salesStr = rdr["total_sales"].ToString();
                    int.TryParse(salesStr, out int totalSales);

                    SalesRecord salesRec = new(employeeId, employeeName, totalSales);
                    sales.Add(salesRec);
                }
                return sales;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return sales;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Execute the given SQL as a non-query (i.e. no results need to be returned).
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <returns>True if the SQL executed without issues, false if an
        /// exception occurred.</returns>
        public bool ExecuteNonQuery(string sql)
        {
            try
            {
                Open();
                MySqlCommand cmd = new(sql, _conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                Close();
            }
        }
    }
}
