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
using Microsoft.Office.Interop;
using System.Data.OleDb;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using ATSCADA.ToolExtensions.PropertyEditor;
using ATSCADA.iWinTools.Trend;
using System.Data.SqlClient;
using ATSCADA.iWinTools.Database;

namespace ATSCADA.iWinTools.Report
{
    public partial class iDataReporter : UserControl
    {

        Timer _SysTimer;
        // DataBase Name
        protected string _Server_Name = "localhost";
        protected string _DataBase_Name = "ATSCADA";
        protected string _User = "root";
        protected string _Password = "101101";
        protected string _Table_Name = "datalog";
        protected string _ChartType = "None";
        protected string _Title = "REPORT";

        //String that contains config values in design mode
        protected string _SerializeString;

        protected MySqlConnection _connMySQL = new MySqlConnection();
        protected MySqlDataAdapter _adpMySQL = new MySqlDataAdapter();

        protected SqlConnection _connMSSQL = new SqlConnection();
        protected SqlDataAdapter _adpMSSQL = new SqlDataAdapter();

        protected string _conn_str;
        protected DataSet _ds;
        protected List<TimeStamp> _TS = new List<TimeStamp>();
        protected List<string> _MyAlias = new List<string>();
        protected List<string> _MyBGColor = new List<string>();

        protected DateTime myFrom;
        protected DateTime myTo;

        protected string _Path = "C:\\";
        protected string _FileName = "Report";
        protected string _ExcelVer = "xlExcel12";

        [Browsable(true), Category("ATSCADA Database")]
        [Description("Database type.")]
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

