using ATSCADA.iWinTools.Database;
using ATSCADA.ToolExtensions.Data;
using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ATSCADA.iWinTools.Logger
{
    [ToolboxBitmap(typeof(System.Windows.Forms.DataGridView))]
    public partial class iDataLogger : Component
    {
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
        [Description("Settings of database.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DatabaseParametter DatabaseLog { get; set; } = new DatabaseParametter()
        {
            DatabaseType = DatabaseType.MySQL,
            ServerName = "localhost",
            UserID = "root",
            Password = "101101",
            DatabaseName = "ATSCADA",
            TableName = "datalog",
            Port = 3306
        };

        [Category("ATSCADA Settings")]
        [Description("Choose tags for data logging.")]
        [Editor(typeof(DataLoggerSettingsEditor), typeof(UITypeEditor))]
        public string Collection { get; set; } = "";

        [Category("ATSCADA Settings")]
        [Description("Timer: Making datalog after an amount of time.\nEvent: Making datalog after tag value has changed.")]
        public UpdateType UpdateType { get; set; } = UpdateType.Timer;

        [Category("ATSCADA Settings")]
        [Description("Enter timerate to log data , the value will be written between 2 quotation marks(\"\").\nIf select from SmartTag editor, the high level will by the value of tag is selected.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string LoggingTimerRate { get; set; }       

        [Category("ATSCADA Settings")]
        [Description("Allow log when status BAD.")]
        public bool AllowLogWhenBad { get; set; }

        public iDataLogger()
        {
            InitializeComponent();
        }

        public iDataLogger(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private void Driver_ConstructionCompleted()
        {
            var dataLogParam = new DataLogParam()
            {
                DatabaseLog = DatabaseLog,
                DataTimeRate = new DataTool(this.driver, LoggingTimerRate),
                AllowLogWhenBad = AllowLogWhenBad
            };

            var actionDataLogger = ActionDataLoggerFactory.GetActionDataLogger(UpdateType, dataLogParam);
            actionDataLogger.DataLoggerItems = Deserialization();
            actionDataLogger.Start();
        }

        private List<DataLoggerItem> Deserialization()
        {
            var dataSerialization = Collection.Split('|');
            var countItems = dataSerialization.Length;
            if (countItems == 0) return null;

            var items = new List<DataLoggerItem>();

            for (int index = 0; index < countItems; index++)
            {
                var data = dataSerialization[index].Split('&');
                if (data.Length != 3) continue;

                var selectedTag = this.driver.GetTagByName(data[0]);
                var alias = data[1];
                var resultTryParse = bool.TryParse(data[2], out bool isTrigger);

                if (selectedTag == null || !resultTryParse) continue;

                items.Add(new DataLoggerItem()
                {
                    SelectedTag = selectedTag,
                    Alias = alias,
                    IsTrigger = isTrigger
                });
            }

            return items;
        }
    }
}
