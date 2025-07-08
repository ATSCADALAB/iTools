using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace ATSCADA.iWinTools.Report
{
    public partial class frmAlarmReporterSettings : Form
    {
        public string SerializeString;

        public bool IsCanceled;

        public frmAlarmReporterSettings()
        {
            InitializeComponent();
            this.Load += FrmAlarmReporterSettings_Load;
            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void FrmAlarmReporterSettings_Load(object sender, EventArgs e)
        {
            try
            {
                //ListTag
                //TaskConverter TC = new TaskConverter();

                //TC.Task_Convert();

                //comboBox1.Items.AddRange(TC.TagCol.ToArray());
                //comboBox1.Text = comboBox1.Items[0].ToString();

                if ((SerializeString != null) && (SerializeString != ""))
                {
                    string[] ST = SerializeString.Split('|');

                    //Display all Value of TimeStampList onto listview
                    for (short i = 0; i < ST.Length; i++)
                    {
                        listView1.Items.Add(new ListViewItem(ST[i]));
                    }
                }
            }
            catch { }
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
                SerializeString = li.Text;

                for (short i = 1; i < listView1.Items.Count; i++)
                {
                    SerializeString = SerializeString + "|" + listView1.Items[i].Text;
                }
                //
                IsCanceled = false;
                this.Hide();
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
                    if (li.SubItems[0].Text == smartTagComboBox1.TagName)
                    {
                        return;
                    }
                }
                string[] s = new string[1];
                s[0] = smartTagComboBox1.TagName;
                listView1.Items.Add(new ListViewItem(s));
            }
            catch { }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