        [Description("The name of table that data will be logged into.")]
        [Browsable(true), Category("ATSCADA Database")]
        public string TableName
        {
            get { return _Table_Name; }
            set { _Table_Name = value; }
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

        [Browsable(true), Category("ATSCADA Database")]
        [Description("The port of database server.")]
        public uint Port { get; set; } = 3306;

        [Description("Show save dialog")]
        [Browsable(true), Category("ATSCADA Settings")]
        public bool ShowSaveDialog { get; set; } = false;

        //For settings in Property Grid
        [Description("Report Design")]
        [EditorAttribute(typeof(DataReporterFormEditor),
            typeof(System.Drawing.Design.UITypeEditor)),
        Category("ATSCADA Settings")]
        public string ColumnSettings
        {
            get { return _SerializeString; }
            set { _SerializeString = value; }
        }

        [Description("Select Type of Chart")]
        [EditorAttribute(typeof(DataReporterListboxEditor1),
            typeof(System.Drawing.Design.UITypeEditor)),
        Category("ATSCADA Settings")]
        public string ChartType
        {
            get { return _ChartType; }
            set { _ChartType = value; }
        }
        [Description("Select Excel Version")]
        [EditorAttribute(typeof(DataReporterListboxEditor2),
            typeof(System.Drawing.Design.UITypeEditor)),
        Category("ATSCADA Settings")]
        public string ExcelVersion
        {
            get { return _ExcelVer; }
            set { _ExcelVer = value; }
        }
        [Description("Column Alias")]
        [Browsable(false), Category("ATSCADA Settings")]
        public List<string> ColumnAlias
        {
            get { return _MyAlias; }
            set { _MyAlias = value; }
        }

        [Description("BackGround Color")]
        [Browsable(false), Category("ATSCADA Settings")]
        public List<string> BGColor
        {
            get { return _MyBGColor; }
            set { _MyBGColor = value; }
        }
        [Description("Title of the Excel report")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
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

        [Description("Set the path for excel report Saving")]
        [Browsable(false), Category("ATSCADA Settings")]
        public string SavePath
        {
            get { return _Path; }
            set { _Path = value; }
        }

        [Description("Set the Excel report name")]
        [Browsable(false), Category("ATSCADA Settings")]
        public string ReportName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        public void Deserialize()
        {
            try
            {
                _MyAlias.Clear();
                _MyBGColor.Clear();

                string[] ST = _SerializeString.Split('|');

                foreach (string s in ST)
                {
                    _MyAlias.Add(s.Split(',')[0]);
                    _MyBGColor.Add(s.Split(',')[1]);
                }
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

                //Remove all bad row
                List<int> _RemoveRows = new List<int>();

                for (int i = 0; i < _ds.Tables["dl"].Rows.Count; i++)
                {
                    bool _NeedRemoveRow = false;
                    for (int j = 0; j < _ds.Tables["dl"].Columns.Count; j++)
                    {
                        if (Convert.ToString(_ds.Tables["dl"].Rows[i][j]) == "Bad")
                        {
                            _NeedRemoveRow = true;
                        }
                    }
                    if (_NeedRemoveRow == true)
                    {
                        _RemoveRows.Add(i);
                    }
                }

                for (int _myi = _RemoveRows.Count - 1; _myi >= 0; _myi--)
                {
                    _ds.Tables["dl"].Rows.RemoveAt(_RemoveRows[_myi]);
                }

                //Get needed columns 
                foreach (string s in _MyAlias)
                {
                    _ds.Tables["dl"].Columns[s].SetOrdinal(_MyAlias.IndexOf(s) + 1);
                }
                //Remove all unneed columns
                for (int i = _ds.Tables["dl"].Columns.Count - 1; i >= _MyAlias.Count + 1; i--)
                {
                    _ds.Tables["dl"].Columns.RemoveAt(i);
                }
            }
            catch { }
        }

        // --------------- QUERY ---------------

        public void QueryMySQL()
        {
            //Create DB & Table
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

            _ds = new DataSet(); //iDataLogSet();

            //connect to table
            _conn_str = "SELECT * FROM " + _Table_Name + " WHERE Datetime >= '" + myFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Datetime <= '" + myTo.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            _adpMySQL.SelectCommand = new MySqlCommand(_conn_str, _connMySQL);
            _adpMySQL.Fill(_ds, "dl");
            _connMySQL.Close();
           
        }

        public void QueryMSSQL()
        {
            //Create DB & Table
            _connMSSQL.ConnectionString = $"Server={ServerName},{Port};User Id={UserID};Password={Password};";

            _conn_str = $"if not exists(select * from sys.databases where name = '{DatabaseName}') create database [{DatabaseName}]";
            _connMSSQL.Open();
            _adpMSSQL.SelectCommand = new SqlCommand(_conn_str, _connMSSQL);
            _adpMSSQL.SelectCommand.ExecuteNonQuery();
            _connMSSQL.Close();

            _connMSSQL.ConnectionString = $"Server={ServerName},{Port};User Id={UserID};Password={Password};Database={DatabaseName}";

            _connMSSQL.Open();

            _ds = new DataSet(); //iDataLogSet();

            //connect to table
            _conn_str = "SELECT * FROM " + _Table_Name + " WHERE Datetime >= '" + myFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Datetime <= '" + myTo.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            _adpMSSQL.SelectCommand = new SqlCommand(_conn_str, _connMSSQL);
            _adpMSSQL.Fill(_ds, "dl");
            _connMSSQL.Close();           
        }



        // -------------------------------------

        public void Export()
        {
            try
            {
                mySQLExcel SQLEx = new mySQLExcel();
                SQLEx.SavePath = _Path;
                SQLEx.FileName = _FileName;
                SQLEx.DataSet = _ds;
                SQLEx.BGColor = _MyBGColor;
                SQLEx.ChartType = _ChartType;
                SQLEx.Title = _Title;
                SQLEx.DataTableName = "dl";
                SQLEx.ExcelVer = _ExcelVer;
                SQLEx.GetMaxCol();
                SQLEx.GetMaxRow();
                SQLEx.GetColLabel();
                SQLEx.Export();
            }
            catch { }
        }

        public void MakeReport()
        {
            try
            {
                Query();
                Export();
            }
            catch { }
        }

        private void MakingReport(object o, EventArgs e)
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

            //_Tag_Name = comboBox1.Text;  
            MakeReport();

            try
            {
                System.Diagnostics.Process.Start(_Path + _FileName + ".xls");
            }
            catch { }

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
            catch { }
        }

        public iDataReporter()
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


            //In runtime

            _SysTimer = new System.Windows.Forms.Timer();
            _SysTimer.Interval = 1000;
            _SysTimer.Tick += _SysTimer_Tick;
            _SysTimer.Enabled = true;
        }

    }

