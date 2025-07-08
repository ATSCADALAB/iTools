using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ATSCADA.ToolExtensions.PropertyEditor;
using ATSCADA.iWinTools.Database;
using System.Data.SqlClient;

namespace ATSCADA.iWinTools.Report
{
    public partial class iAlarmReporter : UserControl
    {
        Timer _SysTimer;

        // DataBase Name
        protected string _Server_Name = "localhost";
        protected string _DataBase_Name = "ATSCADA";
        protected string _User = "root";
        protected string _Password = "101101";
        protected string _Table_Name = "alarmlog";
        protected string _Tag_Name = "All";

        protected string _Title = "ALARM REPORT";
        protected string _Path = "C:\\";
        protected string _FileName = "AlarmReport";
        protected string _ExcelVer = "xlExcel12";

        //String that contains config values in design mode
        protected string _SerializeString;

        protected MySqlConnection _connMySQL = new MySqlConnection();
        protected string _conn_str;
        protected MySqlDataAdapter _adpMySQL = new MySqlDataAdapter();
        protected DataSet _ds;
        protected List<AlarmReportTag> _TS = new List<AlarmReportTag>();

        protected SqlConnection _connMSSQL = new SqlConnection();        
        protected SqlDataAdapter _adpMSSQL = new SqlDataAdapter();


        protected DateTime myFrom;
        protected DateTime myTo;

        [Description("Database type.")]
        [Browsable(true), Category("ATSCADA Database")]
        public DatabaseType DatabaseType { get; set; } = DatabaseType.MySQL;

        [Description("The name or IP of database server.")]
        [Browsable(true), Category("ATSCADA Database")]
        public string ServerName
        {
            get { return _Server_Name; }
            set { _Server_Name = value; }
        }
        [Description("The name of database.")]
        [Browsable(true), Category("ATSCADA Database")]
        public string DatabaseName
        {
            get { return _DataBase_Name; }
            set { _DataBase_Name = value; }
        }

        [Description("The name of table that data have been logged into.")]
        [Browsable(true), Category("ATSCADA Database")]
        public string TableName
        {
            get { return _Table_Name; }
            set
            {
                _Table_Name = value;
            }
        }

        [Description("Username for login authentication.")]
        [Browsable(true), Category("ATSCADA Database")]
        public string UserID
        {
            get { return _User; }
            set { _User = value; }
        }

        [Description("Password for login authentication.")]
        [Browsable(true), Category("ATSCADA Database")]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        [Description("The port of database server.")]
        [Browsable(true), Category("ATSCADA Database")]
        public uint Port { get; set; } = 3306;

        //For setting in Property Grid
        [Description("Add Tags for querying")]
        [EditorAttribute(typeof(AlarmReporterFormEditor),
            typeof(System.Drawing.Design.UITypeEditor)),
        Category("ATSCADA Settings")]
        public string AddTag
        {
            get { return _SerializeString; }
            set { _SerializeString = value; }
        }
        [Description("Title of the Excel report")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        [Description("Set the path for excel report Saving")]
        [Browsable(false), Category("ATSCADA Settings")]
        public string SavePath
        {
            get { return _Path; }
            set { _Path = value; }
        }

        [Description("Show save dialog")]
        [Browsable(true), Category("ATSCADA Settings")]
        public bool ShowSaveDialog { get; set; } = false;


       [Description("Select Excel Version")]
        [EditorAttribute(typeof(DataReporterListboxEditor2),
            typeof(System.Drawing.Design.UITypeEditor)),
        Category("ATSCADA Settings")]
        public string ExcelVersion
        {
            get { return _ExcelVer; }
            set { _ExcelVer = value; }
        }
        [Description("Set the Excel report name")]
        [Browsable(false), Category("ATSCADA Settings")]
        public string ReportName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
        [Description("From Date for querying")]
        [Browsable(false), Category("ATSCADA Settings")]
        public DateTime FromDate
        {
            get { return myFrom; }
            set { myFrom = value; }
        }
        [Description("To Date for querying")]
        [Browsable(false), Category("ATSCADA Settings")]
        public DateTime ToDate
        {
            get { return myTo; }
            set { myTo = value; }
        }

        public void Deserialize()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_SerializeString)) return;
                string[] ST = _SerializeString.Split('|');


