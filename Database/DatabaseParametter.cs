using ATSCADA.ToolExtensions.Converter;
using System;
using System.ComponentModel;

namespace ATSCADA.iWinTools.Database
{
    [TypeConverter(typeof(ClassConverter<DatabaseParametter>))]
    [Serializable]
    public class DatabaseParametter
    {
        [Description("Database type.")]
        public DatabaseType DatabaseType { get; set; }

        [Description("The name or IP of database server.")]
        public string ServerName { get; set; }

        [Description("Username for login authentication.")]
        public string UserID { get; set; }

        [Description("Password for login authentication.")]
        public string Password { get; set; }

        [Description("The name of database.")]
        public string DatabaseName { get; set; }

        [Description("The name of table that data will be logged into.")]
        public string TableName { get; set; }

        [Description("The port of database server.")]
        public uint Port { get; set; }

        public override string ToString()
        {
            return "Database Settings";
        }
    }
}