    public class mySQLExcel
    {
        protected DataSet _Ds;
        protected string _Dt;

        protected int _MaxCol;
        protected int _MaxRow;
        protected List<string> _ColLabel = new List<string>();
        protected string _Path;
        protected string _FileName = "Report";
        protected List<string> _MyBGColor = new List<string>();
        protected string _Title = "";
        protected string _ChartType = "None";
        protected string _ExcelVer = "";
        public string SavePath
        {
            get { return _Path; }
            set { _Path = value; }
        }
        public List<string> BGColor
        {
            get { return _MyBGColor; }
            set { _MyBGColor = value; }
        }
        public string ChartType
        {
            get { return _ChartType; }
            set { _ChartType = value; }
        }
        public string ExcelVer
        {
            get { return _ExcelVer; }
            set { _ExcelVer = value; }
        }
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
        public DataSet DataSet
        {
            get { return _Ds; }
            set { _Ds = value; }
        }
        public string DataTableName
        {
            get { return _Dt; }
            set { _Dt = value; }
        }
        public int MaxCol
        {
            get { return _MaxCol; }
            set { _MaxCol = value; }
        }
        public int MaxRow
        {
            get { return _MaxRow; }
            set { _MaxRow = value; }
        }

        public void GetMaxCol()
        {
            System.Data.DataTable table = _Ds.Tables[_Dt];
            foreach (DataColumn c in table.Columns)
            {
                _ColLabel.Add(c.ColumnName);
                _MaxCol++;
            }
        }
        public void GetColLabel()
        {
            System.Data.DataTable table = _Ds.Tables[_Dt];
            _ColLabel.Clear();
            foreach (DataColumn c in table.Columns)
            {
                _ColLabel.Add(c.ColumnName);
            }
        }
        public void GetMaxRow()
        {
            System.Data.DataTable table = _Ds.Tables[_Dt];
            _MaxRow = table.Rows.Count;
        }

