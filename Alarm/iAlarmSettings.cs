using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ATSCADA.iWinTools.Logger;
using ATSCADA.iWinTools.Database;

namespace ATSCADA.iWinTools.Alarm
{
    public partial class iAlarmSettings : UserControl
    {
        private const int EM_SETCUEBANNER = 0x1501;

        private const string EXAMPLE_EMAIL = "abc@gmail.com, def@xyz.com, ...";

        private AlarmSettingsConnector connector;

        private DatabaseParametter databaseParametter;

        [Category("ATSCADA Database")]
        [Description("The name or IP of database server.")]
        public string ServerName { get; set; } = "localhost";

        [Category("ATSCADA Database")]
        [Description("Username for login authentication.")]
        public string UserID { get; set; } = "root";

        [Category("ATSCADA Database")]
        [Description("Password for login authentication.")]
        public string Password { get; set; } = "100100";

        [Category("ATSCADA Database")]
        [Description("The name of database.")]
        public string DatabaseName { get; set; } = "ATSCADA";

        [Category("ATSCADA Database")]
        [Description("The name of table that data will be logged into.")]
        public string TableName { get; set; } = "alarmsettings";

        public iAlarmSettings()
        {
            InitializeComponent();
            SendMessage(this.txtEmail.Handle, EM_SETCUEBANNER, 0, EXAMPLE_EMAIL);
            try
            {
                var currentProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (currentProcessName == "devenv"
                    || currentProcessName == "VCSExpress"
                    || currentProcessName == "vbexpress"
                    || currentProcessName == "WDExpress") return;
            }
            catch { return; }

            this.Load += IAlarmSettings_Load;
            this.lstvAlarmLoggerSettings.SelectedIndexChanged += LstvAlarmLoggerSettings_SelectedIndexChanged;

            this.txtEmail.TextChanged += TxtEmail_TextChanged;

            this.btnAddUpdate.Click += BtnAddUpdate_Click;
            this.btnRemove.Click += BtnRemove_Click;

            this.btnUp.Click += BtnUp_Click;
            this.btnDown.Click += BtnDown_Click;

            this.btnApply.Click += BtnApply_Click;
        }
       
        private void IAlarmSettings_Load(object sender, EventArgs e)
        {
            SendMessage(this.txtEmail.Handle, EM_SETCUEBANNER, 0, EXAMPLE_EMAIL);

            this.connector = new AlarmSettingsConnector();            
            this.databaseParametter = new DatabaseParametter()
            {
                ServerName = this.ServerName,
                UserID = this.UserID,
                Password = this.Password,
                DatabaseName = this.DatabaseName,
                TableName = this.TableName
            };

            List<AlarmSettingsItem> alarmSettingsItems = null;
            if (this.connector.CreateDatabaseIfNotExists(this.databaseParametter))
                if (this.connector.CreateTableIfNotExists(this.databaseParametter))
                    alarmSettingsItems = this.connector.GetAlarmSettingsItems(this.databaseParametter);
            

            if (alarmSettingsItems == null)
            {
                this.tstContent.Text = "Connection to database failed!";
                this.tstContent.ForeColor = Color.Red;
                return;
            }


            foreach (var alarmSettingsItem in alarmSettingsItems)
            {
                var item = new string[5];

                item[0] = alarmSettingsItem.AlarmParametter.Tracking;
                item[1] = alarmSettingsItem.AlarmParametter.Alias;
                item[2] = alarmSettingsItem.AlarmParametter.HighLevel;
                item[3] = alarmSettingsItem.AlarmParametter.LowLevel;
                item[4] = alarmSettingsItem.Recipients;

                lstvAlarmLoggerSettings.Items.Add(new ListViewItem(item));
            }
        }

        private void LstvAlarmLoggerSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstvAlarmLoggerSettings.SelectedItems.Count <= 0) return;

            cbxTracking.TagName = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[0].Text;
            txtAlias.Text = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[1].Text;
            cbxHighLevel.TagName = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[2].Text;
            cbxLowLevel.TagName = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[3].Text;
            txtEmail.Text = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[4].Text;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private void TxtEmail_TextChanged(object sender, EventArgs e)
        {
            var text = this.txtEmail.Text;
            if (text.Contains(Environment.NewLine))
                this.txtEmail.Text = text.Replace(Environment.NewLine, "");
        }

