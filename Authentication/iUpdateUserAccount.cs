using ATSCADA.iWinTools.Database;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Authentication
{
    public partial class iUpdateUserAccount : UserControl
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

        public iUpdateUserAccount()
        {
            InitializeComponent();

            Load += IUpdateUserAccount_Load;
            btnUpdate.Click += BtnUpdate_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void IUpdateUserAccount_Load(object sender, EventArgs e)
        {
            this.cbxOldRole.Items.Clear();
            this.cbxOldRole.Items.AddRange(UserRoles);
            this.cbxNewRole.Items.Clear();
            this.cbxNewRole.Items.AddRange(UserRoles);

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

        private void BtnUpdate_Click(object sender, EventArgs e)
        {            
            var userName = txtUserName.Text.Trim();
            var oldPassword = txtOldPassword.Text.Trim();
            var newPassword = txtNewPassword.Text.Trim();
            var confirmPassword = txtConfirmPassword.Text.Trim();
            var oldRole = cbxOldRole.Text.Trim();
            var newRole = cbxNewRole.Text.Trim();

            if (string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(oldPassword) ||
                string.IsNullOrEmpty(newPassword) ||
                string.IsNullOrEmpty(confirmPassword) ||
                string.IsNullOrEmpty(oldRole) ||
                string.IsNullOrEmpty(newRole))
            {
                MessageBox.Show("Please enter all required information!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }           

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New Password and Confirm Password do not match!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var oldAccount = new Account()
            {
                UserName = userName,
                Password = oldPassword,
                Role = oldRole
            };

            var newAccount = new Account()
            {
                UserName = userName,
                Password = newPassword,
                Role = newRole
            };

            if (this.connector.CreateDatabaseIfNotExists(this.databaseParametter))
                if (this.connector.CreateTableIfNotExists(this.databaseParametter))
                {
                    if (!this.connector.Login(this.databaseParametter, oldAccount))
                    {
                        MessageBox.Show("Wrong User Name, Old Passcode or Old Role!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (this.connector.UpdateAccount(this.databaseParametter, newAccount, oldAccount))
                        MessageBox.Show("Successfully updated account!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            MessageBox.Show("Account data update failed!", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            txtUserName.Text = string.Empty;
            txtOldPassword.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
            cbxOldRole.Text = string.Empty;
            cbxNewRole.Text = string.Empty;
        }
    }
}
