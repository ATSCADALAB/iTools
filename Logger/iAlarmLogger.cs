using ATSCADA.iWinTools.Alarm;
using ATSCADA.iWinTools.Database;
using ATSCADA.iWinTools.Email;
using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace ATSCADA.iWinTools.Logger
{
    public partial class iAlarmLogger : Component
    {
        private ITag emailTag;

        private List<AlarmSettingsItem> alarmSettingsItems;

        private IAlarmLogConnector logConnector;

        private EmailCore emailCore;

        private iDriver driver;        

        [Category("ATSCADA Settings")]
        [Description("Select driver object.")]
        public iDriver Driver
        {
            get => driver;
            set
            {
                if (driver != null) driver.ConstructionCompleted -= Driver_ConstructionCompleted;
                driver = value;
                if (driver != null) driver.ConstructionCompleted += Driver_ConstructionCompleted;
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Select tag for ATSCADA control.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string EmailTagName { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Settings of database.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DatabaseParametter DatabaseLog { get; set; } = new DatabaseParametter()
        {
            DatabaseType = DatabaseType.MySQL,
            ServerName = "localhost",
            UserID = "root",
            Password = "101101",
            DatabaseName = "ATSCADA",
            TableName = "alarmlog",
            Port = 3306
        };

        [Category("ATSCADA Settings")]
        [Description("Settings of database.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DatabaseParametter DatabaseSettings { get; set; } = new DatabaseParametter()
        {
            DatabaseType = DatabaseType.MySQL,
            ServerName = "localhost",
            UserID = "root",
            Password = "101101",
            DatabaseName = "ATSCADA",
            TableName = "alarmsettings",
            Port = 3306
        };

        [Category("ATSCADA Settings")]
        [Description("Settings of email.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EmailConfigParametter EmailConfig { get; set; } = new EmailConfigParametter()
        {
            Host = "smtp.gmail.com",
            Port = 587,
            TimeOut = 10000,
            EnableSSL = true,
            CredentialEmail = "",
            CredentialPass = "",
        };

        [Category("ATSCADA Settings")]
        [Description("Settings alarm tag collection.")]
        [Editor(typeof(AlarmLoggerSettingsEditor), typeof(UITypeEditor))]
        public string Collection
        {
            get => string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                Enum.GetName(typeof(DatabaseType), DatabaseLog.DatabaseType),
                DatabaseSettings.ServerName,
                DatabaseSettings.UserID,
                DatabaseSettings.Password,
                DatabaseSettings.DatabaseName,
                DatabaseSettings.TableName,
                DatabaseSettings.Port);
        }
       
        public iAlarmLogger()
        {
            InitializeComponent();
        }

        public iAlarmLogger(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private void Driver_ConstructionCompleted()
        {
            this.emailTag = this.driver.GetTagByName(EmailTagName);
            var settingsConnector = AlarmSettingsConnectorFactory.GetConnector(DatabaseSettings.DatabaseType);     
            if (settingsConnector.CreateDatabaseIfNotExists(DatabaseSettings))
                if (settingsConnector.CreateTableIfNotExists(DatabaseSettings))
                    this.alarmSettingsItems = settingsConnector.GetAlarmSettingsItems(DatabaseSettings);

            if (this.alarmSettingsItems == null) return;
            if (this.alarmSettingsItems.Count == 0) return;

            this.logConnector = AlarmLogConnectorFactory.GetConnector(DatabaseLog.DatabaseType);
            this.emailCore = new EmailCore(EmailConfig);

            CreateAlarmTag();
        }

        private void CreateAlarmTag()
        {
            foreach (var alarmSettingsItem in this.alarmSettingsItems)
            {
                var alarmTag = new AlarmTag(this.driver, alarmSettingsItem.AlarmParametter);
                var recipients = alarmSettingsItem.GetRecipientList();

                alarmTag.StatusChanged += (sender, e) =>
                {
                    LogAlarm(e);
                    SendEmail(e, recipients);
                };
            }
        }

        private void LogAlarm(AlarmStatusChangedEventArgs e)
        {
            if (this.logConnector.CreateDatabaseIfNotExists(DatabaseLog))
                if (this.logConnector.CreateTableIfNotExists(DatabaseLog))
                    this.logConnector.InsertAlarm(DatabaseLog, e);
        }

        private void SendEmail(AlarmStatusChangedEventArgs e, List<string> recipients)
        {
            if (recipients is null && (this.emailTag is null)) return;           

            var message = $" DateTime: {e.TimeStamp}\n Tag: {e.AlarmItem.TrackingName}\n Value: {e.AlarmItem.TrackingValue}\n" +
                $" High Level: {e.AlarmItem.HighLevel}\n Low Level: {e.AlarmItem.LowLevel}\n Status: {e.Condition.Message}";

            var mailMessageParameter = new MailMessageParametter()
            {
                Sender = EmailConfig.CredentialEmail,
                Subject = $"ATSCADA ALARM - {e.AlarmItem.TrackingAlias}",
                Body = message
            };

            foreach (var recipient in recipients)
                mailMessageParameter.AddRecipient(recipient);
            if (this.emailTag != null)
            {
                var emailArray = this.emailTag.Value.Trim().Split(',');
                var count = emailArray.Length;
                for (int index = 0; index < count; index++)
                {
                    var email = emailArray[index].Trim();
                    if (string.IsNullOrEmpty(email)) continue;

                    mailMessageParameter.AddRecipient(email);
                }
            }
            this.emailCore.SendEmail(mailMessageParameter);

        }
    }
}
