using ATSCADA.iWinTools.Alarm;
using ATSCADA.iWinTools.Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Logger
{
    public partial class frmAlarmLoggerSettings : Form
    {
        private const int EM_SETCUEBANNER = 0x1501;

        private const string EXAMPLE_EMAIL = "abc@gmail.com, def@xyz.com, ...";

        private IAlarmSettingsConnector connector;

        private DatabaseParametter databaseParametter;

        public bool IsCanceled { get; set; }

        public string DataSerialization { get; set; } = "";

        public frmAlarmLoggerSettings()
        {
            InitializeComponent();

            this.Load += FrmAlarmLoggerSetiings_Load;
            this.lstvAlarmLoggerSettings.SelectedIndexChanged += LstvAlarmLogger_SelectedIndexChanged;

            this.txtEmail.TextChanged += TxtEmail_TextChanged;

            btnAddUpdate.Click += BtnAddUpdate_Click;
            btnRemove.Click += BtnRemove_Click;

            btnUp.Click += BtnUp_Click;
            btnDown.Click += BtnDown_Click;

            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;

            SendMessage(this.txtEmail.Handle, EM_SETCUEBANNER, 0, EXAMPLE_EMAIL);
        }

        private void FrmAlarmLoggerSetiings_Load(object sender, EventArgs e)
        {
            SendMessage(this.txtEmail.Handle, EM_SETCUEBANNER, 0, EXAMPLE_EMAIL);

            var data = DataSerialization.Split('|');
            if (data.Length != 7) return;

            if (!Enum.TryParse(data[0], out DatabaseType databaseType)) return;
            if (!uint.TryParse(data[6], out uint port)) return;

            this.connector = AlarmSettingsConnectorFactory.GetConnector(databaseType);          
            this.databaseParametter = new DatabaseParametter()
            {
                ServerName = data[1],
                UserID = data[2],
                Password = data[3],
                DatabaseName = data[4],
                TableName = data[5],
                Port = port
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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private void LstvAlarmLogger_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstvAlarmLoggerSettings.SelectedItems.Count <= 0) return;

            cbxTracking.TagName = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[0].Text;
            txtAlias.Text = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[1].Text;
            cbxHighLevel.TagName = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[2].Text;
            cbxLowLevel.TagName = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[3].Text;
            txtEmail.Text = lstvAlarmLoggerSettings.SelectedItems[0].SubItems[4].Text;
        }

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

        private void BtnOK_Click(object sender, EventArgs e)
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
                            IsCanceled = false;
                            this.Hide();
                            return;
                        }

            this.tstContent.Text = "Connection to database failed!";
            this.tstContent.ForeColor = Color.Red;
            MessageBox.Show("Connection to database failed!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            IsCanceled = true;
            this.Hide();
        }
    }
}
