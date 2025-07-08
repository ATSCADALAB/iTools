using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace ATSCADA.iWinTools.Report
{
    public partial class frmDataReporterSettings : Form
    {
        public string SerializeString;

        public bool IsCanceled;

        public frmDataReporterSettings()
        {
            InitializeComponent();
            this.Load += FrmDataReporterSettings_Load;
            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;
            btnUp.Click += BtnUp_Click;
            btnDown.Click += BtnDown_Click;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            DrawItemColor(cbxColumnColor);
        }

        private void FrmDataReporterSettings_Load(object sender, EventArgs e)
        {
            try
            {
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

                string _myString = "";
                if (File.Exists(_FullPath))
                {
                    _myString = File.ReadAllText(_FullPath);
                }

                if (!string.IsNullOrEmpty(_myString))
                {
                    string[] _myRawString = _myString.Split('|');
                    List<string> _myAlias = new List<string>();
                    foreach (string s in _myRawString)
                    {
                        _myAlias.Add(s.Split('&')[1]);
                    }

                    comboBox1.Items.AddRange(_myAlias.ToArray());
                }
                
                
                //comboBox1.Text = comboBox1.Items[0].ToString();

                //Color list
                // cbxColumnColor.DrawItem += new DrawItemEventHandler(comboBox2_DrawItem);

                Type colorType = typeof(System.Drawing.Color);
                PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
                foreach (PropertyInfo c in propInfoList)
                {
                    cbxColumnColor.Items.Add(c.Name);
                    cbxColumnColor.Text = c.Name;
                }


               

                if ((SerializeString != null) && (SerializeString != ""))
                {
                    string[] ST = SerializeString.Split('|');

                    //Display all Value of TimeStampList onto listview
                    for (short i = 0; i < ST.Length; i++)
                    {
                        string[] s = ST[i].Split(',');
                        ListViewItem li = new ListViewItem(s);
                        listView1.Items.Add(li);
                    }
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            IsCanceled = true;
            this.Hide();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                SerializeString = "";
                ListViewItem li = listView1.Items[0];
                SerializeString = li.Text + "," + li.SubItems[1].Text;

                for (short i = 1; i < listView1.Items.Count; i++)
                {
                    SerializeString = SerializeString + "|" + listView1.Items[i].Text + "," + listView1.Items[i].SubItems[1].Text;
                }
                
                IsCanceled = false;
                this.Hide();
            }
            catch { }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem li = listView1.SelectedItems[0];
                int _SelectedIndex = li.Index;
                if (_SelectedIndex < listView1.Items.Count - 1)
                {
                    listView1.Items.RemoveAt(_SelectedIndex);
                    listView1.Items.Insert(_SelectedIndex + 1, li);
                }
            }
            catch { }
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem li = listView1.SelectedItems[0];
                int _SelectedIndex = li.Index;
                if (_SelectedIndex > 0)
                {
                    listView1.Items.RemoveAt(_SelectedIndex);
                    listView1.Items.Insert(_SelectedIndex - 1, li);
                }
            }
            catch { }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem li in listView1.SelectedItems)
                        listView1.Items.Remove(li);
                }
            }
            catch { }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem li in listView1.Items)
                {
                    if (li.SubItems[0].Text == comboBox1.Text)
                    {
                        li.SubItems[1].Text = cbxColumnColor.Text;
                        return;
                    }
                }

                string[] s = new string[2];
                s[0] = comboBox1.Text;
                s[1] = cbxColumnColor.Text;
                listView1.Items.Add(new ListViewItem(s));
            }
            catch { }
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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;
                cbxColumnColor.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
            catch { }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {

        }

        private void cbxColumnColor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
