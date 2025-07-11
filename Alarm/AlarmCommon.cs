using ATSCADA.iWinTools.Database;
using System;
using System.Collections.Generic;
using System.Data;

namespace ATSCADA.iWinTools.Alarm
{
    public enum AlarmStatus
    {
        Normal,
        SetPoint,
        Alarm,
        LowAlarm,
        HighAlarm
    }

    public class Condition
    {
        public AlarmStatus Status { get; set; }

        public string Message { get; set; }

        public Condition(AlarmStatus status, string message)
        {
            Status = status;
            Message = message;
        }

        public static readonly Condition Normal = new Condition(AlarmStatus.Normal, "Normal");
    }

    public class AlarmParametter
    {
        public string Alias { get; set; } = "ATSCADA";

        public string Tracking { get; set; }

        public string LowLevel { get; set; }

        public string HighLevel { get; set; }
    }

    public class AlarmStatusChangedEventArgs : EventArgs
    {
        public System.DateTime TimeStamp { get; set; }

        public Condition Condition { get; set; }

        public AlarmItem AlarmItem { get; set; }
    }

    public class AlarmItem
    {
        public string TrackingName { get; set; }

        public string TrackingAlias { get; set; }

        public double TrackingValue { get; set; }

        public double LowLevel { get; set; }

        public double HighLevel { get; set; }
    }

    public interface IAlarmViewerConnector
    {
        IDatabaseHelper DatabaseHelper { get; set; }

        bool CreateDatabaseIfNotExists(DatabaseParametter parametter);

        bool CreateTableIfNotExists(DatabaseParametter parametter);

        /// <summary>
        /// Create active alarms table if not exists
        /// </summary>
        /// <param name="parametter">Database parameters</param>
        /// <param name="activeTableName">Name of the active alarms table</param>
        bool CreateActiveAlarmsTableIfNotExists(DatabaseParametter parametter, string activeTableName);

        DataTable GetDataTableAlarm(DatabaseParametter parametter, uint rowNumber);

        /// <summary>
        /// Get active alarms from the dedicated active alarms table
        /// (Data is maintained by AlarmLogger)
        /// </summary>
        /// <param name="parametter">Database parameters</param>
        /// <param name="activeTableName">Name of the active alarms table</param>
        /// <param name="rowNumber">Maximum number of rows to return</param>
        DataTable GetActiveAlarmsFromTable(DatabaseParametter parametter, string activeTableName, uint rowNumber);

        /// <summary>
        /// Update ACK status for active alarm
        /// (For ACK button functionality)
        /// </summary>
        /// <param name="parametter">Database parameters</param>
        /// <param name="activeTableName">Name of the active alarms table</param>
        /// <param name="tagName">Tag name to update</param>
        /// <param name="acknowledged">ACK status</param>
        bool UpdateActiveAlarmACK(DatabaseParametter parametter, string activeTableName,
                                 string tagName, bool acknowledged);

        bool Acknowledged(DatabaseParametter parametter);
    }
    public class AlarmViewMySQLConnector : IAlarmViewerConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public AlarmViewMySQLConnector()
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
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"create table if not exists `{parametter.TableName}` (`DateTime` Datetime not null, `TagName` varchar(100) not null, `TagAlias` varchar(100) not null, `Value` varchar(45) not null, `HighLevel` varchar(45) not null, `LowLevel` varchar(45) not null, `Status` varchar(200) not null, `Acknowledged` varchar(45) not null)";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool CreateActiveAlarmsTableIfNotExists(DatabaseParametter parametter, string activeTableName)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"create table if not exists `{activeTableName}` (`DateTime` Datetime not null, `TagName` varchar(100) not null, `TagAlias` varchar(100) not null, `Value` varchar(45) not null, `HighLevel` varchar(45) not null, `LowLevel` varchar(45) not null, `Status` varchar(200) not null, `Acknowledged` varchar(45) not null, PRIMARY KEY (`TagName`))";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public DataTable GetDataTableAlarm(DatabaseParametter parametter, uint rowNumber)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"select * from `{parametter.TableName}` order by `DateTime` desc limit 0, {rowNumber}";
                return this.DatabaseHelper.ExecuteQuery(query);
            }
            catch { return null; }
        }

        public DataTable GetActiveAlarmsFromTable(DatabaseParametter parametter, string activeTableName, uint rowNumber)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"select * from `{activeTableName}` order by `DateTime` desc limit 0, {rowNumber}";
                return this.DatabaseHelper.ExecuteQuery(query);
            }
            catch { return null; }
        }

        public bool UpdateActiveAlarmACK(DatabaseParametter parametter, string activeTableName,
                                       string tagName, bool acknowledged)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};Database={parametter.DatabaseName}";
                var ackValue = acknowledged ? "Yes" : "No";
                var query = $"UPDATE `{activeTableName}` SET `Acknowledged` = '{ackValue}' WHERE `TagName` = '{tagName}'";
                return this.DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool Acknowledged(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"update `{parametter.TableName}` set `Acknowledged` = 'Yes' where `Acknowledged` = 'No'";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }
    }
}