        public void Export()
        {
            try
            {
                ExcelDatalogger EExporter = new ExcelDatalogger();

                EExporter.SetPath = _Path;
                EExporter.ReportName = _FileName;
                EExporter.ExcelVer = _ExcelVer;

                #region SetColRange
                if (_MaxCol <= 26)
                {
                    EExporter.SetColRange = ((char)(_MaxCol + 64)).ToString();
                }
                else if (_MaxCol > 26 && _MaxCol <= 52)
                {
                    EExporter.SetColRange = "A" + ((char)(_MaxCol - 26 + 64)).ToString();
                }
                else if (_MaxCol > 52 && _MaxCol <= 78)
                {
                    EExporter.SetColRange = "B" + ((char)(_MaxCol - 52 + 64)).ToString();
                }
                else if (_MaxCol > 78 && _MaxCol <= 104)
                {
                    EExporter.SetColRange = "C" + ((char)(_MaxCol - 78 + 64)).ToString();
                }
                else if (_MaxCol > 104 && _MaxCol <= 130)
                {
                    EExporter.SetColRange = "D" + ((char)(_MaxCol - 104 + 64)).ToString();
                }
                else if (_MaxCol > 130 && _MaxCol <= 156)
                {
                    EExporter.SetColRange = "E" + ((char)(_MaxCol - 130 + 64)).ToString();
                }
                else if (_MaxCol > 156 && _MaxCol <= 182)
                {
                    EExporter.SetColRange = "F" + ((char)(_MaxCol - 156 + 64)).ToString();
                }
                else if (_MaxCol > 182 && _MaxCol <= 208)
                {
                    EExporter.SetColRange = "G" + ((char)(_MaxCol - 182 + 64)).ToString();
                }
                else if (_MaxCol > 208 && _MaxCol <= 234)
                {
                    EExporter.SetColRange = "H" + ((char)(_MaxCol - 208 + 64)).ToString();
                }
                else if (_MaxCol > 234 && _MaxCol <= 260)
                {
                    EExporter.SetColRange = "I" + ((char)(_MaxCol - 234 + 64)).ToString();
                }
                else if (_MaxCol > 260 && _MaxCol <= 286)
                {
                    EExporter.SetColRange = "J" + ((char)(_MaxCol - 260 + 64)).ToString();
                }
                else if (_MaxCol > 286 && _MaxCol <= 312)
                {
                    EExporter.SetColRange = "K" + ((char)(_MaxCol - 286 + 64)).ToString();
                }
                else if (_MaxCol > 312 && _MaxCol <= 338)
                {
                    EExporter.SetColRange = "L" + ((char)(_MaxCol - 312 + 64)).ToString();
                }
                else if (_MaxCol > 338 && _MaxCol <= 364)
                {
                    EExporter.SetColRange = "M" + ((char)(_MaxCol - 338 + 64)).ToString();
                }
                else if (_MaxCol > 364 && _MaxCol <= 390)
                {
                    EExporter.SetColRange = "N" + ((char)(_MaxCol - 364 + 64)).ToString();
                }
                else if (_MaxCol > 390 && _MaxCol <= 416)
                {
                    EExporter.SetColRange = "O" + ((char)(_MaxCol - 390 + 64)).ToString();
                }
                else if (_MaxCol > 416 && _MaxCol <= 442)
                {
                    EExporter.SetColRange = "P" + ((char)(_MaxCol - 416 + 64)).ToString();
                }
                else if (_MaxCol > 442 && _MaxCol <= 468)
                {
                    EExporter.SetColRange = "Q" + ((char)(_MaxCol - 442 + 64)).ToString();
                }
                else if (_MaxCol > 468 && _MaxCol <= 494)
                {
                    EExporter.SetColRange = "R" + ((char)(_MaxCol - 468 + 64)).ToString();
                }
                else if (_MaxCol > 494 && _MaxCol <= 520)
                {
                    EExporter.SetColRange = "S" + ((char)(_MaxCol - 494 + 64)).ToString();
                }
                else if (_MaxCol > 520 && _MaxCol <= 546)
                {
                    EExporter.SetColRange = "T" + ((char)(_MaxCol - 520 + 64)).ToString();
                }
                else if (_MaxCol > 546 && _MaxCol <= 572)
                {
                    EExporter.SetColRange = "U" + ((char)(_MaxCol - 546 + 64)).ToString();
                }
                else if (_MaxCol > 572 && _MaxCol <= 598)
                {
                    EExporter.SetColRange = "V" + ((char)(_MaxCol - 572 + 64)).ToString();
                }
                else if (_MaxCol > 598 && _MaxCol <= 624)
                {
                    EExporter.SetColRange = "W" + ((char)(_MaxCol - 598 + 64)).ToString();
                }
                else if (_MaxCol > 624 && _MaxCol <= 650)
                {
                    EExporter.SetColRange = "X" + ((char)(_MaxCol - 624 + 64)).ToString();
                }
                else if (_MaxCol > 650 && _MaxCol <= 676)
                {
                    EExporter.SetColRange = "Y" + ((char)(_MaxCol - 650 + 64)).ToString();
                }
                else if (_MaxCol > 676 && _MaxCol <= 702)
                {
                    EExporter.SetColRange = "Z" + ((char)(_MaxCol - 676 + 64)).ToString();
                }
                if (_MaxCol > 702)
                {
                    _MaxCol = 702;
                    MessageBox.Show("ATSCADA do not support >702 colunms ", "ATSCADA");
                    return;
                }
                #endregion

                EExporter.SetRowRange = Convert.ToUInt32(_MaxRow);

                //Headers of columms
                string[] Header = _ColLabel.ToArray();
                //Value array
                object[,] value = new object[Convert.ToUInt32(_MaxRow), _MaxCol];

                for (int i = 0; i < _Ds.Tables[_Dt].Rows.Count; i++)
                {
                    DataRow r = _Ds.Tables[_Dt].Rows[i];

                    for (int j = 0; j < _Ds.Tables[_Dt].Columns.Count; j++)
                    {
                        if (j == 0)
                            value[i, j] = Convert.ToDateTime(r[j]).ToString("dd/MM/yyyy - HH:mm:ss");
                        else
                            value[i, j] = r[j].ToString().Replace(',', '.');
                    }
                }
                EExporter.Title = _Title;
                EExporter.BGColor = _MyBGColor;
                EExporter.ChartType = _ChartType;
                EExporter.DataExport(Header, value);
            }
            catch { }
        }
    }

