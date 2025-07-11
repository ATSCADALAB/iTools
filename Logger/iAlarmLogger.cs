using ATSCADA.iWinTools.Alarm;
using ATSCADA.iWinTools.Database;
using ATSCADA.iWinTools.Email;
using ATSCADA.iWinTools.License;
using ATSCADA.ToolExtensions.Data;
using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using LicenseManager = ATSCADA.iWinTools.License.LicenseManager;

namespace ATSCADA.iWinTools.Logger
{
    public partial class iAlarmLogger : Component
    {
        private ITag emailTag;

        // ===== THÊM MỚI: Result Tag và Counter Tag =====
        private ITag alarmCounterTag;    // Tag đếm số alarm
        private ITag alarmResultTag;     // Tag result tổng hợp
        private int currentAlarmCount = 0;   // Biến đếm alarm hiện tại
        private readonly object lockObject = new object(); // Thread safety

        // THÊM MỚI: Tracking trạng thái từng tag để tránh double count
        private Dictionary<string, bool> tagAlarmStates = new Dictionary<string, bool>();
        // ================================================

        private List<AlarmSettingsItem> alarmSettingsItems;
        private IAlarmLogConnector logConnector;
        private EmailCore emailCore;
        private iDriver driver;

        #region Properties - Giữ nguyên các property cũ

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

        #endregion

        #region THÊM MỚI: Properties cho Result và Counter

        [Category("ATSCADA Settings")]
        [Description("Select tag for alarm counter (số lượng alarm đang active).")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string AlarmCounterTagName { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Select tag for alarm result (trạng thái tổng hợp ON/OFF).")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string AlarmResultTagName { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Enter value to write to Result tag when any alarm is active (e.g. \"1\", \"True\", \"ON\").")]
        public string ValueOnAlarm { get; set; } = "1";

        [Category("ATSCADA Settings")]
        [Description("Enter value to write to Result tag when no alarm is active (e.g. \"0\", \"False\", \"OFF\").")]
        public string ValueOffAlarm { get; set; } = "0";

        #endregion

        #region Constructor - Giữ nguyên

        public iAlarmLogger()
        {
            InitializeComponent();
        }

