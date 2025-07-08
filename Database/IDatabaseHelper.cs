using System.Data;

namespace ATSCADA.iWinTools.Database
{
    public interface IDatabaseHelper
    {
        string ConnectionString { get; set; }

        int ExecuteNonQuery(string query);

        int ExecuteNonQuery(string query, params object[] parameters);

        DataTable ExecuteQuery(string query);

        object ExecuteScalarQuery(string query);
    }
}