    //For the base of report
    public class ExcelDatalogger
    {
        //File store path
        private string _Path = "C:\\";
        //Matrix dimension
        private ulong _mRow = 1;
        private string _mCol = "C";
        private string _ReportName = "Report";

        protected List<string> _MyBGColor = new List<string>();
        protected string _ChartType = "None";
        protected string _Title = "";
        protected string _ExcelVer = "";
        KnownColor[] allColors;

        //Excel objects
        private Excel.Application _objApp;
        private Excel.Workbooks _objBooks;
        private Excel.Workbook _objBook;
        private Excel.Sheets _objSheets;

        private Excel.Range _header;
        private Excel.Range _range;

        private ulong _iRow = 0;

        public List<string> BGColor
        {
            get { return _MyBGColor; }
            set { _MyBGColor = value; }
        }
        public string ChartType
        {
            get { return _ChartType; }
            set { _ChartType = value; }
        }
        public string ExcelVer
        {
            get { return _ExcelVer; }
            set { _ExcelVer = value; }
        }
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }
        public string SetPath
        {
            get { return _Path; }
            set { _Path = value; }
        }

        public ulong SetRowRange
        {
            get { return _mRow; }
            set { _mRow = value; }
        }

        public string SetColRange
        {
            get { return _mCol; }
            set { _mCol = value; }
        }
        public string ReportName
        {
            get { return _ReportName; }
            set { _ReportName = value; }
        }

