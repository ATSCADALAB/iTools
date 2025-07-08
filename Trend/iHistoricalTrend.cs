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
using ZedGraph;
using ATSCADA.iWinTools.Report;
using ATSCADA.ToolExtensions.PropertyEditor;

namespace ATSCADA.iWinTools.Trend
{
    public partial class iHistoricalTrend : UserControl
    {
        // DataBase Name
        protected string _Server_Name = "localhost";
        protected string _DataBase_Name = "ATSCADA";        
        protected string _User = "root";
        protected string _Password = "101101";
        protected string _Table_Name = "datalog";
        
        //String that contains config values in design mode
        protected string _SerializeString;

        protected System.Windows.Forms.Timer _SysTimer;

        protected bool _GridLine = false;

        protected Color _PanelColor = Color.Transparent;

        protected MySqlConnection _conn = new MySqlConnection();
        protected string _conn_str;
        protected MySqlDataAdapter _adp = new MySqlDataAdapter();
        protected DataSet _ds;
        
        //Trends collection
        protected List<HistoricalTimeStampList> _TSL = new List<HistoricalTimeStampList>();
        
        protected DateTime myFrom;
        protected DateTime myTo;

        [Description("The Name or IP of mySQL Database Server")]
        [Browsable(true), Category("ATSCADA Database")]
        public string mySQLServerName
        {
            get { return _Server_Name; }
            set { _Server_Name = value; }
        }
        [Description("The Name of mySQL Database")]
        [Browsable(true), Category("ATSCADA Database")]
        public string DatabaseName
        {
            get { return _DataBase_Name; }
            set { _DataBase_Name = value;}
        }
        [Description("Username for Login Authentication")]
        [Browsable(true), Category("ATSCADA Database")]
        public string User
        {
            get { return _User; }
            set { _User = value; }
        }
        [Description("Password for Login Authentication")]
        [Browsable(true), Category("ATSCADA Database")]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        [Description("The Name of Table that Data will be logged into")]
        [Browsable(true), Category("ATSCADA Database")]
        public string TableName
        {
            get { return _Table_Name; }
            set { _Table_Name = value; }
        }

