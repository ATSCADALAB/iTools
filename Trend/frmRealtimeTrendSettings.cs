using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Trend
{
    public partial class frmRealtimeTrendSettings : Form
    {
        public bool IsCanceled { get; set; }

        public string DataSerialization { get; set; } = "";

        public frmRealtimeTrendSettings()
        {
            InitializeComponent();

            DrawItemColor(cbxFillColor);
            DrawItemColor(cbxLineColor);            

            this.Load += FrmRealtimeTrendSettings_Load;
            lstvReatimeTrendSettings.SelectedIndexChanged += LstvReatimeTrendSettings_SelectedIndexChanged;

            btnAddUpdate.Click += BtnAddUpdate_Click;
            btnRemove.Click += BtnRemove_Click;

            btnUp.Click += BtnUp_Click;
            btnDown.Click += BtnDown_Click;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

        }        

        private void FrmRealtimeTrendSettings_Load(object sender, EventArgs e)
        {
            LoadColor(cbxFillColor);
            LoadColor(cbxLineColor);

            cbxType.Text = cbxType.Items[0].ToString();
            cbxLineWidth.Text = cbxLineWidth.Items[0].ToString();

            lstvReatimeTrendSettings.Items.Clear();
            if (DataSerialization == "") return;

            var itemDataConverters = DataSerialization.Split('|');
            var countItems = itemDataConverters.Length;
            if (countItems == 0) return;

            for (int index = 0; index < countItems; index++)
            {
                var item = itemDataConverters[index].Split('&');
                if (item.Length != 6) continue;

                lstvReatimeTrendSettings.Items.Add(new ListViewItem(item));
            }
        }

        private void DrawItemColor(ComboBox comboBox)
        {
            comboBox.DrawItem += (sender, e) =>
            {
                e.DrawBackground();

                Graphics g = e.Graphics;
                Rectangle rect = e.Bounds;
                if (e.Index >= 0)
                {
                    string n = comboBox.Items[e.Index].ToString();
                    Font f = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
                    Color c = Color.FromName(n);
                    Brush b = new SolidBrush(c);
                    g.DrawString(n, f, Brushes.Black, rect.X, rect.Top);
                    g.FillRectangle(b, rect.X + 110, rect.Y + 5, rect.Width - 10, rect.Height - 10);
                }
            };
        }

        private void LoadColor(ComboBox comboBox)
        {
            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo c in propInfoList)
            {
                comboBox.Items.Add(c.Name);
            }

            if (comboBox.Items.Count == 0) return;
            comboBox.SelectedIndex = 1;            
        }

        private void LstvReatimeTrendSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstvReatimeTrendSettings.SelectedItems.Count <= 0) return;

            cbxTagName.TagName = lstvReatimeTrendSettings.SelectedItems[0].SubItems[0].Text;
            txtAlias.Text = lstvReatimeTrendSettings.SelectedItems[0].SubItems[1].Text;
            cbxType.Text = lstvReatimeTrendSettings.SelectedItems[0].SubItems[2].Text;
            cbxFillColor.Text = lstvReatimeTrendSettings.SelectedItems[0].SubItems[3].Text;
            cbxLineColor.Text = lstvReatimeTrendSettings.SelectedItems[0].SubItems[4].Text;
            cbxLineWidth.Text = lstvReatimeTrendSettings.SelectedItems[0].SubItems[5].Text;
        }

        private void BtnAddUpdate_Click(object sender, EventArgs e)
        {
            var tagName = cbxTagName.TagName.Trim();
            var alias = txtAlias.Text.Trim();
            var type = cbxType.Text.Trim();
            var fillColor = cbxFillColor.Text.Trim();
            var lineColor = cbxLineColor.Text.Trim();
            var lineWidth = cbxLineWidth.Text.Trim();

            if (string.IsNullOrEmpty(tagName) ||
                string.IsNullOrEmpty(alias) ||
                string.IsNullOrEmpty(type) ||
                string.IsNullOrEmpty(fillColor) ||
                string.IsNullOrEmpty(lineColor) ||
                string.IsNullOrEmpty(lineWidth)) return;

            if (tagName.Contains("|") || tagName.Contains("&") ||
                alias.Contains("|") || alias.Contains("&") ||
                type.Contains("|") || type.Contains("&") ||
                fillColor.Contains("|") || fillColor.Contains("&") ||
                lineColor.Contains("|") || lineColor.Contains("&") ||
                lineWidth.Contains("|") || lineWidth.Contains("&")) return;

            foreach (ListViewItem listViewItem in lstvReatimeTrendSettings.Items)
            {
                if (listViewItem.SubItems[0].Text == tagName)
                {
                    listViewItem.SubItems[1].Text = alias;
                    listViewItem.SubItems[2].Text = type;
                    listViewItem.SubItems[3].Text = fillColor;
                    listViewItem.SubItems[4].Text = lineColor;
                    listViewItem.SubItems[5].Text = lineWidth;                   

                    return;
                }
            }

            lstvReatimeTrendSettings.Items.Add(new ListViewItem(new string[6]
            {
                tagName,
                alias,
                type,
                fillColor,
                lineColor,
                lineWidth                
            }));
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstvReatimeTrendSettings.SelectedItems.Count <= 0) return;

            foreach (ListViewItem listViewItem in lstvReatimeTrendSettings.SelectedItems)
                lstvReatimeTrendSettings.Items.Remove(listViewItem);
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            if (lstvReatimeTrendSettings.SelectedItems.Count <= 0) return;

            var listViewItem = lstvReatimeTrendSettings.SelectedItems[0];
            var selectedIndex = listViewItem.Index;
            if (selectedIndex > 0)
            {
                lstvReatimeTrendSettings.Items.RemoveAt(selectedIndex);
                lstvReatimeTrendSettings.Items.Insert(selectedIndex - 1, listViewItem);
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            if (lstvReatimeTrendSettings.SelectedItems.Count <= 0) return;

            var listViewItem = lstvReatimeTrendSettings.SelectedItems[0];
            var selectedIndex = listViewItem.Index;
            if (selectedIndex < lstvReatimeTrendSettings.Items.Count - 1)
            {
                lstvReatimeTrendSettings.Items.RemoveAt(selectedIndex);
                lstvReatimeTrendSettings.Items.Insert(selectedIndex + 1, listViewItem);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DataSerialization = "";
            foreach (ListViewItem listViewItem in lstvReatimeTrendSettings.Items)
            {
                var data = string.Format("|{0}&{1}&{2}&{3}&{4}&{5}",
                    listViewItem.SubItems[0].Text,
                    listViewItem.SubItems[1].Text,
                    listViewItem.SubItems[2].Text,
                    listViewItem.SubItems[3].Text,
                    listViewItem.SubItems[4].Text,
                    listViewItem.SubItems[5].Text);

                DataSerialization += data;
            }

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