        public bool DataExport(string[] ColLabels, object[,] values)
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("EXCEL"))
                {
                    proc.Kill();
                }

                //Check availability
                if (File.Exists(_Path + _ReportName + ".xls"))
                {
                    try
                    {
                        File.Delete(_Path + _ReportName + ".xls");
                    }
                    catch
                    {
                        return false;
                    }
                }

                // Create a new instance of Excel and start a new workbook.
                _objApp = new Excel.Application();
                _objBooks = _objApp.Workbooks;

                //New file
                _objBook = _objBooks.Add();
                _objSheets = _objBook.Worksheets;

                //Title
                //Inrease pointer
                _iRow = _iRow + 1; //_iRow=1                
                Excel.Range _TitleRange = _objSheets[1].Range["A" + _iRow.ToString(), _mCol.ToString() + _iRow.ToString()];
                _TitleRange.Merge();
                _TitleRange.HorizontalAlignment = Excel.Constants.xlCenter;
                _TitleRange.Font.Bold = true;
                _TitleRange.Borders.Color = Color.Black;

                _TitleRange.Font.Size = 25;
                _TitleRange.Font.Color = Color.Red;
                _TitleRange.Value = _Title;

                //HEADER
                //Inrease pointer
                _iRow = _iRow + 1; //_iRow=2                

                _header = _objSheets[1].Range["A" + _iRow.ToString(), _mCol.ToString() + _iRow.ToString()];
                _header.Select();
                _header.HorizontalAlignment = Excel.Constants.xlCenter;
                _header.Font.Bold = true;
                _header.Borders.Color = Color.Black;
                _header.Value = ColLabels;
                _header.Columns.AutoFit();

                //DATA
                _iRow = _iRow + 1;
                _range = _objSheets[1].Range["A" + _iRow.ToString(), _mCol + (_iRow + _mRow - 1).ToString()];
                _range.Select();
                _range.HorizontalAlignment = Excel.Constants.xlCenter;
                _range.Font.Bold = false;
                _range.Borders.Color = Color.Black;
                _range.Value = values;
                //_range.Columns.AutoFit();

                //Calculate Color
                System.Array colorsArray = Enum.GetValues(typeof(KnownColor));
                allColors = new KnownColor[colorsArray.Length];
                Array.Copy(colorsArray, allColors, colorsArray.Length);

                if (_MyBGColor.Count > 0)
                {
                    for (int i = 2; i <= ColLabels.Length; i++)
                    {
                        System.Drawing.Color c = Color.FromKnownColor(allColors.ToList().Find(delegate (KnownColor _c) { return _c.ToString() == _MyBGColor[i - 2]; }));

                        _header.Columns[i, Type.Missing].Interior.Color = System.Drawing.ColorTranslator.ToOle(c);
                        _range.Columns[i, Type.Missing].Interior.Color = System.Drawing.ColorTranslator.ToOle(c);

                    }
                }

                //Chart 
                if (_ChartType != "None")
                {
                    Excel.Range _Chartrange = _objSheets[1].Range["A2", _mCol + (_iRow + _mRow - 1).ToString()];
                    Excel.ChartObjects xlCharts = (Excel.ChartObjects)_objSheets[1].ChartObjects(Type.Missing);
                    Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
                    Excel.Chart chartPage = myChart.Chart;

                    chartPage.SetSourceData(_Chartrange, Type.Missing);

                    if (_ChartType == "Line")
                    {
                        chartPage.ChartType = Excel.XlChartType.xlLineMarkers;
                    }
                    else if (_ChartType == "Column")
                    {
                        chartPage.ChartType = Excel.XlChartType.xlColumnClustered;
                    }
                    else if (_ChartType == "Scatter")
                    {
                        chartPage.ChartType = Excel.XlChartType.xlXYScatter;
                    }

                    Excel.Axis xAxis = (Excel.Axis)chartPage.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
                    Excel.Axis yAxis = (Excel.Axis)chartPage.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);

                    yAxis.CrossesAt = yAxis.MinimumScale;

                    chartPage.Location(Excel.XlChartLocation.xlLocationAsNewSheet, Type.Missing);
                }

                if (_ExcelVer == "xlExcel12")
                    _objBook.SaveAs(_Path + _ReportName + ".xls",
                                    Excel.XlFileFormat.xlExcel12);
                else if (_ExcelVer == "xlExcel8")
                    _objBook.SaveAs(_Path + _ReportName + ".xls",
                                    Excel.XlFileFormat.xlExcel8);
                else if (_ExcelVer == "xlExcel9795")
                    _objBook.SaveAs(_Path + _ReportName + ".xls",
                                    Excel.XlFileFormat.xlExcel9795);
                else if (_ExcelVer == "xlOpenXMLWorkbook")
                    _objBook.SaveAs(_Path + _ReportName + ".xls",
                                    Excel.XlFileFormat.xlOpenXMLWorkbook);
                else if (_ExcelVer == "xlCSV")
                    _objBook.SaveAs(_Path + _ReportName + ".xls",
                                    Excel.XlFileFormat.xlCSV);

                _objBook.Close();
                _objApp.Quit();

                // release COM Objects
                releaseObject(_objSheets);
                releaseObject(_objBook);
                releaseObject(_objBooks);
                releaseObject(_objApp);

                return true;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); return false; }

        }
        static public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                throw new Exception("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        public void Close()
        {
            try
            {
                _objBook = null;
                _objBooks = null;
                _objApp = null;
            }
            catch { }
        }

    }



    public class DataReporterFormEditor : PropertyEditorBase
    {

        private frmDataReporterSettings myControl; //this is the control to be used in design time DropDown editor

        protected override System.Windows.Forms.Control GetEditControl(string PropertyName, Object CurrentValue)
        {
            myControl = new frmDataReporterSettings();

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
    // /////////////////////////////////////////////////////////////////////////
    //   myListBoxPropertyEditor => using a ListBox as EditControl
    // /////////////////////////////////////////////////////////////////////////
    //
    public class DataReporterListboxEditor1 : PropertyEditorBase
    {
        private ListBox myListBox;//this is the control to be used in design time DropDown editor

        protected override System.Windows.Forms.Control GetEditControl(string PropertyName, Object CurrentValue)
        {
            myListBox = new ListBox();
            myListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            //Creating ListBox items... 
            //Note that as this is executed in design mode, performance is not important and there is no need to cache these items if they can change each time.
            myListBox.Items.Clear();//clear previous items if any
            myListBox.Items.Add("None");
            myListBox.Items.Add("Line");
            myListBox.Items.Add("Column");
            myListBox.Items.Add("Scatter");
            myListBox.SelectedIndex = myListBox.FindString((string)CurrentValue);//Select current item based on CurrentValue of the property
            myListBox.Height = myListBox.PreferredHeight;
            myListBox.Click += new EventHandler(myListBox_Click);
            return myListBox;
        }

        protected override Object GetEditedValue(System.Windows.Forms.Control EditControl, string PropertyName, Object OldValue)
        {
            return myListBox.Text;//return new value for property
        }

        private void myListBox_Click(object sender, System.EventArgs e)
        {
            this.CloseDropDownWindow();//when user clicks on an item, the edit process is done.
        }

    }
    public class TimeStamp
    {
        protected DateTime _Time;
        protected String _Value;

        public DateTime Time
        {
            get
            { return _Time; }
            set
            { _Time = value; }
        }
        public String Value
        {
            get
            { return _Value; }
            set
            { _Value = value; }
        }

    }
    public class DataReporterListboxEditor2 : PropertyEditorBase
    {
        private ListBox myListBox;//this is the control to be used in design time DropDown editor

        protected override System.Windows.Forms.Control GetEditControl(string PropertyName, Object CurrentValue)
        {
            myListBox = new ListBox();
            myListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            //Creating ListBox items... 
            //Note that as this is executed in design mode, performance is not important and there is no need to cache these items if they can change each time.
            myListBox.Items.Clear();//clear previous items if any

            myListBox.Items.Add("xlExcel9795");
            myListBox.Items.Add("xlExcel8");
            myListBox.Items.Add("xlExcel12");
            myListBox.Items.Add("xlOpenXMLWorkbook");
            myListBox.Items.Add("xlCSV");
            myListBox.SelectedIndex = myListBox.FindString((string)CurrentValue);//Select current item based on CurrentValue of the property
            myListBox.Height = myListBox.PreferredHeight;
            myListBox.Click += new EventHandler(myListBox_Click);
            return myListBox;
        }

        protected override Object GetEditedValue(System.Windows.Forms.Control EditControl, string PropertyName, Object OldValue)
        {
            return myListBox.Text;//return new value for property
        }

        private void myListBox_Click(object sender, System.EventArgs e)
        {
            this.CloseDropDownWindow();//when user clicks on an item, the edit process is done.
        }

    }

}

