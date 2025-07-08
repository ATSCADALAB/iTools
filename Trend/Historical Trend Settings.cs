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

namespace ATSCADA.iWinTools.Trend
{
    public partial class Historical_Trend_Settings : Form
    {
        public string SerializeString;        

        public bool IsCanceled;

        public Historical_Trend_Settings()
        {
            InitializeComponent();
        }

        private void Historical_Trend_Settings_Load(object sender, EventArgs e)
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

                string[] _myRawString = _myString.Split('|');
                List<string> _myAlias = new List<string>();
                foreach (string s in _myRawString)
                {
                    _myAlias.Add(s.Split('&')[1]);
                }

                comboBox1.Items.AddRange(_myAlias.ToArray());
                comboBox1.Text = comboBox1.Items[0].ToString();

                //Color list
                comboBox2.DrawItem += new DrawItemEventHandler(comboBox2_DrawItem);
                comboBox3.DrawItem += new DrawItemEventHandler(comboBox3_DrawItem);

                Type colorType = typeof(System.Drawing.Color);
                PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
                foreach (PropertyInfo c in propInfoList)
                {
                    comboBox2.Items.Add(c.Name);
                    comboBox2.Text = c.Name;
                    comboBox3.Items.Add(c.Name);
                    comboBox3.Text = "Transparent";
                }

                string[] ST = SerializeString.Split('|');

                //Display all Value of TimeStampList onto listview
                for(short i=0; i< ST.Length ; i++)
                {
                    string[] _s = ST[i].Split(',');
                    if (_s[0] == "" || _s[0] == null)
                        return;
                    listView1.Items.Add(new ListViewItem(_s));
                }
            }
            catch {} 
        }

        private void comboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Rectangle rect = e.Bounds;
                if (e.Index >= 0)
                {
                    string n = ((ComboBox)sender).Items[e.Index].ToString();
                    Font f = new Font("Arial", 9, FontStyle.Regular);
                    Color c = Color.FromName(n);
                    Brush b = new SolidBrush(c);
                    g.DrawString(n, f, Brushes.Black, rect.X, rect.Top);
                    g.FillRectangle(b, rect.X + 110, rect.Y + 5, rect.Width - 10, rect.Height - 10);
                }
            }
            catch { }
        }

        private void comboBox3_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Rectangle rect = e.Bounds;
                if (e.Index >= 0)
                {
                    string n = ((ComboBox)sender).Items[e.Index].ToString();
                    Font f = new Font("Arial", 9, FontStyle.Regular);
                    Color c = Color.FromName(n);
                    Brush b = new SolidBrush(c);
                    g.DrawString(n, f, Brushes.Black, rect.X, rect.Top);
                    g.FillRectangle(b, rect.X + 110, rect.Y + 5, rect.Width - 10, rect.Height - 10);
                }
            }
            catch { }
        }

        //Add to Listview
        private void button1_Click(object sender, EventArgs e)        
        {
            try
            {
                foreach (ListViewItem li in listView1.Items)
                {
                    if (li.SubItems[0].Text == comboBox1.Text)
                    {
                        li.SubItems[1].Text = comboBox2.Text;
                        li.SubItems[2].Text = comboBox3.Text;
                        li.SubItems[3].Text = comboBox5.Text;
                        li.SubItems[4].Text = comboBox4.Text;

                        return;
                    }
                }

                string[] s = new string[5];
                s[0] = comboBox1.Text;
                s[1] = comboBox2.Text;
                s[2] = comboBox3.Text;
                s[3] = comboBox5.Text;
                s[4] = comboBox4.Text;

                listView1.Items.Add(new ListViewItem(s));
            }
            catch { }
        }

        //Remove
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                if (listView1.SelectedItems.Count > 0)
                {
                    foreach(ListViewItem li in listView1.SelectedItems)  
                        listView1.Items.Remove(li);
                }
            }
            catch{ } 
        }

        //Ok -> Generate new Serializestring
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SerializeString = "";
                ListViewItem li = listView1.Items[0];
                SerializeString = li.Text + "," + li.SubItems[1].Text + "," + li.SubItems[2].Text + "," + li.SubItems[3].Text + "," + li.SubItems[4].Text;

                for (short i = 1; i < listView1.Items.Count; i++)
                {
                    SerializeString = SerializeString + "|" + listView1.Items[i].Text + "," + listView1.Items[i].SubItems[1].Text + ","
                                                            + listView1.Items[i].SubItems[2].Text + "," + listView1.Items[i].SubItems[3].Text + "," + listView1.Items[i].SubItems[4].Text;
                }
                //
                IsCanceled = false;
                this.Hide();
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IsCanceled = true;
            this.Hide(); 
        }
        //Listview click
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;
                comboBox2.Text = listView1.SelectedItems[0].SubItems[1].Text;
                comboBox3.Text = listView1.SelectedItems[0].SubItems[2].Text;
                comboBox5.Text = listView1.SelectedItems[0].SubItems[3].Text;
                comboBox4.Text = listView1.SelectedItems[0].SubItems[4].Text;
            }
            catch { } 
        }
        //Up
        private void button5_Click(object sender, EventArgs e)
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
        //Down
        private void button6_Click(object sender, EventArgs e)
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
        
    }
}