        [Description("Add Tags for data querying")]
        [EditorAttribute(typeof(HistoricalTrendFormEditor),
            typeof(System.Drawing.Design.UITypeEditor)),
        Category("ATSCADA Settings")]
        public string AddTag
        {
            get { return _SerializeString; }
            set { _SerializeString = value; }
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
     
        //For setting in Property Grid
        [Description("Allow or not allow gridline view")]
        [Browsable(true), Category("ATSCADA Settings")]
        public bool GridLine
        {
            get { return _GridLine; }
            set
            {
                _GridLine = value;
                if (_GridLine)
                {
                    ZG1.GraphPane.XAxis.MajorGrid.IsVisible = true;
                    ZG1.GraphPane.YAxis.MajorGrid.IsVisible = true;
                }
                else
                {
                    ZG1.GraphPane.XAxis.MajorGrid.IsVisible = false;
                    ZG1.GraphPane.YAxis.MajorGrid.IsVisible = false;
                }
            }
        }
        
        [Description("Choose color for the trend panel")]
        [TypeConverter(typeof(Color)), CategoryAttribute("ATSCADA Settings")]
        public Color PanelColor
        {
            get { return _PanelColor; }
            set
            {
                _PanelColor = value;
                ZG1.GraphPane.Chart.Fill = new Fill(Color.White, _PanelColor, -45F);
            }
        }

        KnownColor[] allColors;
        private void Deserialize()
        {            
            try
            {
                //Calculate Color
                System.Array colorsArray = Enum.GetValues(typeof(KnownColor));
                allColors = new KnownColor[colorsArray.Length];
                Array.Copy(colorsArray, allColors, colorsArray.Length);

                //
                string[] SL = _SerializeString.Split('|');

                _TSL.Clear();

                comboBox1.Items.Clear();

                if (SL.Length > 1)
                {
                    if (SL[0].Split(',').Length > 1)
                    {
                        //Add to combobox for selection
                        comboBox1.Items.Add("All");
                        comboBox1.Text = "All";
                    }
                }

                foreach (string s in SL)
                {
                    string[] _s = s.Split(',');

                    HistoricalTimeStampList  t = new HistoricalTimeStampList();

                    //Add to combobox for selection
                    comboBox1.Items.Add(_s[0]);
                    if(SL.Length ==1)
                    {
                        comboBox1.Text = _s[0];  
                    }

                     t.ColumnName = _s[0];
                    
                    KnownColor c = allColors.ToList().Find(delegate(KnownColor _c) { return _c.ToString() == _s[1]; });
                    t.LineColor = Color.FromKnownColor(c);

                    c = allColors.ToList().Find(delegate(KnownColor _c) { return _c.ToString() == _s[2]; });
                    t.FillColor = Color.FromKnownColor(c);
                    t.TrendWidth = _s[3];
                    t.TrendType = _s[4];

                    _TSL.Add(t);
                }                
   
            }
            catch (Exception ex) { throw ex; }
        }

        public void Query()
        {                    
            try
            {                                
                _conn.ConnectionString = "server=" + _Server_Name + ";"
                    + "user id=" + _User + ";"
                    + "password=" + _Password + ";"
                    + "database=" + _DataBase_Name;

                _conn.Open();
                
                _ds = new DataSet();
                //connect to table
                _conn_str = "SELECT * FROM " + _Table_Name + " WHERE Datetime >= '" + myFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Datetime <= '" + myTo.ToString("yyyy-MM-dd HH:mm:ss") +"'";
                _adp.SelectCommand = new MySqlCommand(_conn_str, _conn);
                _adp.Fill(_ds, "dl");

                _conn.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void UpdateValue( HistoricalTimeStampList ht)
        {
            try
            {
                TimeStamp t;
                DataTable table = _ds.Tables["dl"];
                DataRow r;
                ht.TSList.Clear();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    r = table.Rows[i];
                    t = new TimeStamp();
                    t.Time = Convert.ToDateTime(r["Datetime"]);
                    t.Value = Convert.ToString(r[ht.ColumnName]);
                    ht.TSList.Add(t);
                }
            }
            catch (Exception ex) { throw ex; }
            
        }
        private void SetSize()
        {
            ZG1.Location = new Point(5, 140);
            // Leave a small margin around the outside of the control
            ZG1.Size = new Size(ClientRectangle.Width - 10, ClientRectangle.Height - 150);
        }

        public XDate ConvertDateToXdate(DateTime date)
        {
            return new XDate(date.ToOADate());
        }

        private void CreateGraph()
        {
            try
            {
                if (InvokeRequired)
                {
                    MethodInvoker method = new
                    MethodInvoker(CreateGraph);
                    Invoke(method);
                    return;
                }

                GraphPane myPane = ZG1.GraphPane;
                //Remove Previous Curves
                while (myPane.CurveList.Count > 0)
                    myPane.CurveList.RemoveAt(myPane.CurveList.Count - 1);

                for (int i = 0; i < _TSL.Count; i++)
                {
                    if (comboBox1.Text != "All")
                    {
                        if (_TSL[i].ColumnName != comboBox1.Text)
                            continue;
                    }

                    if (_TSL[i].TrendType == "Line")
                    {
                        PointPairList list = new PointPairList();
                        double x;
                        double y;
                        for (ushort j = 0; j < _TSL[i].TSList.Count; j++)
                        {
                            if (_TSL[i].TSList[j] != null)
                            {
                                if (_TSL[i].TSList[j].Value != "Bad")
                                {
                                    x = ConvertDateToXdate(_TSL[i].TSList[j].Time);
                                    y = Convert.ToDouble(_TSL[i].TSList[j].Value);
                                }
                                else
                                    continue; 
                            }
                            else
                            {
                                x = 0;
                                y = 0;
                            }
                            list.Add(x, y);
                        }
                        if (_TSL[i].TSList.Count > 0)
                        {
                            myPane.XAxis.Scale.Min = ConvertDateToXdate(_TSL[i].TSList[0].Time);
                            myPane.XAxis.Scale.Max = ConvertDateToXdate(_TSL[i].TSList[_TSL[i].TSList.Count -1 ].Time);
                        }
                        else
                        {
                            myPane.XAxis.Scale.Min = ConvertDateToXdate(DateTime.Now.AddMinutes(-1));
                            myPane.XAxis.Scale.Max = ConvertDateToXdate(DateTime.Now);
                        }
                        _TSL[i].ZedLine = myPane.AddCurve(_TSL[i].ColumnName, list, _TSL[i].LineColor, SymbolType.Circle);
                        _TSL[i].ZedLine.Line.Width = float.Parse(_TSL[i].TrendWidth); 
                        _TSL[i].ZedLine.Line.Fill = new Fill(Color.White, _TSL[i].FillColor, 45F);
                        // Make the symbols opaque by filling them with white
                        _TSL[i].ZedLine.Symbol.Fill = new Fill(Color.White);
                    }
                    else if (_TSL[i].TrendType == "Bar")
                    {
                        double[] xValues = new double[_TSL[i].TSList.Count];
                        double[] yValues = new double[_TSL[i].TSList.Count];
                        double x;
                        double y;
                        for (ushort j = 0; j < _TSL[i].TSList.Count; j++)
                        {
                            if (_TSL[i].TSList[j] != null)
                            {
                                x = ConvertDateToXdate(_TSL[i].TSList[j].Time);
                                y = Convert.ToDouble(_TSL[i].TSList[j].Value);
                            }
                            else
                            {
                                x = 0;
                                y = 0;
                            }
                            xValues[j] = x;
                            yValues[j] = y;
                        }

                        if (_TSL[i].TSList.Count > 0)
                        {
                            myPane.XAxis.Scale.Min = ConvertDateToXdate(_TSL[i].TSList[0].Time);
                            myPane.XAxis.Scale.Max = ConvertDateToXdate(_TSL[i].TSList[_TSL[i].TSList.Count - 1].Time);
                        }
                        else
                        {
                            myPane.XAxis.Scale.Min = ConvertDateToXdate(DateTime.Now.AddMinutes(-1));
                            myPane.XAxis.Scale.Max = ConvertDateToXdate(DateTime.Now);
                        }
                        _TSL[i].ZedBar = myPane.AddBar(_TSL[i].ColumnName, xValues, yValues, _TSL[i].LineColor);
                        _TSL[i].ZedBar.Bar.Fill = new Fill(_TSL[i].FillColor, Color.White, _TSL[i].FillColor);
                    }
                }
                
                ZG1.AxisChange();

            }
            catch (Exception ex) { throw ex; }
        }

        private void CreateGraphControl(object o, EventArgs e)
        {
            try
            {
                //Truy van luoc su
                TimeSpan t1 = new TimeSpan(dateTimePicker3.Value.Hour, dateTimePicker3.Value.Minute, dateTimePicker3.Value.Second);
                myFrom = dateTimePicker1.Value.Date + t1;
                TimeSpan t2 = new TimeSpan(dateTimePicker4.Value.Hour, dateTimePicker4.Value.Minute, dateTimePicker4.Value.Second);
                myTo = dateTimePicker2.Value.Date + t2;

                //Get value for each HistoricalTimeStampList
                Query();

                foreach (HistoricalTimeStampList h in _TSL)
                {                    
                    UpdateValue(h);
                }

                ZG1.Invalidate();
                ZG1.GraphPane.Chart.Fill = new Fill(Color.White, _PanelColor, -45F);
                if (_GridLine )
                {
                    ZG1.GraphPane.XAxis.MajorGrid.IsVisible = true;
                    ZG1.GraphPane.YAxis.MajorGrid.IsVisible = true;
                }
                else
                {
                    ZG1.GraphPane.XAxis.MajorGrid.IsVisible = false;
                    ZG1.GraphPane.YAxis.MajorGrid.IsVisible = false;
                }
                CreateGraph();
                SetSize();
                ZG1.BringToFront();
                ZG1.Refresh();
            }
            catch { }
        }

        int _TryCount = 0;
        private void _SysTimer_Tick(object sender, EventArgs e)
        {
            //Get Tag list
            try
            {
                Deserialize();

                GraphPane myPane = ZG1.GraphPane;
                // Set the titles and axis labels
                myPane.XAxis.Type = AxisType.Date;
                myPane.Title.Text = "Historical Trend";
                myPane.XAxis.Title.Text = "DateTime";
                myPane.YAxis.Title.Text = "Value";
                myPane.XAxis.Scale.Format = "dd/MM/yyyy \n HH:mm:ss";

                button1.Click += new EventHandler(CreateGraphControl);

                _SysTimer.Enabled = false;
                _SysTimer.Dispose();  
            }
            catch 
            {
                _TryCount++;
                if (_TryCount >= 10)
                {
                    _SysTimer.Enabled = false;
                    _SysTimer.Dispose();
                }
            }
        }

        public iHistoricalTrend()
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


            _SysTimer = new Timer();
            _SysTimer.Interval = 1000;
            _SysTimer.Tick +=new EventHandler(_SysTimer_Tick);
            _SysTimer.Enabled = true;  
        }

    }