                comboBox1.Items.Clear();
                //Create objects                
                comboBox1.Items.AddRange(ST);
                comboBox1.Items.Add("All");
                comboBox1.Text = "All";
                _Tag_Name = comboBox1.Text;
            }
            catch (Exception ex) { throw ex; }

        }

        public void Query()
        {
            try
            {
                if (DatabaseType == DatabaseType.MySQL)
                    QueryMySQL();
                else
                    QueryMSSQL();

            }
            catch (Exception ex) { throw ex; }
        }

        //-------- QUERY ------

        private void QueryMySQL()
        {
            _connMySQL.ConnectionString = "server=" + _Server_Name + $";Port={Port};" 
                    + "user id=" + _User + ";"
                    + "password=" + _Password + ";";

            _conn_str = "create database if not exists " + _DataBase_Name;
            _connMySQL.Open();
            _adpMySQL.SelectCommand = new MySqlCommand(_conn_str, _connMySQL);
            _adpMySQL.SelectCommand.ExecuteNonQuery();
            _connMySQL.Close();


            _connMySQL.ConnectionString = "server=" + _Server_Name + $";Port={Port};"
                + "user id=" + _User + ";"
                + "password=" + _Password + ";"
                + "database=" + _DataBase_Name;

            _connMySQL.Open();
            //create table if not exit
            _conn_str = "create table if not exists alarmlog (DateTime Datetime NOT NULL, TagName VARCHAR(100) NOT NULl, TagAlias VARCHAR(100) NOT NULl, Value VARCHAR(45) NOT NULl, HighLevel VARCHAR(45) NOT NULl, LowLevel VARCHAR(45) NOT NULl, Status VARCHAR(200) NOT NULl, Acknowledged VARCHAR(45) NOT NULl )";
            _adpMySQL.SelectCommand = new MySqlCommand(_conn_str, _connMySQL);
            _adpMySQL.SelectCommand.ExecuteNonQuery();

            _ds = new DataSet();
            //connect to table
            if (_Tag_Name != "All")
            {
                _conn_str = $"SELECT * FROM `{TableName}` WHERE Datetime >= '" + myFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Datetime <= '" + myTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND TagName = '" + _Tag_Name + "'";
            }
            else
                _conn_str = $"SELECT * FROM `{TableName}` WHERE Datetime >= '" + myFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Datetime <= '" + myTo.ToString("yyyy-MM-dd HH:mm:ss") + "'";

            _adpMySQL.SelectCommand = new MySqlCommand(_conn_str, _connMySQL);
            _adpMySQL.Fill(_ds, "al");

            _connMySQL.Close();
        }

        private void QueryMSSQL()
        {
            _connMSSQL.ConnectionString = $"Server={ServerName},{Port};User Id={UserID};Password={Password};";

            _conn_str = $"if not exists(select * from sys.databases where name = '{DatabaseName}') create database [{DatabaseName}]";
            _connMSSQL.Open();
            _adpMSSQL.SelectCommand = new SqlCommand(_conn_str, _connMSSQL);
            _adpMSSQL.SelectCommand.ExecuteNonQuery();
            _connMSSQL.Close();


            _connMSSQL.ConnectionString = $"Server={ServerName},{Port};User Id={UserID};Password={Password};Database={DatabaseName}";

            _connMSSQL.Open();
            //create table if not exit
            _conn_str = $"if not exists (select object_id from sys.tables where name = '{TableName}' and schema_name(schema_id) = 'dbo')" +
                    $"create table [{DatabaseName}].[dbo].[{TableName}]" +
                    $"([DateTime] Datetime not null, [TagName] varchar(100) not null, [TagAlias] varchar(100) not null, [Value] varchar(45) not null, [HighLevel] varchar(45) not null, [LowLevel] varchar(45) not null, [Status] varchar(200) not null, [Acknowledged] varchar(45) not null)";

            _adpMSSQL.SelectCommand = new SqlCommand(_conn_str, _connMSSQL);
            _adpMSSQL.SelectCommand.ExecuteNonQuery();

            _ds = new DataSet();
            //connect to table
            if (_Tag_Name != "All")
            {
                _conn_str = $"SELECT * FROM [{TableName}] WHERE [Datetime] >= '" + myFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND [Datetime] <= '" + myTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND [TagName] = '" + _Tag_Name + "'";
            }
            else
                _conn_str = $"SELECT * FROM [{TableName}] WHERE [Datetime] >= '" + myFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND [Datetime] <= '" + myTo.ToString("yyyy-MM-dd HH:mm:ss") + "'";

            _adpMSSQL.SelectCommand = new SqlCommand(_conn_str, _connMSSQL);
            _adpMSSQL.Fill(_ds, "al");

            _connMSSQL.Close();
        }


        // --------------------

        public void UpdateValue()
        {
            try
            {
                AlarmReportTag t;
                System.Data.DataTable table = _ds.Tables["al"];
                DataRow r;
                _TS.Clear();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    r = table.Rows[i];
                    t = new AlarmReportTag();
                    t.Time = Convert.ToDateTime(r["Datetime"]);
                    t.TagName = Convert.ToString(r["TagName"]);
                    t.TagAlias = Convert.ToString(r["TagAlias"]);
                    t.Value = Convert.ToString(r["Value"]);
                    t.HighValue = Convert.ToString(r["HighLevel"]);
                    t.LowValue = Convert.ToString(r["LowLevel"]);
                    t.Status = Convert.ToString(r["Status"]);
                    t.Ack = Convert.ToString(r["Acknowledged"]);

                    _TS.Add(t);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public void Export()
        {
            try
            {
                //Trien khai xuat ra excel
                if (_TS.Count > 0)
                {
                    ExcelDatalogger EExporter = new ExcelDatalogger();
                    EExporter.SetPath = _Path;
                    EExporter.ReportName = _FileName;
                    EExporter.ExcelVer = _ExcelVer;

                    EExporter.SetColRange = "H";
                    EExporter.SetRowRange = Convert.ToUInt32(_TS.Count());

                    EExporter.Title = _Title;

                    //Headers of columms
                    string[] Header = new string[8] { "DATETIME", "TAG NAME", "TAG ALIAS", "VALUE", "HIGH LEVEL", "LOW LEVEL", "STATUS", "ACKNOWLEDGED" };
                    //Value array
                    object[,] value = new object[Convert.ToUInt32(_TS.Count()), 8];

                    if (_TS.Count > 0)
                    {
                        for (int i = 0; i < _TS.Count; i++)
                        {
                            value[i, 0] = _TS[i].Time.ToString("dd/MM/yyyy - HH:mm:ss");
                            value[i, 1] = _TS[i].TagName;
                            value[i, 2] = _TS[i].TagAlias;
                            value[i, 3] = _TS[i].Value;
                            value[i, 4] = _TS[i].HighValue;
                            value[i, 5] = _TS[i].LowValue;
                            value[i, 6] = _TS[i].Status;
                            value[i, 7] = _TS[i].Ack;
                        }
                    }

                    EExporter.DataExport(Header, value);
                }
                else
                {
                    // MessageBox.Show("No Result", "ATSCADA");
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public void MakeReport()
        {
            try
            {
                Query();
                UpdateValue();
                Export();
            }
            catch (Exception ex) { throw ex; }
        }
        private void MakingReport(object o, EventArgs e)
        {
            try
            {
                //Check availability

                if (ShowSaveDialog)
                {
                    var saveFileDialog = new SaveFileDialog();  
                    saveFileDialog.Filter = "Excel|*.xls|Excel 2010|*.xlsx";
                    if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

                    var fileName = saveFileDialog.FileName;
                    _Path = System.IO.Path.GetDirectoryName(fileName) + "\\";
                    _FileName = Path.GetFileName(fileName);
                }
               
                
                if (File.Exists(_Path + _FileName + ".xls"))
                {
                    try
                    {
                        File.Delete(_Path + _FileName + ".xls");
                    }
                    catch { }
                }

                //Truy van luoc su
                TimeSpan t1 = new TimeSpan(dateTimePicker3.Value.Hour, dateTimePicker3.Value.Minute, dateTimePicker3.Value.Second);
                myFrom = dateTimePicker1.Value.Date + t1;
                TimeSpan t2 = new TimeSpan(dateTimePicker4.Value.Hour, dateTimePicker4.Value.Minute, dateTimePicker4.Value.Second);
                myTo = dateTimePicker2.Value.Date + t2;

                _Tag_Name = comboBox1.Text;

                MakeReport();

                try
                {
                    System.Diagnostics.Process.Start(_Path + _FileName + ".xls");
                }
                catch { }
            }
            catch { }
        }

        private void iAlarmReporter_ParentChanged(object o, EventArgs e)
        {
            try
            {
                _Path = "C:\\Program Files\\ATPro\\ATSCADA\\Reports\\";
                if (!Directory.Exists(_Path))
                {
                    _Path = "C:\\Program Files (x86)\\ATPro\\ATSCADA\\Reports\\";
                    if (!Directory.Exists(_Path))
                    {
                        _Path = "C:\\";
                    }
                }
                Deserialize();
                button1.Click += new EventHandler(MakingReport);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        public iAlarmReporter()
        {
            InitializeComponent();

            try
            {
                //not support design time
                if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv"
                || System.Diagnostics.Process.GetCurrentProcess().ProcessName == "VCSExpress"
                || System.Diagnostics.Process.GetCurrentProcess().ProcessName == "vbexpress"
                || System.Diagnostics.Process.GetCurrentProcess().ProcessName == "WDExpress")
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //this.ParentChanged +=new EventHandler(iAlarmReporter_ParentChanged);


            //In runtime

            _SysTimer = new System.Windows.Forms.Timer();
            _SysTimer.Interval = 1000;
            _SysTimer.Tick += _SysTimer_Tick;
            _SysTimer.Enabled = true;

        }

        void _SysTimer_Tick(object sender, EventArgs e)
        {

            try
            {
                _Path = "C:\\Program Files\\ATPro\\ATSCADA\\Reports\\";
                if (!Directory.Exists(_Path))
                {
                    _Path = "C:\\Program Files (x86)\\ATPro\\ATSCADA\\Reports\\";
                    if (!Directory.Exists(_Path))
                    {
                        _Path = "C:\\";
                    }
                }

                Deserialize();

                try
                {
                    button1.Click -= new EventHandler(MakingReport);
                }
                catch { }

                button1.Click += new EventHandler(MakingReport);

                _SysTimer.Enabled = false;
                _SysTimer.Dispose();

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

        }


    }
    public class AlarmReportTag
    {
        public DateTime Time;
        public string TagName;
        public string TagAlias;
        public string Value;
        public string HighValue;
        public string LowValue;
        public string Status;
        public string Ack;

    }

    public class AlarmReporterFormEditor : PropertyEditorBase
    {

        private frmAlarmReporterSettings myControl; //this is the control to be used in design time DropDown editor

        protected override System.Windows.Forms.Control GetEditControl(string PropertyName, Object CurrentValue)
        {
            myControl = new frmAlarmReporterSettings();

            string s = (string)CurrentValue;
            myControl.SerializeString = s;

            return myControl;
        }

        protected override Object GetEditedValue(System.Windows.Forms.Control EditControl, String PropertyName, Object OldValue)
        {
            if (myControl == null)
                return OldValue;

            if (myControl.IsCanceled)
                return OldValue;
            else
                return myControl.SerializeString;
        }

    }
}
