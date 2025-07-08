using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Loading
{
    public partial class Loading_Procees_Screen : Form
    {
        protected int _Form_Size_Width = 300;
        protected int _Form_Size_Height = 200;
        public int width;
        public int height;
        public Image img;
        public string loadingTxt;
        public string noteTxt;

        public Loading_Procees_Screen()
        {
            this.WindowState = FormWindowState.Minimized;

            InitializeComponent();
         
            this.BackColor = Color.Gray;
            this.TransparencyKey = Color.Gray;
            
            //panel1.Parent = pictureBox1;
            //label1.Parent = panel1;
            //panel2.Parent = pictureBox1;
            //label2.Parent = panel2;
        }

        private void Center(Form form)
        {
            form.Location = new Point((Screen.PrimaryScreen.Bounds.Size.Width / 2) - (form.Size.Width / 2), (Screen.PrimaryScreen.Bounds.Size.Height / 2) - (form.Size.Height / 2));
        }

        private void Loading_Procees_Screen_Load(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Normal;           

            if (img != null)
            {
                pictureBox1.Image = img;
            }
            label1.Text = loadingTxt;
            panel1.Size = new Size(width, (height - 140) / 2);
            label1.Size = new Size(width, 40);

            label2.Text = noteTxt;
            panel2.Size = new Size(width, (height - 140) / 2);
            label2.Size = new Size(width, 30);

            this.Size = new Size(width, height);
            //this.StartPosition = FormStartPosition.CenterParent;
            Center(this);
        

        }


    }
}
