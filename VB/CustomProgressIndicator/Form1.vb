Imports System
Imports System.Data.OleDb
Imports System.IO
Imports System.Windows.Forms
Imports CustomProgressIndicator.nwindDataSetTableAdapters
Imports DevExpress.Snap.Core.API
Imports DevExpress.Snap.Core.Services
Imports DevExpress.Utils

' ...
Namespace CustomProgressIndicator

    Public Partial Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Shared Function CreateDataSource() As Object
            Dim dataSource = New nwindDataSet()
            Dim connection = New OleDbConnection()
            connection.ConnectionString = Properties.Settings.Default.nwindConnectionString
            Dim customers As CustomersTableAdapter = New CustomersTableAdapter()
            customers.Connection = connection
            customers.Fill(dataSource.Customers)
            Dim orders As OrdersTableAdapter = New OrdersTableAdapter()
            orders.Connection = connection
            orders.Fill(dataSource.Orders)
            Dim bindingSource = New BindingSource()
            bindingSource.DataSource = dataSource
            bindingSource.DataMember = "Customers"
            Return bindingSource
        End Function

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
            snapControl1.DataSources.Add(String.Empty, CreateDataSource())
            snapControl1.Options.SnapMailMergeVisualOptions.DataSourceName = String.Empty
            Dim filename As String = FilesHelper.FindingFileName(AppDomain.CurrentDomain.BaseDirectory, "Data\template.snx", True)
            If File.Exists(filename) Then snapControl1.Document.LoadDocument(filename, SnapDocumentFormat.Snap)
            snapControl1.ReplaceService(Of ISnapMailMergeProgressIndicationService)(New MyProgressIndicationService(snapControl1))
        End Sub
    End Class
End Namespace
