using ATSCADA.iWinTools.Alarm;
using ATSCADA.iWinTools.Database;
using ATSCADA.ToolExtensions.PropertyEditor;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Logger
{
    public class AlarmSettingsItem
    {
        public AlarmParametter AlarmParametter { get; set; }

        public string Recipients { get; set; }

        public List<string> GetRecipientList() => Recipients.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).ToList();
    }

    public class AlarmLoggerSettingsEditor : PropertyEditorBase
    {
        private frmAlarmLoggerSettings control;

        protected override Control GetEditControl(string PropertyName, object CurrentValue)
        {
            this.control = new frmAlarmLoggerSettings()
            {
                DataSerialization = (string)CurrentValue
            };

            return this.control;
        }

        protected override object GetEditedValue(Control EditControl, string PropertyName, object OldValue)
        {
            if (this.control == null) return OldValue;
            return this.control.IsCanceled ? OldValue : this.control.DataSerialization;
        }
    }

    #region ALARM SETTINGS CONNECTOR

    public interface IAlarmSettingsConnector
    {
        IDatabaseHelper DatabaseHelper { get; set; }

        bool CreateDatabaseIfNotExists(DatabaseParametter parametter);

        bool CreateTableIfNotExists(DatabaseParametter parametter);

        bool TruncateTableSettings(DatabaseParametter parametter);

        bool UpdateTableSettings(DatabaseParametter parametter, List<AlarmSettingsItem> items);

        List<AlarmSettingsItem> GetAlarmSettingsItems(DatabaseParametter parametter);
    }

    public class AlarmSettingsConnectorFactory
    {
        public static IAlarmSettingsConnector GetConnector(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.MySQL:
                    return new AlarmSettingsMySQLConnector();
                case DatabaseType.MSSQL:
                    return new AlarmSettingsMSSQLConnector();
                default:
                    return new AlarmSettingsMySQLConnector();
            }
        }
    }

    public class AlarmSettingsMySQLConnector : IAlarmSettingsConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public AlarmSettingsMySQLConnector()
        {
            this.DatabaseHelper = new MySQLHelper();
        }
        public bool CreateDatabaseIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};";

                var query = $"create database if not exists {parametter.DatabaseName}";

                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool CreateTableIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"create table if not exists {parametter.TableName} (TagName varchar(100) not null, TagAlias varchar(100) not null, HighLevel varchar(100) not null, LowLevel varchar(100) not null, Email varchar(500))";

                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool TruncateTableSettings(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"truncate table {parametter.TableName}";

                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool UpdateTableSettings(DatabaseParametter parametter, List<AlarmSettingsItem> items)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"insert into {parametter.TableName} values ";

                var count = items.Count;
                for (int index = 0; index < count; index++)
                {
                    var alarmParam = items[index].AlarmParametter;
                    var email = items[index].Recipients;

                    if (index == 0)
                    {
                        query += $" ('{alarmParam.Tracking}', '{alarmParam.Alias}', '{alarmParam.HighLevel}', '{alarmParam.LowLevel}', '{email}')";
                        continue;
                    }

                    query += $", ('{alarmParam.Tracking}', '{alarmParam.Alias}', '{alarmParam.HighLevel}', '{alarmParam.LowLevel}', '{email}')";
                }

                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public List<AlarmSettingsItem> GetAlarmSettingsItems(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"select * from {parametter.TableName}";

                var alarmSettingsDataTable = this.DatabaseHelper.ExecuteQuery(query);
                if (alarmSettingsDataTable == null) return null;

                var alarmSettingsItems = new List<AlarmSettingsItem>();
                foreach (DataRow row in alarmSettingsDataTable.Rows)
                {
                    alarmSettingsItems.Add(new AlarmSettingsItem()
                    {
                        AlarmParametter = new AlarmParametter()
                        {
                            Tracking = row["TagName"].ToString(),
                            Alias = row["TagAlias"].ToString(),
                            HighLevel = row["HighLevel"].ToString(),
                            LowLevel = row["LowLevel"].ToString(),
                        },
                        Recipients = row["Email"].ToString()
                    });
                }

                return alarmSettingsItems;
            }
            catch
            {
                return null;
            }
        }
    }

    public class AlarmSettingsMSSQLConnector : IAlarmSettingsConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public AlarmSettingsMSSQLConnector()
        {
            this.DatabaseHelper = new MSSQLHelper();
        }
        public bool CreateDatabaseIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};";
                var query = $"if not exists(select * from sys.databases where name = '{parametter.DatabaseName}') create database [{parametter.DatabaseName}]";

                this.DatabaseHelper.ExecuteNonQuery(query);
                return true;
            }
            catch { return false; }
        }

        public bool CreateTableIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"if not exists (select object_id from sys.tables where name = '{parametter.TableName}' and schema_name(schema_id) = 'dbo')" +
                    $"create table [{parametter.DatabaseName}].[dbo].[{parametter.TableName}]" +
                    $"([TagName] varchar(100) not null, [TagAlias] varchar(100) not null, [HighLevel] varchar(100) not null, [LowLevel] varchar(100) not null, [Email] varchar(500))";

                this.DatabaseHelper.ExecuteNonQuery(query);
                return true;
            }
            catch { return false; }
        }

        public bool TruncateTableSettings(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"truncate table {parametter.TableName}";

                this.DatabaseHelper.ExecuteNonQuery(query);
                return true;
            }
            catch { return false; }
        }

        public bool UpdateTableSettings(DatabaseParametter parametter, List<AlarmSettingsItem> items)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"insert into {parametter.TableName} values ";

                var count = items.Count;
                for (int index = 0; index < count; index++)
                {
                    var alarmParam = items[index].AlarmParametter;
                    var email = items[index].Recipients;

                    if (index == 0)
                    {
                        query += $" ('{alarmParam.Tracking}', '{alarmParam.Alias}', '{alarmParam.HighLevel}', '{alarmParam.LowLevel}', '{email}')";
                        continue;
                    }

                    query += $", ('{alarmParam.Tracking}', '{alarmParam.Alias}', '{alarmParam.HighLevel}', '{alarmParam.LowLevel}', '{email}')";
                }

                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public List<AlarmSettingsItem> GetAlarmSettingsItems(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"select * from [{parametter.TableName}]";

                var alarmSettingsDataTable = this.DatabaseHelper.ExecuteQuery(query);
                if (alarmSettingsDataTable == null) return null;

                var alarmSettingsItems = new List<AlarmSettingsItem>();
                foreach (DataRow row in alarmSettingsDataTable.Rows)
                {
                    alarmSettingsItems.Add(new AlarmSettingsItem()
                    {
                        AlarmParametter = new AlarmParametter()
                        {
                            Tracking = row["TagName"].ToString(),
                            Alias = row["TagAlias"].ToString(),
                            HighLevel = row["HighLevel"].ToString(),
                            LowLevel = row["LowLevel"].ToString(),
                        },
                        Recipients = row["Email"].ToString()
                    });
                }

                return alarmSettingsItems;
            }
            catch
            {
                return null;
            }
        }
    }

    #endregion

    #region ALARM LOG CONNECTOR

    public interface IAlarmLogConnector
    {
        IDatabaseHelper DatabaseHelper { get; set; }

        bool CreateDatabaseIfNotExists(DatabaseParametter parametter);

        bool CreateTableIfNotExists(DatabaseParametter parametter);

        bool InsertAlarm(DatabaseParametter parametter, AlarmStatusChangedEventArgs e);

        /// <summary>
        /// Create active alarms table if not exists
        /// </summary>
        /// <param name="parametter">Database parameters</param>
        /// <param name="activeTableName">Name of the active alarms table</param>
        bool CreateActiveAlarmsTableIfNotExists(DatabaseParametter parametter, string activeTableName);

        /// <summary>
        /// Add new alarm to active alarms table when alarm occurs
        /// </summary>
        /// <param name="parametter">Database parameters</param>
        /// <param name="activeTableName">Name of the active alarms table</param>
        /// <param name="e">Alarm event args</param>
        bool AddActiveAlarm(DatabaseParametter parametter, string activeTableName, AlarmStatusChangedEventArgs e);

        /// <summary>
        /// Remove alarm from active alarms table when it becomes Normal
        /// </summary>
        /// <param name="parametter">Database parameters</param>
        /// <param name="activeTableName">Name of the active alarms table</param>
        /// <param name="tagName">Tag name to remove</param>
        bool RemoveActiveAlarm(DatabaseParametter parametter, string activeTableName, string tagName);

        /// <summary>
        /// Update existing active alarm (value, status changed but still in alarm)
        /// </summary>
        /// <param name="parametter">Database parameters</param>
        /// <param name="activeTableName">Name of the active alarms table</param>
        /// <param name="e">Alarm event args</param>
        bool UpdateActiveAlarm(DatabaseParametter parametter, string activeTableName, AlarmStatusChangedEventArgs e);
    }

    public class AlarmLogConnectorFactory
    {
        public static IAlarmLogConnector GetConnector(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.MySQL:
                    return new AlarmLogMySQLConnector();
                case DatabaseType.MSSQL:
                    return new AlarmLogMSSQLConnector();
                default:
                    return new AlarmLogMySQLConnector();
            }
        }
    }

    public class AlarmLogMySQLConnector : IAlarmLogConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public AlarmLogMySQLConnector()
        {
            this.DatabaseHelper = new MySQLHelper();
        }

        public bool CreateDatabaseIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};";
                var query = $"create database if not exists `{parametter.DatabaseName}`";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool CreateTableIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"create table if not exists `{parametter.TableName}` (`DateTime` Datetime NOT NULL, `TagName` VARCHAR(100) NOT NULL, `TagAlias` VARCHAR(100) NOT NULL, `Value` VARCHAR(45) NOT NULL, `HighLevel` VARCHAR(45) NOT NULL, `LowLevel` VARCHAR(45) NOT NULL, `Status` VARCHAR(200) NOT NULL, `Acknowledged` VARCHAR(45) NOT NULL)";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool InsertAlarm(DatabaseParametter parametter, AlarmStatusChangedEventArgs e)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";

                var dateTime = e.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                var tagName = e.AlarmItem.TrackingName;
                var tagAlias = e.AlarmItem.TrackingAlias;
                var value = e.AlarmItem.TrackingValue;
                var highLevel = e.AlarmItem.HighLevel;
                var lowLevel = e.AlarmItem.LowLevel;
                var status = e.Condition.Message;

                var query = $"insert into `{parametter.TableName}` values ('{dateTime}', '{tagName}', '{tagAlias}', '{value}', '{highLevel}', '{lowLevel}', '{status}', 'No')";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool CreateActiveAlarmsTableIfNotExists(DatabaseParametter parametter, string activeTableName)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"create table if not exists `{activeTableName}` (`DateTime` Datetime NOT NULL, `TagName` VARCHAR(100) NOT NULL, `TagAlias` VARCHAR(100) NOT NULL, `Value` VARCHAR(45) NOT NULL, `HighLevel` VARCHAR(45) NOT NULL, `LowLevel` VARCHAR(45) NOT NULL, `Status` VARCHAR(200) NOT NULL, `Acknowledged` VARCHAR(45) NOT NULL, PRIMARY KEY (`TagName`))";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool AddActiveAlarm(DatabaseParametter parametter, string activeTableName, AlarmStatusChangedEventArgs e)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";

                var dateTime = e.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                var tagName = e.AlarmItem.TrackingName;
                var tagAlias = e.AlarmItem.TrackingAlias;
                var value = e.AlarmItem.TrackingValue;
                var highLevel = e.AlarmItem.HighLevel;
                var lowLevel = e.AlarmItem.LowLevel;
                var status = e.Condition.Message;

                // Check if this tag already exists in active alarms table
                var checkQuery = $"SELECT COUNT(*) FROM `{activeTableName}` WHERE `TagName` = '{tagName}'";
                var existingCount = this.DatabaseHelper.ExecuteScalarQuery(checkQuery);

                bool tagExists = false;
                if (existingCount != null && int.TryParse(existingCount.ToString(), out int count))
                {
                    tagExists = count > 0;
                }

                string query;
                if (tagExists)
                {
                    // Update existing active alarm (preserve ACK status)
                    query = $@"UPDATE `{activeTableName}` 
                              SET `DateTime` = '{dateTime}', 
                                  `Value` = '{value}', 
                                  `Status` = '{status}',
                                  `HighLevel` = '{highLevel}',
                                  `LowLevel` = '{lowLevel}'
                              WHERE `TagName` = '{tagName}'";
                }
                else
                {
                    // Insert new active alarm (default ACK = No)
                    query = $"INSERT INTO `{activeTableName}` VALUES ('{dateTime}', '{tagName}', '{tagAlias}', '{value}', '{highLevel}', '{lowLevel}', '{status}', 'No')";
                }

                return this.DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool RemoveActiveAlarm(DatabaseParametter parametter, string activeTableName, string tagName)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"DELETE FROM `{activeTableName}` WHERE `TagName` = '{tagName}'";
                return this.DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool UpdateActiveAlarm(DatabaseParametter parametter, string activeTableName, AlarmStatusChangedEventArgs e)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";

                var dateTime = e.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                var tagName = e.AlarmItem.TrackingName;
                var value = e.AlarmItem.TrackingValue;
                var highLevel = e.AlarmItem.HighLevel;
                var lowLevel = e.AlarmItem.LowLevel;
                var status = e.Condition.Message;

                // Update existing active alarm (preserve ACK status)
                var query = $@"UPDATE `{activeTableName}` 
                              SET `DateTime` = '{dateTime}', 
                                  `Value` = '{value}', 
                                  `Status` = '{status}',
                                  `HighLevel` = '{highLevel}',
                                  `LowLevel` = '{lowLevel}'
                              WHERE `TagName` = '{tagName}'";

                return this.DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }
    }

    public class AlarmLogMSSQLConnector : IAlarmLogConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public AlarmLogMSSQLConnector()
        {
            this.DatabaseHelper = new MySQLHelper();
        }

        public bool CreateDatabaseIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};";
                var query = $"create database if not exists `{parametter.DatabaseName}`";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool CreateTableIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"create table if not exists `{parametter.TableName}` (`DateTime` Datetime NOT NULL, `TagName` VARCHAR(100) NOT NULL, `TagAlias` VARCHAR(100) NOT NULL, `Value` VARCHAR(45) NOT NULL, `HighLevel` VARCHAR(45) NOT NULL, `LowLevel` VARCHAR(45) NOT NULL, `Status` VARCHAR(200) NOT NULL, `Acknowledged` VARCHAR(45) NOT NULL)";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool InsertAlarm(DatabaseParametter parametter, AlarmStatusChangedEventArgs e)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";

                var dateTime = e.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                var tagName = e.AlarmItem.TrackingName;
                var tagAlias = e.AlarmItem.TrackingAlias;
                var value = e.AlarmItem.TrackingValue;
                var highLevel = e.AlarmItem.HighLevel;
                var lowLevel = e.AlarmItem.LowLevel;
                var status = e.Condition.Message;

                var query = $"insert into `{parametter.TableName}` values ('{dateTime}', '{tagName}', '{tagAlias}', '{value}', '{highLevel}', '{lowLevel}', '{status}', 'No')";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool CreateActiveAlarmsTableIfNotExists(DatabaseParametter parametter, string activeTableName)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"create table if not exists `{activeTableName}` (`DateTime` Datetime NOT NULL, `TagName` VARCHAR(100) NOT NULL, `TagAlias` VARCHAR(100) NOT NULL, `Value` VARCHAR(45) NOT NULL, `HighLevel` VARCHAR(45) NOT NULL, `LowLevel` VARCHAR(45) NOT NULL, `Status` VARCHAR(200) NOT NULL, `Acknowledged` VARCHAR(45) NOT NULL, PRIMARY KEY (`TagName`))";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool AddActiveAlarm(DatabaseParametter parametter, string activeTableName, AlarmStatusChangedEventArgs e)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";

                var dateTime = e.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                var tagName = e.AlarmItem.TrackingName;
                var tagAlias = e.AlarmItem.TrackingAlias;
                var value = e.AlarmItem.TrackingValue;
                var highLevel = e.AlarmItem.HighLevel;
                var lowLevel = e.AlarmItem.LowLevel;
                var status = e.Condition.Message;

                // Check if this tag already exists in active alarms table
                var checkQuery = $"SELECT COUNT(*) FROM `{activeTableName}` WHERE `TagName` = '{tagName}'";
                var existingCount = this.DatabaseHelper.ExecuteScalarQuery(checkQuery);

                bool tagExists = false;
                if (existingCount != null && int.TryParse(existingCount.ToString(), out int count))
                {
                    tagExists = count > 0;
                }

                string query;
                if (tagExists)
                {
                    // Update existing active alarm (preserve ACK status)
                    query = $@"UPDATE `{activeTableName}` 
                              SET `DateTime` = '{dateTime}', 
                                  `Value` = '{value}', 
                                  `Status` = '{status}',
                                  `HighLevel` = '{highLevel}',
                                  `LowLevel` = '{lowLevel}'
                              WHERE `TagName` = '{tagName}'";
                }
                else
                {
                    // Insert new active alarm (default ACK = No)
                    query = $"INSERT INTO `{activeTableName}` VALUES ('{dateTime}', '{tagName}', '{tagAlias}', '{value}', '{highLevel}', '{lowLevel}', '{status}', 'No')";
                }

                return this.DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool RemoveActiveAlarm(DatabaseParametter parametter, string activeTableName, string tagName)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"DELETE FROM `{activeTableName}` WHERE `TagName` = '{tagName}'";
                return this.DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool UpdateActiveAlarm(DatabaseParametter parametter, string activeTableName, AlarmStatusChangedEventArgs e)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";

                var dateTime = e.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                var tagName = e.AlarmItem.TrackingName;
                var value = e.AlarmItem.TrackingValue;
                var highLevel = e.AlarmItem.HighLevel;
                var lowLevel = e.AlarmItem.LowLevel;
                var status = e.Condition.Message;

                // Update existing active alarm (preserve ACK status)
                var query = $@"UPDATE `{activeTableName}` 
                              SET `DateTime` = '{dateTime}', 
                                  `Value` = '{value}', 
                                  `Status` = '{status}',
                                  `HighLevel` = '{highLevel}',
                                  `LowLevel` = '{lowLevel}'
                              WHERE `TagName` = '{tagName}'";

                return this.DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }
    }
    #endregion
}
