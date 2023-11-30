using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IOTApp
{
    /// <summary>
    /// Interaction logic for EmployeeSalesWindow.xaml. This window shows a simple sales
    /// report for the selected employee.
    /// </summary>
    public partial class EmployeeSalesWindow : Window
    {
        private Database _db;
        private Employee _employee;

        /// <summary>
        /// Initialise the database by creating and displaying a simple sales report for
        /// the selected employee. This includes total sales and a breakdown of sales by
        /// client.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="emp"></param>
        public EmployeeSalesWindow(Database db, Employee emp)
        {
            InitializeComponent();
            _db = db;
            _employee = emp;
            int id = _employee.Id;
            LabelEmployeeName.Content = $"{_employee}";

            // Get and display the employee's total sales.
            string whereClause = $"WHERE ww.employee_id = {id} ";
            List<SalesRecord> salesSum = _db.QueryEmployeeTotalSales(whereClause);
            int totalSales;
            if (salesSum.Count > 0)
            {
                totalSales = salesSum[0].TotalSales;
            }
            else
            {
                totalSales = 0;
            }
            string formattedTotalSales = $"${totalSales.ToString("N0")}";
            LabelTotalSales.Content = formattedTotalSales;

            // Get and display the employee's sales breakdown.
            whereClause = $"WHERE ww.employee_id = {id} ";
            List<SalesRecord> salesBreakdown = _db.QuerySalesByClient(whereClause);
            DataGridSalesBreakdown.ItemsSource = salesBreakdown;
        }
    }
}
