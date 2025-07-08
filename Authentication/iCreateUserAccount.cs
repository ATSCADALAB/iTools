using ATSCADA.iWinTools.Database;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Authentication
{
    public partial class iCreateUserAccount : UserControl
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

        public iCreateUserAccount()
        {
            InitializeComponent();

            Load += ICreateUserAccount_Load;
            btnCreate.Click += BtnCreate_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void ICreateUserAccount_Load(object sender, EventArgs e)
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

        private void BtnCreate_Click(object sender, EventArgs e)
        {           
            var userName = txtUserName.Text.Trim();
            var password = txtPassword.Text.Trim();
            var retypePassword = txtRetypePassword.Text.Trim();
            var role = cbxRole.Text.Trim();

            if (string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(retypePassword) ||
                string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please enter all required information!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }          

            if (password != retypePassword)
            {
                MessageBox.Show("Password and Retype Password do not match!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (this.connector.CheckAccountIsAvailable(this.databaseParametter, account))
                    {
                        MessageBox.Show("This User Name is available, please choose another User Name!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (this.connector.CreateAccount(this.databaseParametter, account))
                        MessageBox.Show("Successfully add new account!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            MessageBox.Show("Your account creation request failed!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            txtUserName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtRetypePassword.Text = string.Empty;
            cbxRole.Text = string.Empty;
        }
    }
}
