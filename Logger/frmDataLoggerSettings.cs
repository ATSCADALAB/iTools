using ATSCADA.ToolExtensions.TagCollection;
using System;
using System.IO;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Logger
{
    public partial class frmDataLoggerSettings : Form
    {
        public bool IsCanceled { get; set; }

        public string DataSerialization { get; set; } = "";

        public frmDataLoggerSettings()
        {
            InitializeComponent();

            Load += FrmAdderSettings_Load;
            lstvDataLoggerSettings.SelectedIndexChanged += LstvAdderSettings_SelectedIndexChanged;           

            btnAddUpdate.Click += BtnAddUpdate_Click;
            btnRemove.Click += BtnRemove_Click;

            btnUp.Click += BtnUp_Click;
            btnDown.Click += BtnDown_Click;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            InitToolTip();
        }

        private void InitToolTip()
        {
            var toolTip = new ToolTip()
            {
                ToolTipTitle = "Help",
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                AutoPopDelay = 10000,
                InitialDelay = 1000
            };

            toolTip.SetToolTip(cbxSelectedName, "Select the tag to log.");
            toolTip.SetToolTip(txtAlias, "Enter the alias of tag.");               
        }

        private void FrmAdderSettings_Load(object sender, EventArgs e)
        {             
            lstvDataLoggerSettings.Items.Clear();
            if (DataSerialization == "") return;

            var itemDataConverters = DataSerialization.Split('|');
            var countItems = itemDataConverters.Length;
            if (countItems == 0) return;

            for (int index = 0; index < countItems; index++)
            {
                var item = itemDataConverters[index].Split('&');
                if (item.Length != 3) continue;

                lstvDataLoggerSettings.Items.Add(new ListViewItem(item));
            }
        }

        private void LstvAdderSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstvDataLoggerSettings.SelectedItems.Count <= 0) return;

            cbxSelectedName.TagName = lstvDataLoggerSettings.SelectedItems[0].SubItems[0].Text;
            txtAlias.Text = lstvDataLoggerSettings.SelectedItems[0].SubItems[1].Text;
            if (bool.TryParse(lstvDataLoggerSettings.SelectedItems[0].SubItems[2].Text, out bool triggerParse))
                chkTrigger.Checked = triggerParse;
        }

      
        private void BtnAddUpdate_Click(object sender, EventArgs e)
        {
            var selectedName = cbxSelectedName.TagName.Trim();
            var alias = txtAlias.Text.Trim();
            var trigger = chkTrigger.Checked.ToString();       

            if (string.IsNullOrEmpty(selectedName) ||
                string.IsNullOrEmpty(alias) ||                
                string.IsNullOrEmpty(trigger)) return;

            if (selectedName.Contains("|") || selectedName.Contains("&") ||               
                trigger.Contains("|") || trigger.Contains("&")) return;

            foreach (ListViewItem listViewItem in lstvDataLoggerSettings.Items)
            {
                if (listViewItem.SubItems[1].Text == alias && listViewItem.SubItems[0].Text == selectedName)
                {
                    listViewItem.SubItems[2].Text = trigger;
                    return;
                }
                if (listViewItem.SubItems[1].Text == alias)
                {
                    MessageBox.Show("Alias is existed !", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (listViewItem.SubItems[0].Text == selectedName)                    
                {
                    listViewItem.SubItems[1].Text = alias;
                    listViewItem.SubItems[2].Text = trigger;

                    return;
                }
            }

            lstvDataLoggerSettings.Items.Add(new ListViewItem(new string[3]
            {
                selectedName,
                alias,
                trigger
            }));
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstvDataLoggerSettings.SelectedItems.Count <= 0) return;

            foreach (ListViewItem listViewItem in lstvDataLoggerSettings.SelectedItems)
                lstvDataLoggerSettings.Items.Remove(listViewItem);
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            if (lstvDataLoggerSettings.SelectedItems.Count <= 0) return;

            var listViewItem = lstvDataLoggerSettings.SelectedItems[0];
            var selectedIndex = listViewItem.Index;
            if (selectedIndex > 0)
            {
                lstvDataLoggerSettings.Items.RemoveAt(selectedIndex);
                lstvDataLoggerSettings.Items.Insert(selectedIndex - 1, listViewItem);
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            if (lstvDataLoggerSettings.SelectedItems.Count <= 0) return;

            var listViewItem = lstvDataLoggerSettings.SelectedItems[0];
            var selectedIndex = listViewItem.Index;
            if (selectedIndex < lstvDataLoggerSettings.Items.Count - 1)
            {
                lstvDataLoggerSettings.Items.RemoveAt(selectedIndex);
                lstvDataLoggerSettings.Items.Insert(selectedIndex + 1, listViewItem);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DataSerialization = "";
            ListViewItem li = lstvDataLoggerSettings.Items[0];
            DataSerialization = li.SubItems[0].Text + "&" + li.SubItems[1].Text + "&" + li.SubItems[2].Text;

            for (short i = 1; i < lstvDataLoggerSettings.Items.Count; i++)
            {
                DataSerialization = DataSerialization + "|" + lstvDataLoggerSettings.Items[i].SubItems[0].Text + "&" + lstvDataLoggerSettings.Items[i].SubItems[1].Text + "&" + lstvDataLoggerSettings.Items[i].SubItems[2].Text;
            }

            //DataSerialization = "";
            //foreach (ListViewItem listViewItem in lstvDataLoggerSettings.Items)
            //{
            //    var data = string.Format("|{0}&{1}&{2}",
            //        listViewItem.SubItems[0].Text,
            //        listViewItem.SubItems[1].Text,                    
            //        listViewItem.SubItems[2].Text);

            //    DataSerialization += data;
            //}
            //Write to file
            //Detect for SCADA folder
            string _FullPath = "C:\\Program Files\\ATPro\\ATSCADA\\DesignerFiles\\";
            if (!Directory.Exists(_FullPath))
            {
                _FullPath = "C:\\Program Files (x86)\\ATPro\\ATSCADA\\DesignerFiles\\";
                if (!Directory.Exists(_FullPath))
                {
                    MessageBox.Show("No ATSCADA\\DesignerFiles folder in this computer", "ATSCADA");
                    _FullPath = "C:\\";
                }
            }
            _FullPath = _FullPath + "DataLogTagAlias.txt";

            if (File.Exists(_FullPath))
            {
                File.Delete(_FullPath);
            }

            File.WriteAllText(_FullPath, DataSerialization);
            IsCanceled = false;
            this.Hide();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            IsCanceled = true;
            this.Hide();
        }

    }    

}
