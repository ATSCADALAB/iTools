namespace ATSCADA.iWinTools.Authentication
{
    partial class iUpdateUserAccount
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grbUpdateUserAccount = new System.Windows.Forms.GroupBox();
            this.cbxOldRole = new System.Windows.Forms.ComboBox();
            this.lblOldValue = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.lblNewPassword = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.cbxNewRole = new System.Windows.Forms.ComboBox();
            this.lblNewRole = new System.Windows.Forms.Label();
            this.txtOldPassword = new System.Windows.Forms.TextBox();
            this.lblOldPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.grbUpdateUserAccount.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbUpdateUserAccount
            // 
            this.grbUpdateUserAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbUpdateUserAccount.Controls.Add(this.cbxOldRole);
            this.grbUpdateUserAccount.Controls.Add(this.lblOldValue);
            this.grbUpdateUserAccount.Controls.Add(this.txtConfirmPassword);
            this.grbUpdateUserAccount.Controls.Add(this.lblConfirmPassword);
            this.grbUpdateUserAccount.Controls.Add(this.txtNewPassword);
            this.grbUpdateUserAccount.Controls.Add(this.lblNewPassword);
            this.grbUpdateUserAccount.Controls.Add(this.txtUserName);
            this.grbUpdateUserAccount.Controls.Add(this.btnCancel);
            this.grbUpdateUserAccount.Controls.Add(this.btnUpdate);
            this.grbUpdateUserAccount.Controls.Add(this.cbxNewRole);
            this.grbUpdateUserAccount.Controls.Add(this.lblNewRole);
            this.grbUpdateUserAccount.Controls.Add(this.txtOldPassword);
            this.grbUpdateUserAccount.Controls.Add(this.lblOldPassword);
            this.grbUpdateUserAccount.Controls.Add(this.lblUserName);
            this.grbUpdateUserAccount.Location = new System.Drawing.Point(0, 0);
            this.grbUpdateUserAccount.Name = "grbUpdateUserAccount";
            this.grbUpdateUserAccount.Size = new System.Drawing.Size(376, 262);
            this.grbUpdateUserAccount.TabIndex = 40;
            this.grbUpdateUserAccount.TabStop = false;
            this.grbUpdateUserAccount.Text = "Update User Account";
            // 
            // cbxOldRole
            // 
            this.cbxOldRole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxOldRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxOldRole.FormattingEnabled = true;
            this.cbxOldRole.Items.AddRange(new object[] {
            "Admin",
            "Operator"});
            this.cbxOldRole.Location = new System.Drawing.Point(127, 153);
            this.cbxOldRole.Name = "cbxOldRole";
            this.cbxOldRole.Size = new System.Drawing.Size(234, 23);
            this.cbxOldRole.TabIndex = 4;
            // 
            // lblOldValue
            // 
            this.lblOldValue.AutoSize = true;
            this.lblOldValue.Location = new System.Drawing.Point(10, 156);
            this.lblOldValue.Name = "lblOldValue";
            this.lblOldValue.Size = new System.Drawing.Size(55, 15);
            this.lblOldValue.TabIndex = 32;
            this.lblOldValue.Text = "Old Role";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfirmPassword.Location = new System.Drawing.Point(127, 121);
            this.txtConfirmPassword.Multiline = true;
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '•';
            this.txtConfirmPassword.Size = new System.Drawing.Size(234, 23);
            this.txtConfirmPassword.TabIndex = 3;
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.Location = new System.Drawing.Point(10, 124);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(107, 15);
            this.lblConfirmPassword.TabIndex = 30;
            this.lblConfirmPassword.Text = "Confirm Password";
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewPassword.Location = new System.Drawing.Point(127, 89);
            this.txtNewPassword.Multiline = true;
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '•';
            this.txtNewPassword.Size = new System.Drawing.Size(234, 23);
            this.txtNewPassword.TabIndex = 2;
            // 
            // lblNewPassword
            // 
            this.lblNewPassword.AutoSize = true;
            this.lblNewPassword.Location = new System.Drawing.Point(10, 92);
            this.lblNewPassword.Name = "lblNewPassword";
            this.lblNewPassword.Size = new System.Drawing.Size(89, 15);
            this.lblNewPassword.TabIndex = 28;
            this.lblNewPassword.Text = "New Password";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(127, 25);
            this.txtUserName.Multiline = true;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(234, 23);
            this.txtUserName.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(249, 218);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(127, 218);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(112, 28);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // cbxNewRole
            // 
            this.cbxNewRole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxNewRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxNewRole.FormattingEnabled = true;
            this.cbxNewRole.Items.AddRange(new object[] {
            "Admin",
            "Operator"});
            this.cbxNewRole.Location = new System.Drawing.Point(127, 185);
            this.cbxNewRole.Name = "cbxNewRole";
            this.cbxNewRole.Size = new System.Drawing.Size(234, 23);
            this.cbxNewRole.TabIndex = 5;
            // 
            // lblNewRole
            // 
            this.lblNewRole.AutoSize = true;
            this.lblNewRole.Location = new System.Drawing.Point(10, 188);
            this.lblNewRole.Name = "lblNewRole";
            this.lblNewRole.Size = new System.Drawing.Size(61, 15);
            this.lblNewRole.TabIndex = 26;
            this.lblNewRole.Text = "New Role";
            // 
            // txtOldPassword
            // 
            this.txtOldPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOldPassword.Location = new System.Drawing.Point(127, 57);
            this.txtOldPassword.Multiline = true;
            this.txtOldPassword.Name = "txtOldPassword";
            this.txtOldPassword.PasswordChar = '•';
            this.txtOldPassword.Size = new System.Drawing.Size(234, 23);
            this.txtOldPassword.TabIndex = 1;
            // 
            // lblOldPassword
            // 
            this.lblOldPassword.AutoSize = true;
            this.lblOldPassword.Location = new System.Drawing.Point(10, 60);
            this.lblOldPassword.Name = "lblOldPassword";
            this.lblOldPassword.Size = new System.Drawing.Size(83, 15);
            this.lblOldPassword.TabIndex = 2;
            this.lblOldPassword.Text = "Old Password";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(10, 28);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(70, 15);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User Name";
            // 
            // iUpdateUserAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grbUpdateUserAccount);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "iUpdateUserAccount";
            this.Size = new System.Drawing.Size(376, 262);
            this.grbUpdateUserAccount.ResumeLayout(false);
            this.grbUpdateUserAccount.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbUpdateUserAccount;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.ComboBox cbxNewRole;
        private System.Windows.Forms.Label lblNewRole;
        private System.Windows.Forms.TextBox txtOldPassword;
        private System.Windows.Forms.Label lblOldPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.ComboBox cbxOldRole;
        private System.Windows.Forms.Label lblOldValue;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
    }
}
