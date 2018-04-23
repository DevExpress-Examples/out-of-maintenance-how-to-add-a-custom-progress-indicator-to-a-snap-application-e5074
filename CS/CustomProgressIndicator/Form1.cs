using System;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using CustomProgressIndicator.nwindDataSetTableAdapters;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Services;
using DevExpress.Utils;
// ...

namespace CustomProgressIndicator {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private static object CreateDataSource() {
            var dataSource = new nwindDataSet();
            var connection = new OleDbConnection();
            connection.ConnectionString = Properties.Settings.Default.nwindConnectionString;

            CustomersTableAdapter customers = new CustomersTableAdapter();
            customers.Connection = connection;
            customers.Fill(dataSource.Customers);

            OrdersTableAdapter orders = new OrdersTableAdapter();
            orders.Connection = connection;
            orders.Fill(dataSource.Orders);

            var bindingSource = new BindingSource();
            bindingSource.DataSource = dataSource;
            bindingSource.DataMember = "Customers";
            return bindingSource;
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.snapControl1.DataSources.Add(string.Empty, CreateDataSource());

            this.snapControl1.Options.SnapMailMergeVisualOptions.DataSourceName = string.Empty;
            string filename = FilesHelper.FindingFileName(AppDomain.CurrentDomain.BaseDirectory,
                @"Data\template.snx", true);

            if (File.Exists(filename))
                this.snapControl1.Document.LoadDocument(filename, SnapDocumentFormat.Snap);
            this.snapControl1.ReplaceService<ISnapMailMergeProgressIndicationService>(
                new MyProgressIndicationService(this.snapControl1));
        }
    }
}
