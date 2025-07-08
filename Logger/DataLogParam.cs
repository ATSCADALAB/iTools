using ATSCADA.iWinTools.Database;
using ATSCADA.ToolExtensions.Data;

namespace ATSCADA.iWinTools.Logger
{
    public class DataLogParam
    {
        public DatabaseParametter DatabaseLog { get; set; }

        public DataTool DataTimeRate { get; set; }

        public bool AllowLogWhenBad { get; set; }
    }
}
