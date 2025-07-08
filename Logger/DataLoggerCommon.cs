using ATSCADA.iWinTools.Database;
using ATSCADA.ToolExtensions.PropertyEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Logger
{
    public enum UpdateType
    {
        Timer,
        Event,
        All
    }

    public class DataLoggerSettingsEditor : PropertyEditorBase
    {
        private frmDataLoggerSettings control;

        protected override Control GetEditControl(string PropertyName, object CurrentValue)
        {
            this.control = new frmDataLoggerSettings()
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

    public abstract class ActionDataLogger
    {       
        protected readonly IDataLogConnector connector;

        protected readonly DataLogParam dataLogParam;

        public List<DataLoggerItem> DataLoggerItems { get; set; }

        public ActionDataLogger(DataLogParam dataLogParam)
        {
            this.dataLogParam = dataLogParam;

            var databaseLog = dataLogParam.DatabaseLog;
            this.connector = DataLogConnectorFactory.GetConnector(databaseLog.DatabaseType);
            this.connector.Parametter = databaseLog;
        }

        public abstract void Start();

        protected void Log(string dateTime)
        {
            var containsStatusBad = DataLoggerItems.Any(x => x.SelectedTag.Status == "Bad");
            if (containsStatusBad && !this.dataLogParam.AllowLogWhenBad) return;

            if (this.connector.CreateDatabaseIfNotExists())
                if (this.connector.CreateTableIfNotExists(DataLoggerItems))
                    this.connector.InsertData(dateTime, DataLoggerItems);
        }
    }

    public class DataLoggerByTimer : ActionDataLogger
    {        
        private readonly System.Timers.Timer tmrLog;

        private readonly Stopwatch stopWatch;

        public DataLoggerByTimer(DataLogParam dataLogParam) :
            base(dataLogParam)
        {
            this.stopWatch = new Stopwatch();
            this.tmrLog = new System.Timers.Timer();
            this.tmrLog.Elapsed += (sender, e) => LogData();
        }

        public override void Start()
        {
            if (DataLoggerItems.Count == 0) return;

            this.tmrLog.Interval = GetTimeRate();
            this.tmrLog.AutoReset = false;
            this.tmrLog.Start();            
        }

        private void LogData()
        {
            try
            {
                this.tmrLog.Stop();
                Log($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
                this.tmrLog.Interval = GetTimeRate() - 15;
                this.tmrLog.Start();
            }
            catch
            {
                this.tmrLog.Start();
            }                      
        }


        private double GetTimeRate()
        {
            var dataTimeRate = this.dataLogParam.DataTimeRate;
            return double.TryParse(dataTimeRate.Value, out double timeRate) && timeRate > 1000 ? timeRate : 60000;
        }
    }

    public class DataLoggerByEvent : ActionDataLogger
    {        
        public DataLoggerByEvent(DataLogParam dataLogParam) :
            base(dataLogParam)
        {                
        }

        public override void Start()
        {
            if (DataLoggerItems.Count == 0) return;

            foreach (var item in DataLoggerItems)
                if (item.IsTrigger)
                    item.SelectedTag.TagValueChanged += (sender, e) 
                        => Log($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        }
    }

    public class DataLoggerByAll : ActionDataLogger
    {        
        private readonly System.Timers.Timer tmrLog;

        public DataLoggerByAll(DataLogParam dataLogParam) :
            base(dataLogParam)
        {            
            this.tmrLog = new System.Timers.Timer();
            this.tmrLog.Elapsed += (sender, e) => LogData();
        }

        public override void Start()
        {
            if (DataLoggerItems.Count == 0) return;

            foreach (var item in DataLoggerItems)
                if (item.IsTrigger)
                    item.SelectedTag.TagValueChanged += (sender, e) 
                        => Log($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");

            this.tmrLog.Interval = GetTimeRate() - 15;
            this.tmrLog.AutoReset = false;
            this.tmrLog.Start();
        }

        private void LogData()
        {
            this.tmrLog.Stop();
            this.tmrLog.Interval = GetTimeRate() - 15;            
            this.tmrLog.Start();
            System.Threading.Tasks.Task.Run(() =>
            {
                Log($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
            });
        }

        private double GetTimeRate()
        {
            var dataTimeRate = this.dataLogParam.DataTimeRate;
            return double.TryParse(dataTimeRate.Value, out double timeRate) && timeRate > 1000 ? timeRate : 60000;
        }
    }

    public static class ActionDataLoggerFactory
    {
        public static ActionDataLogger GetActionDataLogger(UpdateType updateType, DataLogParam dataLogParam)
        {
            switch (updateType)
            {
                case UpdateType.Timer:
                    return new DataLoggerByTimer(dataLogParam);
                case UpdateType.Event:
                    return new DataLoggerByEvent(dataLogParam);
                case UpdateType.All:
                    return new DataLoggerByAll(dataLogParam);
                default:
                    return new DataLoggerByTimer(dataLogParam);
            }
        }
    }

    public class DataLoggerItem
    {
        public ITag SelectedTag { get; set; }

        public string Alias { get; set; }

        public bool IsTrigger { get; set; }
    }

    #region DATA LOG CONNECTOR

    public interface IDataLogConnector
    {
        IDatabaseHelper DatabaseHelper { get; set; }

        DatabaseParametter Parametter { get; set; }

        bool CreateDatabaseIfNotExists();

        bool CreateTableIfNotExists(List<DataLoggerItem> items);

        bool InsertData(string dateTime, List<DataLoggerItem> items);
    }

    public class DataLogConnectorFactory
    {
        public static IDataLogConnector GetConnector(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.MySQL:
                    return new DataLogMySQLConnector();
                case DatabaseType.MSSQL:
                    return new DataLogMSSQLConnector();
                default:
                    return new DataLogMySQLConnector();
            }
        }
    }

    public class DataLogMySQLConnector : IDataLogConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public DatabaseParametter Parametter { get; set; }

        public DataLogMySQLConnector()
        {
            Parametter = new DatabaseParametter();
            this.DatabaseHelper = new MySQLHelper();
        }

        public DataLogMySQLConnector(DatabaseParametter parametter)
        {
            Parametter = parametter;
            this.DatabaseHelper = new MySQLHelper();
        }

        public bool CreateDatabaseIfNotExists()
        {
            try
            {
                DatabaseHelper.ConnectionString = 
                    $"Server={Parametter.ServerName};" +
                    $"Uid={Parametter.UserID};" +
                    $"Pwd={Parametter.Password};";

                var query = $"create database if not exists {Parametter.DatabaseName}";
                return DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool CreateTableIfNotExists(List<DataLoggerItem> items)
        {
            try
            {
                DatabaseHelper.ConnectionString = 
                    $"Server={Parametter.ServerName};" +
                    $"Uid={Parametter.UserID};" +
                    $"Pwd={Parametter.Password};" +
                    $"Database={Parametter.DatabaseName}";

                var fieldBuilder = new StringBuilder();                
                foreach (var item in items)
                    fieldBuilder.Append($", `{item.Alias}` varchar(200) not null");                   

                var query = $"create table if not exists {Parametter.TableName} (`DateTime` datetime not null {fieldBuilder})";
                return this.DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }

        public bool InsertData(string dateTime, List<DataLoggerItem> items)
        {
            try
            {
                DatabaseHelper.ConnectionString = 
                    $"Server={Parametter.ServerName};" +
                    $"Uid={Parametter.UserID};" +
                    $"Pwd={Parametter.Password};" +
                    $"Database={Parametter.DatabaseName}";

                var fieldBuilder = new StringBuilder();
                var valueBuilder = new StringBuilder();                
                foreach (var item in items)
                {
                    var value = item.SelectedTag.Value ?? "";

                    fieldBuilder.Append($", `{item.Alias}`");
                    valueBuilder.Append($", '{value}'");                    
                }

                var query = $"insert into {Parametter.TableName} (`DateTime` {fieldBuilder}) values ('{dateTime}' {valueBuilder})";
                return DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }       
    }

    public class DataLogMSSQLConnector : IDataLogConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public DatabaseParametter Parametter { get; set; }

        public DataLogMSSQLConnector()
        {
            Parametter = new DatabaseParametter();
            DatabaseHelper = new MSSQLHelper();
        }

        public DataLogMSSQLConnector(DatabaseParametter parametter)
        {
            Parametter = parametter;
            DatabaseHelper = new MSSQLHelper();
        }
        public bool CreateDatabaseIfNotExists()
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={Parametter.ServerName},{Parametter.Port};User Id={Parametter.UserID};Password={Parametter.Password};";                 
                var query = $"if not exists(select * from sys.databases where name = '{Parametter.DatabaseName}') create database [{Parametter.DatabaseName}]";

                this.DatabaseHelper.ExecuteNonQuery(query);
                return true;
            }
            catch { return false; }
        }

        public bool CreateTableIfNotExists(List<DataLoggerItem> items)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={Parametter.ServerName},{Parametter.Port};User Id={Parametter.UserID};Password={Parametter.Password};Database={Parametter.DatabaseName}";

                var fieldBuilder = new StringBuilder();
                foreach (var item in items)
                    fieldBuilder.Append($", [{item.Alias}] varchar(200) not null");
                var query = $"if not exists (select object_id from sys.tables where name = '{Parametter.TableName}' and schema_name(schema_id) = 'dbo')" +
                    $"create table [{Parametter.TableName}] ([DateTime] datetime not null {fieldBuilder})";

                this.DatabaseHelper.ExecuteNonQuery(query);
                return true;
            }
            catch { return false; }
        }

        public bool InsertData(string dateTime, List<DataLoggerItem> items)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={Parametter.ServerName},{Parametter.Port};User Id={Parametter.UserID};Password={Parametter.Password};Database={Parametter.DatabaseName}";

                var fieldBuilder = new StringBuilder();
                var valueBuilder = new StringBuilder();
                foreach (var item in items)
                {
                    var value = item.SelectedTag.Value ?? "";

                    fieldBuilder.Append($", [{item.Alias}]");
                    valueBuilder.Append($", N'{value}'");
                }

                var query = $"insert into {Parametter.TableName} ([DateTime] {fieldBuilder}) values ('{dateTime}' {valueBuilder})";
                return DatabaseHelper.ExecuteNonQuery(query) < 0 ? false : true;
            }
            catch { return false; }
        }
    }

    #endregion
}
