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

        DataTable GetDataTableAlarm(DatabaseParametter parametter, uint rowNumber);

        bool Acknowledged(DatabaseParametter parametter);
    }

    public class AlarmViewerConnectorFactory
    {
        public static IAlarmViewerConnector GetConnector(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.MySQL:
                    return new AlarmViewMySQLConnector();
                case DatabaseType.MSSQL:
                    return new AlarmViewMSSQLConnector();
                default:
                    return new AlarmViewMySQLConnector();
            }
        }
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

        public DataTable GetDataTableAlarm(DatabaseParametter parametter, uint rowNumber)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"select * from {parametter.TableName} order by `DateTime` desc limit 0, {rowNumber}";

                return this.DatabaseHelper.ExecuteQuery(query);
            }
            catch { return null; }
        }

        public bool Acknowledged(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"update {parametter.TableName} set `Acknowledged` = 'Yes' where `Acknowledged` = 'No'";

                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }
    }

    public class AlarmViewMSSQLConnector : IAlarmViewerConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public AlarmViewMSSQLConnector()
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
            catch
            { 
                return false; 
            }
        }

        public bool CreateTableIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"if not exists (select object_id from sys.tables where name = '{parametter.TableName}' and schema_name(schema_id) = 'dbo')" +
                    $"create table [{parametter.DatabaseName}].[dbo].[{parametter.TableName}]" +
                    $"([DateTime] Datetime not null, [TagName] varchar(100) not null, [TagAlias] varchar(100) not null, [Value] varchar(45) not null, [HighLevel] varchar(45) not null, [LowLevel] varchar(45) not null, [Status] varchar(200) not null, [Acknowledged] varchar(45) not null)";

                this.DatabaseHelper.ExecuteNonQuery(query);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataTable GetDataTableAlarm(DatabaseParametter parametter, uint rowNumber)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"select top {rowNumber} * from [{parametter.TableName}] order by [DateTime] desc";
                return this.DatabaseHelper.ExecuteQuery(query);
            }
            catch
            {
                return default;
            }
        }

        public bool Acknowledged(DatabaseParametter parametter)
        {
            try
            {
                this.DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"update [{parametter.TableName}] set [Acknowledged] = 'Yes' where [Acknowledged] = 'No'";

                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch
            {
                return false;
            }
        }
    }
}