    public class HistoricalTimeStampList
    {
        protected string _ColumnName;

        protected string _TrendType;
        protected string _TrendWidth;

        protected List<TimeStamp> _TSList = new List<TimeStamp>();

        protected Color _LineColor = Color.Blue;
        protected Color _FillColor = Color.Green;

     
        public string ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }

        public string TrendType
        {
            get { return _TrendType; }
            set { _TrendType = value; }
        }
        public string TrendWidth
        {
            get { return _TrendWidth; }
            set { _TrendWidth = value; }
        }

        //----------Line-------------
        protected LineItem _ZedLine;
        public LineItem ZedLine
        {
            get { return _ZedLine; }
            set { _ZedLine = value; }
        }
        public Color LineColor
        {
            get { return _LineColor; }
            set { _LineColor = value; }
        }
        public Color FillColor
        {
            get { return _FillColor; }
            set { _FillColor = value; }
        }

        //-------Bar----------
        protected BarItem _ZedBar;
        public BarItem ZedBar
        {
            get { return _ZedBar; }
            set { _ZedBar = value; }
        }

        //----------------------
        protected PieItem _ZedPie;
        public PieItem ZedPie
        {
            get { return _ZedPie; }
            set { _ZedPie = value; }
        }


        public List<TimeStamp> TSList
        {
            get { return _TSList; }
            set { _TSList = value; }
        }        
        
        public HistoricalTimeStampList() { }

    }

    public class HistoricalTrendFormEditor : PropertyEditorBase
    {

        private Historical_Trend_Settings myControl; //this is the control to be used in design time DropDown editor

        protected override System.Windows.Forms.Control GetEditControl(string PropertyName, Object CurrentValue)
        {
            myControl = new Historical_Trend_Settings();

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