        public iAlarmLogger(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        #endregion

        #region Driver Events - Chỉnh sửa để thêm tính năng mới

        private void Driver_ConstructionCompleted()
        {
            // ===== KIỂM TRA LICENSE TRƯỚC TIÊN =====
            if (!CheckLicense())
            {
                return; // Dừng khởi tạo nếu license không hợp lệ
            }
            // =======================================

            // PROTECTION: Tránh multiple calls
            if (this.alarmSettingsItems != null)
            {
                LogSystemEvent("Warning: Driver_ConstructionCompleted called multiple times - ignoring");
                return;
            }

            try
            {
                // Khởi tạo các tag (bao gồm các tag mới)
                InitializeTags();

                // Khởi tạo database và email (giữ nguyên)
                var settingsConnector = AlarmSettingsConnectorFactory.GetConnector(DatabaseSettings.DatabaseType);
                if (settingsConnector.CreateDatabaseIfNotExists(DatabaseSettings))
                    if (settingsConnector.CreateTableIfNotExists(DatabaseSettings))
                        this.alarmSettingsItems = settingsConnector.GetAlarmSettingsItems(DatabaseSettings);

                if (this.alarmSettingsItems == null || this.alarmSettingsItems.Count == 0)
                {
                    LogSystemEvent("No alarm settings found - alarm logger inactive");
                    return;
                }

                this.logConnector = AlarmLogConnectorFactory.GetConnector(DatabaseLog.DatabaseType);
                this.emailCore = new EmailCore(EmailConfig);

                // QUAN TRỌNG: Khôi phục trạng thái TRƯỚC khi tạo alarm tags
                InitializeAlarmSystem();

                // Tạo alarm tags với logic mới
                CreateAlarmTag();

                LogSystemEvent($"AlarmLogger initialized successfully with {this.alarmSettingsItems.Count} alarm configurations");

                // Hiển thị license info khi khởi tạo thành công
                ShowLicenseInfo();
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Critical error in Driver_ConstructionCompleted: {ex.Message}");
                // Ensure safe state
                currentAlarmCount = 0;
                if (alarmCounterTag != null) UpdateAlarmCounter(0);
                if (alarmResultTag != null) UpdateAlarmResult(false);
            }
        }

        // ===== THÊM MỚI: LICENSE MANAGEMENT =====

        /// <summary>
        /// Kiểm tra license khi khởi tạo
        /// </summary>
        private bool CheckLicense()
        {
            try
            {
                if (!LicenseManager.IsLicenseValid())
                {
                    // License hết hạn - hiển thị thông báo và dừng
                    string errorMessage = LicenseManager.GetLicenseWarningMessage();

                    LogSystemEvent("❌ LICENSE EXPIRED - AlarmLogger disabled");

                    // Hiển thị MessageBox thông báo
                    MessageBox.Show(errorMessage, "License Expired - ATSCADA AlarmLogger",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }

                // License hợp lệ - kiểm tra cảnh báo gần hết hạn
                string warningMessage = LicenseManager.GetLicenseWarningMessage();
                if (!string.IsNullOrEmpty(warningMessage))
                {
                    LogSystemEvent($"⚠️ LICENSE WARNING - {LicenseManager.GetDaysRemaining()} days remaining");

                    // Hiển thị warning (không block)
                    MessageBox.Show(warningMessage, "License Warning - ATSCADA AlarmLogger",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error checking license: {ex.Message}");

                MessageBox.Show($"License validation error: {ex.Message}\n\nAlarmLogger will be disabled.",
                    "License Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }

        /// <summary>
        /// Hiển thị thông tin license
        /// </summary>
        private void ShowLicenseInfo()
        {
            try
            {
                var licenseInfo = LicenseManager.GetLicenseInfo();
                LogSystemEvent($"✅ LICENSE OK - {licenseInfo.ProductName} valid until {licenseInfo.ExpiryDate:dd/MM/yyyy} ({licenseInfo.DaysRemaining} days remaining)");
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error showing license info: {ex.Message}");
            }
        }

        // THÊM MỚI: Khởi tạo các tag
        private void InitializeTags()
        {
            // Email tag (giữ nguyên)
            this.emailTag = this.driver.GetTagByName(EmailTagName);

            // THÊM MỚI: Alarm counter tag
            if (!string.IsNullOrEmpty(AlarmCounterTagName))
            {
                this.alarmCounterTag = this.driver.GetTagByName(AlarmCounterTagName);
            }

            // THÊM MỚI: Alarm result tag
            if (!string.IsNullOrEmpty(AlarmResultTagName))
            {
                this.alarmResultTag = this.driver.GetTagByName(AlarmResultTagName);
            }

            // KHÔNG CẦN DataTool nữa - chỉ dùng string trực tiếp
        }

        // THÊM MỚI: Khởi tạo hệ thống alarm
        private void InitializeAlarmSystem()
        {
            lock (lockObject)
            {
                // BỎ phần RestoreAlarmCountFromTag() - KHÔNG tin vào giá trị cũ

                // LUÔN LUÔN tính toán lại từ trạng thái thực tế của các tag
                RecalculateAlarmCountFromAlarmTags();

                // Log trạng thái khởi động
                LogSystemEvent($"System started - Calculated alarm count from actual tag states: {currentAlarmCount}");
            }
        }

        // THÊM MỚI: Khôi phục số đếm từ tag
        private void RestoreAlarmCountFromTag()
        {
            // BỎ HOÀN TOÀN method này - không tin vào giá trị cũ
            // Luôn tính toán từ trạng thái thực tế
        }

        // THÊM MỚI: Tính lại số alarm từ trạng thái thực tế của các TAG TRACKING
        private void RecalculateAlarmCountFromAlarmTags()
        {
            try
            {
                int calculatedAlarmCount = 0;
                tagAlarmStates.Clear(); // Reset dictionary

                LogSystemEvent("Starting alarm count calculation from actual tag values...");

                if (this.alarmSettingsItems != null)
                {
                    foreach (var alarmSettingsItem in this.alarmSettingsItems)
                    {
                        string tagKey = alarmSettingsItem.AlarmParametter.Tracking;

                        try
                        {
                            // TRỰC TIẾP đọc và so sánh giá trị từ tracking, high, low tags
                            bool isTagInAlarm = CheckTagAlarmStatus(alarmSettingsItem.AlarmParametter);

                            // Lưu trạng thái vào dictionary
                            tagAlarmStates[tagKey] = isTagInAlarm;

                            // Đếm số tag đang alarm
                            if (isTagInAlarm)
                            {
                                calculatedAlarmCount++;
                                var startupAlarmEvent = CreateStartupAlarmEvent(alarmSettingsItem.AlarmParametter);
                                // Log vào database
                                LogAlarm(startupAlarmEvent);

                                // Send email
                                var recipients = alarmSettingsItem.GetRecipientList();
                                SendEmail(startupAlarmEvent, recipients);
                                LogSystemEvent($"Tag {tagKey} is in alarm state");
                            }
                            else
                            {
                                LogSystemEvent($"Tag {tagKey} is normal");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogSystemEvent($"Error checking tag {tagKey}: {ex.Message}");
                            tagAlarmStates[tagKey] = false; // Assume normal nếu lỗi
                        }
                    }
                }

                // CẬP NHẬT biến internal trước
                currentAlarmCount = calculatedAlarmCount;

                // SAU ĐÓ mới update các tag
                UpdateAlarmCounter(currentAlarmCount);
                UpdateAlarmResult(currentAlarmCount > 0);

                LogSystemEvent($"Alarm calculation completed: {calculatedAlarmCount} tags in alarm, Counter updated, Result = {(currentAlarmCount > 0 ? ValueOnAlarm : ValueOffAlarm)}");
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Critical error in alarm calculation: {ex.Message}");
                currentAlarmCount = 0;
                UpdateAlarmCounter(0);
                UpdateAlarmResult(false);
            }
        }
        // THÊM MỚI: Tạo AlarmStatusChangedEventArgs cho startup alarm
        private AlarmStatusChangedEventArgs CreateStartupAlarmEvent(AlarmParametter alarmParam)
        {
            try
            {
                // Đọc giá trị hiện tại
                var trackingTag = this.driver.GetTagByName(alarmParam.Tracking);
                if (!TryParseDouble(trackingTag.Value, out double trackingValue))
                    trackingValue = 0;

                if (!ParseAlarmLevel(alarmParam.LowLevel, "LowLevel", alarmParam.Tracking, out double lowValue))
                    lowValue = 0;

                if (!ParseAlarmLevel(alarmParam.HighLevel, "HighLevel", alarmParam.Tracking, out double highValue))
                    highValue = 0;

                // Xác định loại alarm
                Condition alarmCondition;
                if (Math.Abs(highValue - lowValue) < 0.0001)
                {
                    // SetPoint alarm
                    alarmCondition = new Condition(AlarmStatus.Alarm, "Startup Alarm");
                }
                else if (highValue < lowValue)
                {
                    // Configuration error
                    alarmCondition = new Condition(AlarmStatus.SetPoint, "Startup SetPoint Error");
                }
                else if (trackingValue <= lowValue)
                {
                    alarmCondition = new Condition(AlarmStatus.LowAlarm, "Startup Low Alarm");
                }
                else if (trackingValue >= highValue)
                {
                    alarmCondition = new Condition(AlarmStatus.HighAlarm, "Startup High Alarm");
                }
                else
                {
                    alarmCondition = new Condition(AlarmStatus.Normal, "Normal");
                }

                // Tạo AlarmItem
                var alarmItem = new AlarmItem()
                {
                    TrackingName = alarmParam.Tracking,
                    TrackingAlias = alarmParam.Alias ?? alarmParam.Tracking,
                    TrackingValue = trackingValue,
                    LowLevel = lowValue,
                    HighLevel = highValue
                };

                // Tạo event args
                return new AlarmStatusChangedEventArgs()
                {
                    TimeStamp = DateTime.Now,
                    Condition = alarmCondition,
                    AlarmItem = alarmItem
                };
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error creating startup alarm event for {alarmParam.Tracking}: {ex.Message}");

                // Return minimal event nếu lỗi
                return new AlarmStatusChangedEventArgs()
                {
                    TimeStamp = DateTime.Now,
                    Condition = new Condition(AlarmStatus.Alarm, "Startup Alarm - Error"),
                    AlarmItem = new AlarmItem()
                    {
                        TrackingName = alarmParam.Tracking,
                        TrackingAlias = alarmParam.Alias ?? alarmParam.Tracking,
                        TrackingValue = 0,
                        LowLevel = 0,
                        HighLevel = 0
                    }
                };
            }
        }

        // THÊM MỚI: Kiểm tra trạng thái alarm của 1 tag cụ thể
        private bool CheckTagAlarmStatus(AlarmParametter alarmParam)
        {
            try
            {
                // Đọc trực tiếp từ driver
                var trackingTag = this.driver.GetTagByName(alarmParam.Tracking);
                if (trackingTag == null)
                {
                    LogSystemEvent($"Warning: Tracking tag {alarmParam.Tracking} not found");
                    return false; // Tag không tồn tại = không alarm
                }

                // CHECK: Tag status - cho phép Good và Uncertain
                if (trackingTag.Status != "Good" && trackingTag.Status != "Uncertain")
                {
                    LogSystemEvent($"Warning: Tag {alarmParam.Tracking} status = {trackingTag.Status}");
                    return false; // Tag lỗi = không alarm
                }

                // ROBUST Parse giá trị tracking
                if (!TryParseDouble(trackingTag.Value, out double trackingValue))
                {
                    LogSystemEvent($"Warning: Cannot parse tracking value '{trackingTag.Value}' for {alarmParam.Tracking}");
                    return false; // Không parse được = không alarm
                }

                // Parse low level với validation
                if (!ParseAlarmLevel(alarmParam.LowLevel, "LowLevel", alarmParam.Tracking, out double lowValue))
                    return false;

                // Parse high level với validation  
                if (!ParseAlarmLevel(alarmParam.HighLevel, "HighLevel", alarmParam.Tracking, out double highValue))
                    return false;

                // LOGIC GIỐNG TRONG AlarmTag.CheckAlarm() - với edge case handling
                if (Math.Abs(highValue - lowValue) < 0.0001) // Float comparison
                {
                    // SetPoint alarm
                    return (Math.Abs(trackingValue - highValue) < 0.0001);
                }
                else if (highValue < lowValue)
                {
                    // Configuration error
                    LogSystemEvent($"Warning: Config error for {alarmParam.Tracking} - High({highValue}) < Low({lowValue})");
                    return true; // Coi như alarm
                }
                else
                {
                    // Range alarm
                    return (trackingValue <= lowValue || trackingValue >= highValue);
                }
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error checking alarm status for {alarmParam.Tracking}: {ex.Message}");
                return false; // Lỗi = assume normal
            }
        }

        // THÊM MỚI: Robust number parsing
        private bool TryParseDouble(string value, out double result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Trim whitespace
            value = value.Trim();

            // Try standard parsing first
            if (double.TryParse(value, out result))
                return true;

            // Try with invariant culture (for different decimal separators)
            if (double.TryParse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out result))
                return true;

            return false;
        }

        // THÊM MỚI: Parse alarm level (constant or tag)
        private bool ParseAlarmLevel(string levelParam, string levelName, string trackingTagName, out double value)
        {
            value = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(levelParam))
                {
                    LogSystemEvent($"Warning: {levelName} is empty for {trackingTagName}");
                    return false;
                }

                // Check if it's a constant value (quoted)
                if (levelParam.StartsWith("\"") && levelParam.EndsWith("\"") && levelParam.Length > 2)
                {
                    // Constant value - remove quotes
                    string constantValue = levelParam.Substring(1, levelParam.Length - 2);
                    if (!TryParseDouble(constantValue, out value))
                    {
                        LogSystemEvent($"Warning: Cannot parse {levelName} constant '{constantValue}' for {trackingTagName}");
                        return false;
                    }
                    return true;
                }
                else
                {
                    // Tag value
                    var levelTag = this.driver.GetTagByName(levelParam);
                    if (levelTag == null)
                    {
                        LogSystemEvent($"Warning: {levelName} tag '{levelParam}' not found for {trackingTagName}");
                        return false;
                    }

                    if (levelTag.Status != "Good" && levelTag.Status != "Uncertain")
                    {
                        LogSystemEvent($"Warning: {levelName} tag '{levelParam}' status = {levelTag.Status} for {trackingTagName}");
                        return false;
                    }

                    if (!TryParseDouble(levelTag.Value, out value))
                    {
                        LogSystemEvent($"Warning: Cannot parse {levelName} tag value '{levelTag.Value}' for {trackingTagName}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error parsing {levelName} '{levelParam}' for {trackingTagName}: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Alarm Management - Chỉnh sửa để thêm logic mới

        private void CreateAlarmTag()
        {
            foreach (var alarmSettingsItem in this.alarmSettingsItems)
            {
                var alarmTag = new AlarmTag(this.driver, alarmSettingsItem.AlarmParametter);
                var recipients = alarmSettingsItem.GetRecipientList();

                alarmTag.StatusChanged += (sender, e) =>
                {
                    // CHỈNH SỬA: Thêm logic xử lý Result và Counter
                    HandleAlarmStatusChanged(e, recipients);

                    // Giữ nguyên: Log và Email
                    LogAlarm(e);
                    SendEmail(e, recipients);
                };

                // THÊM MỚI: Check alarm ngay khi vừa tạo (startup check)
                PerformInitialAlarmCheck(alarmTag, recipients);
            }
        }

        // THÊM MỚI: Kiểm tra alarm ban đầu khi startup
        private void PerformInitialAlarmCheck(AlarmTag alarmTag, List<string> recipients)
        {
            try
            {
                // Đợi một chút để tag ổn định
                System.Threading.Thread.Sleep(100);

                // Force check alarm với giá trị hiện tại
                alarmTag.CheckAlarm(true);

                LogSystemEvent($"Initial alarm check completed for tag: {alarmTag.Parametter.Tracking}");
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error in initial alarm check for {alarmTag.Parametter.Tracking}: {ex.Message}");
            }
        }

        // THÊM MỚI: Xử lý thay đổi trạng thái alarm
        private void HandleAlarmStatusChanged(AlarmStatusChangedEventArgs e, List<string> recipients)
        {
            lock (lockObject)
            {
                string tagKey = e.AlarmItem.TrackingName;
                bool isCurrentlyInAlarm = e.Condition.Status != AlarmStatus.Normal;

                // Kiểm tra trạng thái trước đó của tag này
                bool wasInAlarm = tagAlarmStates.ContainsKey(tagKey) ? tagAlarmStates[tagKey] : false;

                // Cập nhật trạng thái mới
                tagAlarmStates[tagKey] = isCurrentlyInAlarm;

                // Logic count: chỉ count theo TAG, không theo loại alarm
                if (isCurrentlyInAlarm && !wasInAlarm)
                {
                    // Tag chuyển từ Normal → Alarm (bất kỳ loại nào)
                    currentAlarmCount++;
                    UpdateAlarmCounter(currentAlarmCount);
                    UpdateAlarmResult(true);
                    // Nếu là alarm đầu tiên, bật Result
                    if (currentAlarmCount == 1)
                    {
                        
                        LogSystemEvent($"Alarm System ACTIVATED - Tag: {tagKey}, Type: {e.Condition.Message}");
                    }
                    else
                    {
                        LogSystemEvent($"Additional alarm - Tag: {tagKey}, Type: {e.Condition.Message}, Total: {currentAlarmCount}");
                    }
                }
                else if (!isCurrentlyInAlarm && wasInAlarm)
                {
                    // Tag chuyển từ Alarm → Normal
                    if (currentAlarmCount > 0)
                    {
                        currentAlarmCount--;
                        UpdateAlarmCounter(currentAlarmCount);

                        // Nếu không còn alarm nào, tắt Result
                        if (currentAlarmCount == 0)
                        {
                            //UpdateAlarmResult(false);
                            LogSystemEvent($"Alarm System DEACTIVATED - Tag: {tagKey} cleared, All alarms resolved");
                        }
                        else
                        {
                            LogSystemEvent($"Alarm cleared - Tag: {tagKey}, Remaining: {currentAlarmCount}");
                        }
                    }
                }
                else if (isCurrentlyInAlarm && wasInAlarm)
                {
                    // Tag chuyển từ alarm type này sang alarm type khác (High→Low, Low→High)
                    LogSystemEvent($"Alarm type changed - Tag: {tagKey}, New type: {e.Condition.Message}, Count unchanged: {currentAlarmCount}");
                    // KHÔNG thay đổi count - vì cùng 1 tag
                }
                // Trường hợp (!isCurrentlyInAlarm && !wasInAlarm) = không làm gì
            }
        }

        #endregion

        #region THÊM MỚI: Tag Updates

        private void UpdateAlarmCounter(int count)
        {
            try
            {
                if (this.alarmCounterTag != null)
                {
                    this.alarmCounterTag.ASynWrite(count.ToString());
                }
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error updating alarm counter: {ex.Message}");
            }
        }

        private void UpdateAlarmResult(bool hasAlarms)
        {
            try
            {
                if (this.alarmResultTag != null)
                {
                    // SỬ DỤNG STRING TRỰC TIẾP thay vì DataTool
                    string valueToWrite = hasAlarms ? ValueOnAlarm : ValueOffAlarm;
                    this.alarmResultTag.ASynWrite(valueToWrite);
                }
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error updating alarm result: {ex.Message}");
            }
        }

        private void LogSystemEvent(string message)
        {
            try
            {
                // Log system events (có thể mở rộng để ghi vào database)
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] AlarmLogger: {message}");
            }
            catch
            {
                // Ignore logging errors
            }
        }

        #endregion

        #region Giữ nguyên: LogAlarm và SendEmail

        private void LogAlarm(AlarmStatusChangedEventArgs e)
        {
            if (this.logConnector.CreateDatabaseIfNotExists(DatabaseLog))
                if (this.logConnector.CreateTableIfNotExists(DatabaseLog))
                    this.logConnector.InsertAlarm(DatabaseLog, e);
        }

        private void SendEmail(AlarmStatusChangedEventArgs e, List<string> recipients)
        {
            if (recipients is null && (this.emailTag is null)) return;

            // CHỈNH SỬA: Thêm thông tin Counter vào email
            var message = $" DateTime: {e.TimeStamp}\n Tag: {e.AlarmItem.TrackingName}\n Value: {e.AlarmItem.TrackingValue}\n" +
                $" High Level: {e.AlarmItem.HighLevel}\n Low Level: {e.AlarmItem.LowLevel}\n Status: {e.Condition.Message}\n" +
                $" Total Active Alarms: {currentAlarmCount}\n System Status: {(currentAlarmCount > 0 ? "ALARM ACTIVE" : "NORMAL")}";

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

        #endregion

        #region THÊM MỚI: Public Methods

        /// <summary>
        /// Lấy số lượng alarm hiện tại
        /// </summary>
        public int GetCurrentAlarmCount()
        {
            lock (lockObject)
            {
                return currentAlarmCount;
            }
        }

        /// <summary>
        /// Kiểm tra xem có alarm nào đang active không
        /// </summary>
        public bool HasActiveAlarms()
        {
            lock (lockObject)
            {
                return currentAlarmCount > 0;
            }
        }

        /// <summary>
        /// Reset alarm counter (sử dụng cẩn thận)
        /// </summary>
        public void ResetAlarmCounter()
        {
            lock (lockObject)
            {
                currentAlarmCount = 0;
                UpdateAlarmCounter(0);
                UpdateAlarmResult(false);
                LogSystemEvent("Alarm counter manually reset");
            }
        }

        /// <summary>
        /// THÊM MỚI: Đồng bộ lại trạng thái alarm từ các tag thực tế
        /// </summary>
        public void SynchronizeAlarmState()
        {
            lock (lockObject)
            {
                LogSystemEvent("Manual synchronization requested");
                RecalculateAlarmCountFromAlarmTags();
                UpdateAlarmResult(currentAlarmCount > 0);
            }
        }

        /// <summary>
        /// THÊM MỚI: Force check tất cả alarm tags ngay lập tức
        /// </summary>
        public void ForceCheckAllAlarms()
        {
            try
            {
                LogSystemEvent("Force check all alarms requested");

                if (this.alarmSettingsItems != null)
                {
                    foreach (var alarmSettingsItem in this.alarmSettingsItems)
                    {
                        var tempAlarmTag = new AlarmTag(this.driver, alarmSettingsItem.AlarmParametter);

                        // Force check với raise event = true
                        tempAlarmTag.CheckAlarm(true);

                        // Đợi một chút giữa các check
                        System.Threading.Thread.Sleep(10);
                    }
                }

                LogSystemEvent("Force check all alarms completed");
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error in force check all alarms: {ex.Message}");
            }
        }

        /// <summary>
        /// THÊM MỚI: Cleanup resources khi dispose
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    // Cleanup dictionary
                    tagAlarmStates?.Clear();

                    // Log final state
                    LogSystemEvent($"AlarmLogger disposed - Final count: {currentAlarmCount}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in dispose: {ex.Message}");
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// THÊM MỚI: Health check cho alarm system
        /// </summary>
        public bool IsSystemHealthy()
        {
            try
            {
                // KIỂM TRA LICENSE TRƯỚC TIÊN
                if (!LicenseManager.IsLicenseValid())
                {
                    return false; // License hết hạn = system không healthy
                }

                // Check basic components
                if (this.driver == null) return false;
                if (this.alarmSettingsItems == null || this.alarmSettingsItems.Count == 0) return false;

                // Check essential tags
                if (!string.IsNullOrEmpty(AlarmCounterTagName) && this.alarmCounterTag == null) return false;
                if (!string.IsNullOrEmpty(AlarmResultTagName) && this.alarmResultTag == null) return false;

                // Check if count matches reality (basic sanity check)
                if (currentAlarmCount < 0 || currentAlarmCount > this.alarmSettingsItems.Count) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// THÊM MỚI: Lấy thông tin license
        /// </summary>
        public LicenseInfo GetLicenseInfo()
        {
            return LicenseManager.GetLicenseInfo();
        }

        /// <summary>
        /// THÊM MỚI: Kiểm tra license có hợp lệ không
        /// </summary>
        public bool IsLicenseValid()
        {
            return LicenseManager.IsLicenseValid();
        }

        /// <summary>
        /// THÊM MỚI: Lưu trạng thái vào database để khôi phục sau
        /// </summary>
        public void SaveAlarmStateToDatabase()
        {
            try
            {
                // Có thể mở rộng để lưu vào bảng riêng trong database
                // Ví dụ: INSERT INTO alarm_system_state (timestamp, alarm_count, result_value)
                LogSystemEvent($"Saving alarm state - Count: {currentAlarmCount}, Result: {(currentAlarmCount > 0 ? ValueOnAlarm : ValueOffAlarm)}");
            }
            catch (Exception ex)
            {
                LogSystemEvent($"Error saving alarm state: {ex.Message}");
            }
        }

        #endregion
    }
}