        private void BtnAddUpdate_Click(object sender, EventArgs e)
        {
            var tracking = cbxTracking.TagName.Trim();
            var alias = txtAlias.Text.Trim();
            var lowLevel = cbxLowLevel.TagName.Trim();
            var highLevel = cbxHighLevel.TagName.Trim();
            var email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(tracking) ||
                string.IsNullOrEmpty(alias) ||
                string.IsNullOrEmpty(lowLevel) ||
                string.IsNullOrEmpty(highLevel)) return;

            if (tracking.Contains("|") || tracking.Contains("&") ||
                alias.Contains("|") || alias.Contains("&") ||
                lowLevel.Contains("|") || lowLevel.Contains("&") ||
                highLevel.Contains("|") || highLevel.Contains("&")) return;

            foreach (ListViewItem listViewItem in lstvAlarmLoggerSettings.Items)
            {
                if (listViewItem.SubItems[0].Text == tracking)
                {
                    listViewItem.SubItems[1].Text = alias;
                    listViewItem.SubItems[2].Text = highLevel;
                    listViewItem.SubItems[3].Text = lowLevel;
                    listViewItem.SubItems[4].Text = email;

                    return;
                }
            }

            lstvAlarmLoggerSettings.Items.Add(new ListViewItem(new string[5]
            {
                tracking,
                alias,
                highLevel,
                lowLevel,
                email
            }));
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstvAlarmLoggerSettings.SelectedItems.Count <= 0) return;

            foreach (ListViewItem listViewItem in lstvAlarmLoggerSettings.SelectedItems)
                lstvAlarmLoggerSettings.Items.Remove(listViewItem);
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            if (lstvAlarmLoggerSettings.SelectedItems.Count <= 0) return;

            var listViewItem = lstvAlarmLoggerSettings.SelectedItems[0];
            var selectedIndex = listViewItem.Index;
            if (selectedIndex > 0)
            {
                lstvAlarmLoggerSettings.Items.RemoveAt(selectedIndex);
                lstvAlarmLoggerSettings.Items.Insert(selectedIndex - 1, listViewItem);
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            if (lstvAlarmLoggerSettings.SelectedItems.Count <= 0) return;

            var listViewItem = lstvAlarmLoggerSettings.SelectedItems[0];
            var selectedIndex = listViewItem.Index;
            if (selectedIndex < lstvAlarmLoggerSettings.Items.Count - 1)
            {
                lstvAlarmLoggerSettings.Items.RemoveAt(selectedIndex);
                lstvAlarmLoggerSettings.Items.Insert(selectedIndex + 1, listViewItem);
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            var alarmSettingsItems = new List<AlarmSettingsItem>();
            foreach (ListViewItem listViewItem in lstvAlarmLoggerSettings.Items)
            {
                var tracking = listViewItem.SubItems[0].Text;
                var alias = listViewItem.SubItems[1].Text;
                var highLevel = listViewItem.SubItems[2].Text;
                var lowLevel = listViewItem.SubItems[3].Text;
                var email = listViewItem.SubItems[4].Text;

                alarmSettingsItems.Add(new AlarmSettingsItem()
                {
                    AlarmParametter = new AlarmParametter()
                    {
                        Tracking = tracking,
                        Alias = alias,
                        HighLevel = highLevel,
                        LowLevel = lowLevel
                    },
                    Recipients = email
                });
            }

            if (this.connector.CreateDatabaseIfNotExists(this.databaseParametter))
                if (this.connector.CreateTableIfNotExists(this.databaseParametter))
                    if (this.connector.TruncateTableSettings(this.databaseParametter))
                        if (this.connector.UpdateTableSettings(this.databaseParametter, alarmSettingsItems))
                        {

                            this.tstContent.Text = "Update success!  Please restart the application for affecting.";
                            this.tstContent.ForeColor = Color.Green;
                            MessageBox.Show("Update successful!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
            
            this.tstContent.Text = "Connection to database failed!";
            this.tstContent.ForeColor = Color.Red;
            MessageBox.Show("Connection to database failed!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
