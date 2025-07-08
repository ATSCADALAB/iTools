using ATSCADA.iWinTools.Database;
using ATSCADA.iWinTools.Encyption;
using System;

namespace ATSCADA.iWinTools.Authentication
{
    public class Account
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }

    public class AuthenticationConnectorFactory
    {
        public static IAuthenticationConnector GetConnector(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.MySQL:
                    return new AuthenticationMySQLConnector();
                case DatabaseType.MSSQL:
                    return new AuthenticationMSSQLConnector();
                default:
                    return new AuthenticationMySQLConnector();
            }
        }
    }

    public interface IAuthenticationConnector
    {
        bool CreateDatabaseIfNotExists(DatabaseParametter parametter);

        bool CreateTableIfNotExists(DatabaseParametter parametter);

        bool Login(DatabaseParametter parametter, Account account);

        bool CheckAccountIsAvailable(DatabaseParametter parametter, Account account);

        bool CreateAccount(DatabaseParametter parametter, Account account);

        bool UpdateAccount(DatabaseParametter parametter, Account account, Account oldAccount);
    }

    public class AuthenticationMySQLConnector : IAuthenticationConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public AuthenticationMySQLConnector()
        {
            DatabaseHelper = new MySQLHelper();
        }
        public bool CreateDatabaseIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password};";

                var query = $"create database if not exists {parametter.DatabaseName}";

                return DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool CreateTableIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"create table if not exists {parametter.TableName} (UserName varchar(100) not null, Password varchar(45) not null, Role varchar(45) not null)";

                return DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool Login(DatabaseParametter parametter, Account account)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"select count(*) from {parametter.TableName} where UserName = '{account.UserName}' and Password = '{account.Password.ToMD5()}' and Role = '{account.Role}'";

                return Convert.ToInt32(DatabaseHelper.ExecuteScalarQuery(query)) > 0;
            }
            catch { return false; }
        }
       
        public bool CheckAccountIsAvailable(DatabaseParametter parametter, Account account)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"select count(*) from {parametter.TableName} where UserName = '{account.UserName}'";

                return Convert.ToInt32(DatabaseHelper.ExecuteScalarQuery(query)) > 0;
            }
            catch { return false; }
        }

        public bool CreateAccount(DatabaseParametter parametter, Account account)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"insert into {parametter.TableName} values ('{account.UserName}', '{account.Password.ToMD5()}', '{account.Role}')";

                return DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool UpdateAccount(DatabaseParametter parametter, Account account, Account oldAccount)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName};Port={parametter.Port};Uid={parametter.UserID};Pwd={parametter.Password}; Database={parametter.DatabaseName}";
                var query = $"update {parametter.TableName} set UserName = '{account.UserName}', Password = '{account.Password.ToMD5()}', Role = '{account.Role}' where UserName = '{oldAccount.UserName}' and Password ='{oldAccount.Password.ToMD5()}' and Role ='{oldAccount.Role}'";

                return DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }
    }

    public class AuthenticationMSSQLConnector : IAuthenticationConnector
    {
        public IDatabaseHelper DatabaseHelper { get; set; }

        public AuthenticationMSSQLConnector()
        {
            DatabaseHelper = new MSSQLHelper();
        }
        public bool CreateDatabaseIfNotExists(DatabaseParametter parametter)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};";
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
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"if not exists (select object_id from sys.tables where name = '{parametter.TableName}' and schema_name(schema_id) = 'dbo')" +
                    $"create table [{parametter.DatabaseName}].[dbo].[{parametter.TableName}]" +
                    $"([UserName] varchar(100) not null, [Password] varchar(45) not null, [Role] varchar(45) not null)";

                this.DatabaseHelper.ExecuteNonQuery(query);
                return true;
            }
            catch { return false; }
        }

        public bool Login(DatabaseParametter parametter, Account account)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"select count(*) from [{parametter.TableName}] where [UserName] = '{account.UserName}' and [Password] = '{account.Password.ToMD5()}' and [Role] = '{account.Role}'";

                return Convert.ToInt32(DatabaseHelper.ExecuteScalarQuery(query)) > 0;
            }
            catch { return false; }
        }

        public bool CheckAccountIsAvailable(DatabaseParametter parametter, Account account)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"select count(*) from [{parametter.TableName}] where [UserName] = '{account.UserName}'";

                return Convert.ToInt32(DatabaseHelper.ExecuteScalarQuery(query)) > 0;
            }
            catch { return false; }
        }

        public bool CreateAccount(DatabaseParametter parametter, Account account)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"insert into [{parametter.TableName}] values ('{account.UserName}', '{account.Password.ToMD5()}', '{account.Role}')";

                return DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }

        public bool UpdateAccount(DatabaseParametter parametter, Account account, Account oldAccount)
        {
            try
            {
                DatabaseHelper.ConnectionString = $"Server={parametter.ServerName},{parametter.Port};User Id={parametter.UserID};Password={parametter.Password};Database={parametter.DatabaseName}";
                var query = $"update [{parametter.TableName}] set [UserName] = '{account.UserName}', [Password] = '{account.Password.ToMD5()}', [Role] = '{account.Role}' where [UserName] = '{oldAccount.UserName}' and [Password] ='{oldAccount.Password}' and Role ='{oldAccount.Role}'";

                return DatabaseHelper.ExecuteNonQuery(query) >= 0;
            }
            catch { return false; }
        }
    }
}
