using ATSCADA.iWinTools.Database;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Authentication
{
    [DefaultEvent("RightAccountEvent")]
    public partial class iLogin : UserControl
    {        
        private IAuthenticationConnector connector;

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
        [Description("The name of table.")]
        public string TableName { get; set; } = "useraccount";

        [Category("ATSCADA Database")]
        [Description("The port of database server.")]
        public uint Port { get; set; } = 3306;

        [Category("ATSCADA Database")]
        [Description("ATSCADA user roles.")]
        public string[] UserRoles { get; set; } = new string[2] { "Admin", "Operator" };

        [Category("ATSCADA Database")]
        [Description("Login user name.")]
        [Browsable(false)]
        public string UserName => this.txtUserName.Text.Trim();

        [Category("ATSCADA Database")]
        [Description("Login user role.")]
        [Browsable(false)]
        public string Role => this.cbxRole.Text.Trim();

        public event Action RightAccountEvent;

        public iLogin()
        {
            InitializeComponent();            

            Load += ILogin_Load;
            btnLogin.Click += BtnLogin_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void ILogin_Load(object sender, EventArgs e)
        {
            this.cbxRole.Items.Clear();
            this.cbxRole.Items.AddRange(UserRoles);
            this.databaseParametter = new DatabaseParametter()
            {
                DatabaseType = this.DatabaseType,
                ServerName = this.ServerName,
                UserID = this.UserID,
                Password = this.Password,
                DatabaseName = this.DatabaseName,
                TableName = this.TableName,
                Port = this.Port
            };
            this.connector = AuthenticationConnectorFactory.GetConnector(DatabaseType);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {            
            var userName = txtUserName.Text.Trim();
            var password = txtPassword.Text.Trim();
            var role = cbxRole.Text.Trim();

            if (string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please enter all required information!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }           
           
            var account = new Account()
            {
                UserName = userName,
                Password = password,
                Role = role
            };

            if (this.connector.CreateDatabaseIfNotExists(this.databaseParametter))
                if (this.connector.CreateTableIfNotExists(this.databaseParametter))
                {
                    if (this.connector.Login(this.databaseParametter, account))
                    {
                        OnRightAccountEvent();
                    }
                    else
                    {
                        MessageBox.Show("Wrong User, Password or Role!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            txtUserName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            cbxRole.Text = string.Empty;
        }

        private void OnRightAccountEvent()
        {
            Action handler;
            lock (this) { handler = RightAccountEvent; }
            handler?.Invoke();
        }
    }
}
