using ATSCADA.iWinTools.Database;
using ATSCADA.ToolExtensions.ExtensionMethods;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Alarm
{
    [ToolboxBitmap(typeof(System.Windows.Forms.DataGridView))]
    public partial class iAlarmViewer : UserControl
    {
        private readonly System.Timers.Timer tmrUpdateView;

        private IAlarmViewerConnector connector;

        private DatabaseParametter databaseParametter;

        [Category("ATSCADA Database")]
        [Description("Database type.")]
        public DatabaseType DatabaseType { get; set; } = DatabaseType.MySQL;

        [Category("ATSCADA Database")]
        [Description("The name or IP of database server.")]
        public string ServerName { get; set; } = "localhost";

        [Category("ATSCADA Database")]
        [Description("Username for login authentication.")]
        public string UserID { get; set; } = "root";

        [Category("ATSCADA Database")]
        [Description("Password for login authentication.")]
        public string Password { get; set; } = "101101";

        [Category("ATSCADA Database")]
        [Description("The name of database.")]
        public string DatabaseName { get; set; } = "ATSCADA";

        [Category("ATSCADA Database")]
        [Description("The name of table that data will be logged into.")]
        public string TableName { get; set; } = "alarmlog";

        [Category("ATSCADA Database")]
        [Description("The port of database server.")]
        public uint Port { get; set; } = 3306;

        [Category("ATSCADA Database")]
        [Description("Number of rows that appear onto gridview")]
        public uint RowNumber { get; set; } = 20;

        [Category("ATSCADA Database")]
        [Description("The rate (milisecond) for updating new status of alarm events")]

        public int UpdateRate { get; set; } = 4000;

        public iAlarmViewer()
        {
            InitializeComponent();

            try
            {
                var currentProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (currentProcessName == "devenv"
                    || currentProcessName == "VCSExpress"
                    || currentProcessName == "vbexpress"
                    || currentProcessName == "WDExpress") return;
            }
            catch { return; }           

            
            this.tmrUpdateView = new System.Timers.Timer();
            this.tmrUpdateView.AutoReset = false;
            this.tmrUpdateView.Elapsed += TmrUpdateView_Elapsed;

            this.Load += IAlarmViewer_Load;
            this.btnACK.Click += BtnACK_Click;
        }        

        private void IAlarmViewer_Load(object sender, EventArgs e)
        {
            this.connector = AlarmViewerConnectorFactory.GetConnector(DatabaseType);
            this.tmrUpdateView.Interval = UpdateRate;
            this.databaseParametter = new DatabaseParametter()
            {
                ServerName = ServerName,
                UserID = UserID,
                Password = Password,
                DatabaseName = DatabaseName,
                TableName = TableName,
                Port = Port,
            };

            UpdateAlarmViewer();
            this.tmrUpdateView.Start();           
        }

        private void TmrUpdateView_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.tmrUpdateView.Stop();
                UpdateAlarmViewer();
            }
            finally
            {
                this.tmrUpdateView.Start();
            }
        }

       
        private void BtnACK_Click(object sender, EventArgs e)
        {
            this.connector.Acknowledged(this.databaseParametter);
            UpdateAlarmViewer();
        }

        private void UpdateAlarmViewer()
        {
            var dataTableAlarm = GetDataTableAlarm();

            if (dataTableAlarm == null)
                this.SynchronizedInvokeAction(() => this.dgrvAlarmViewer.Rows.Clear());
            else
                BindingDataToView(dataTableAlarm);
        }

        private DataTable GetDataTableAlarm()
        {
            DataTable dataTableAlarm = null;
            if (this.connector.CreateDatabaseIfNotExists(this.databaseParametter))
                if (this.connector.CreateTableIfNotExists(this.databaseParametter))
                    dataTableAlarm = this.connector.GetDataTableAlarm(this.databaseParametter, RowNumber);

            return dataTableAlarm;
        }

        private void BindingDataToView(DataTable dataTableAlarm)
        {
            SuspendLayout();

            var indexOfFirstDisplayedScrollingRow = this.dgrvAlarmViewer.FirstDisplayedScrollingRowIndex;
            var lastCellRow = this.dgrvAlarmViewer.CurrentCell?.RowIndex ?? -1;
            var lastCellColumn = this.dgrvAlarmViewer.CurrentCell?.ColumnIndex ?? -1;

            this.dgrvAlarmViewer.SynchronizedInvokeAction(() =>
            {
                this.dgrvAlarmViewer.DataSource = dataTableAlarm;

                if (indexOfFirstDisplayedScrollingRow >= 0)
                    this.dgrvAlarmViewer.FirstDisplayedScrollingRowIndex = indexOfFirstDisplayedScrollingRow;

                if (lastCellRow > -1 && lastCellColumn > -1)
                    this.dgrvAlarmViewer.CurrentCell = this.dgrvAlarmViewer[lastCellColumn, lastCellRow];

                var countRow = dataTableAlarm.Rows.Count;
                for (int index = 0; index < countRow; index++)
                {
                    var row = this.dgrvAlarmViewer.Rows[index];

                    var status = Convert.ToString(row.Cells[6].Value);
                    var ack = Convert.ToString(row.Cells[7].Value);

                    if (status != "Normal" && ack == "No")
                    {
                        row.DefaultCellStyle.ForeColor = Color.White;
                        row.DefaultCellStyle.BackColor = Color.Red;

                    }                        
                    else if (status == "Normal" && ack == "No")
                    {
                        row.DefaultCellStyle.ForeColor = Color.White;
                        row.DefaultCellStyle.BackColor = Color.Blue;
                    }
                    else
                    {
                        row.DefaultCellStyle.ForeColor = Color.Black;                        
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }                        
                }
            });

            ResumeLayout();
        }
    }
}
