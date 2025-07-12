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
        [Description("The name of active alarms table.")]
        public string ActiveAlarmsTableName { get; set; } = "activealarms";

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

            // Setup tab events
            this.tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // Setup DataGridView columns
            SetupDataGridColumns();
        }

        private void IAlarmViewer_Load(object sender, EventArgs e)
        {
            this.connector = new AlarmViewMySQLConnector();
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

            // Chỉ tạo bảng nếu chưa có, không initialize dữ liệu
            CreateTablesIfNotExists();

            UpdateAlarmViewer();
            this.tmrUpdateView.Start();
        }

        private void CreateTablesIfNotExists()
        {
            try
            {
                if (this.connector.CreateDatabaseIfNotExists(this.databaseParametter))
                {
                    this.connector.CreateTableIfNotExists(this.databaseParametter);
                    this.connector.CreateActiveAlarmsTableIfNotExists(this.databaseParametter, ActiveAlarmsTableName);
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine($"Error creating tables: {ex.Message}");
            }
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

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update immediately when switching tabs
            UpdateAlarmViewer();
        }

        private void SetupDataGridColumns()
        {
            // Setup Active Alarm DataGridView
            SetupGridViewColumns(dgrvActiveAlarm);

            // Setup History Alarm DataGridView  
            SetupGridViewColumns(dgrvHistoryAlarm);
        }

        private void SetupGridViewColumns(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = true;
            dgv.Columns.Clear();

            // Add fixed columns
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateTime",
                HeaderText = "Time",
                DataPropertyName = "DateTime",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TagName",
                HeaderText = "Tag Name",
                DataPropertyName = "TagName",
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TagAlias",
                HeaderText = "Alias",
                DataPropertyName = "TagAlias",
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Value",
                HeaderText = "Value",
                DataPropertyName = "Value",
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HighLevel",
                HeaderText = "High Level",
                DataPropertyName = "HighLevel",
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LowLevel",
                HeaderText = "Low Level",
                DataPropertyName = "LowLevel",
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataPropertyName = "Status",
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Acknowledged",
                HeaderText = "ACK",
                DataPropertyName = "Acknowledged",
            });
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void UpdateAlarmViewer()
        {
            // Get selected tab index safely from UI thread
            int selectedTabIndex = 0;
            this.SynchronizedInvokeAction(() => {
                selectedTabIndex = tabControl.SelectedIndex;
            });

            if (selectedTabIndex == 0) // Active Alarm tab
            {
                UpdateActiveAlarms();
            }
            else // History Alarm tab
            {
                UpdateHistoryAlarms();
            }
        }

        private void UpdateActiveAlarms()
        {
            var dataTableAlarm = GetActiveAlarmsFromTable();

            if (dataTableAlarm == null)
                this.SynchronizedInvokeAction(() => this.dgrvActiveAlarm.Rows.Clear());
            else
                BindingActiveAlarmsToView(dataTableAlarm);
        }

        private void UpdateHistoryAlarms()
        {
            var dataTableAlarm = GetHistoryAlarms();

            if (dataTableAlarm == null)
                this.SynchronizedInvokeAction(() => this.dgrvHistoryAlarm.Rows.Clear());
            else
                BindingHistoryAlarmsToView(dataTableAlarm);
        }

        private DataTable GetActiveAlarmsFromTable()
        {
            DataTable dataTableAlarm = null;
            try
            {
                if (this.connector.CreateActiveAlarmsTableIfNotExists(this.databaseParametter, ActiveAlarmsTableName))
                {
                    // Chỉ đọc dữ liệu từ bảng active alarms (do AlarmLogger maintain)
                    dataTableAlarm = this.connector.GetActiveAlarmsFromTable(this.databaseParametter, ActiveAlarmsTableName, RowNumber);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting active alarms: {ex.Message}");
            }

            return dataTableAlarm;
        }

        private DataTable GetHistoryAlarms()
        {
            DataTable dataTableAlarm = null;
            if (this.connector.CreateDatabaseIfNotExists(this.databaseParametter))
                if (this.connector.CreateTableIfNotExists(this.databaseParametter))
                {
                    // Query for all alarms ordered by DateTime DESC
                    dataTableAlarm = this.connector.GetDataTableAlarm(this.databaseParametter, RowNumber);
                }

            return dataTableAlarm;
        }

        private void BindingActiveAlarmsToView(DataTable dataTableAlarm)
        {
            SuspendLayout();

            var indexOfFirstDisplayedScrollingRow = this.dgrvActiveAlarm.FirstDisplayedScrollingRowIndex;
            var lastCellRow = this.dgrvActiveAlarm.CurrentCell?.RowIndex ?? -1;
            var lastCellColumn = this.dgrvActiveAlarm.CurrentCell?.ColumnIndex ?? -1;

            this.dgrvActiveAlarm.SynchronizedInvokeAction(() =>
            {
                this.dgrvActiveAlarm.DataSource = dataTableAlarm;

                if (indexOfFirstDisplayedScrollingRow >= 0 && indexOfFirstDisplayedScrollingRow < this.dgrvActiveAlarm.Rows.Count)
                    this.dgrvActiveAlarm.FirstDisplayedScrollingRowIndex = indexOfFirstDisplayedScrollingRow;

                if (lastCellRow > -1 && lastCellColumn > -1 &&
                    lastCellRow < this.dgrvActiveAlarm.Rows.Count &&
                    lastCellColumn < this.dgrvActiveAlarm.Columns.Count)
                    this.dgrvActiveAlarm.CurrentCell = this.dgrvActiveAlarm[lastCellColumn, lastCellRow];

                // Color coding for Active Alarms - Simple logic based on ACK status only
                var countRow = dataTableAlarm.Rows.Count;
                for (int index = 0; index < countRow; index++)
                {
                    var row = this.dgrvActiveAlarm.Rows[index];
                    var ack = Convert.ToString(row.Cells["Acknowledged"].Value);

                    if (ack == "No") // Chưa ACK - Màu đỏ
                    {
                        row.DefaultCellStyle.ForeColor = Color.White;
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                    else // Đã ACK - Màu vàng
                    {
                        row.DefaultCellStyle.ForeColor = Color.Black;
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                }
            });

            ResumeLayout();
        }

        private void BindingHistoryAlarmsToView(DataTable dataTableAlarm)
        {
            SuspendLayout();

            var indexOfFirstDisplayedScrollingRow = this.dgrvHistoryAlarm.FirstDisplayedScrollingRowIndex;
            var lastCellRow = this.dgrvHistoryAlarm.CurrentCell?.RowIndex ?? -1;
            var lastCellColumn = this.dgrvHistoryAlarm.CurrentCell?.ColumnIndex ?? -1;

            this.dgrvHistoryAlarm.SynchronizedInvokeAction(() =>
            {
                this.dgrvHistoryAlarm.DataSource = dataTableAlarm;

                if (indexOfFirstDisplayedScrollingRow >= 0 && indexOfFirstDisplayedScrollingRow < this.dgrvHistoryAlarm.Rows.Count)
                    this.dgrvHistoryAlarm.FirstDisplayedScrollingRowIndex = indexOfFirstDisplayedScrollingRow;

                if (lastCellRow > -1 && lastCellColumn > -1 &&
                    lastCellRow < this.dgrvHistoryAlarm.Rows.Count &&
                    lastCellColumn < this.dgrvHistoryAlarm.Columns.Count)
                    this.dgrvHistoryAlarm.CurrentCell = this.dgrvHistoryAlarm[lastCellColumn, lastCellRow];

                // Color coding for History Alarms (giống như cũ)
                var countRow = dataTableAlarm.Rows.Count;
                for (int index = 0; index < countRow; index++)
                {
                    var row = this.dgrvHistoryAlarm.Rows[index];

                    var status = Convert.ToString(row.Cells["Status"].Value);
                    var ack = Convert.ToString(row.Cells["Acknowledged"].Value);

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