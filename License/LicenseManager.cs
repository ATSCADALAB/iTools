using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ATSCADA.iWinTools.License
{
    /// <summary>
    /// Hệ thống License cho iAlarmLogger
    /// </summary>
    public static class LicenseManager
    {
        private static readonly string LICENSE_KEY = "ATSCADA-ALARM-2025";
        private static readonly DateTime EXPIRY_DATE = new DateTime(2025, 7, 15, 23, 59, 59); // 15/07/2025
        private static readonly string HASH_SALT = "iWinTools@2025#AlarmLogger";

        private static bool? _isLicenseValidCache = null;
        private static DateTime _lastCheckTime = DateTime.MinValue;

        /// <summary>
        /// Kiểm tra license có hợp lệ không
        /// </summary>
        public static bool IsLicenseValid()
        {
            try
            {
                // Cache result trong 1 phút để tránh check liên tục
                if (_isLicenseValidCache.HasValue &&
                    DateTime.Now.Subtract(_lastCheckTime).TotalMinutes < 1)
                {
                    return _isLicenseValidCache.Value;
                }

                DateTime currentDate = DateTime.Now;

                // Kiểm tra ngày hết hạn
                if (currentDate > EXPIRY_DATE)
                {
                    _isLicenseValidCache = false;
                    _lastCheckTime = currentDate;
                    return false;
                }

                // Kiểm tra license key (optional - có thể thêm validation phức tạp hơn)
                bool keyValid = ValidateLicenseKey();

                _isLicenseValidCache = keyValid;
                _lastCheckTime = currentDate;

                return keyValid;
            }
            catch
            {
                return false; // Nếu có lỗi gì thì coi như license không hợp lệ
            }
        }

        /// <summary>
        /// Lấy thông tin license
        /// </summary>
        public static LicenseInfo GetLicenseInfo()
        {
            return new LicenseInfo
            {
                LicenseKey = LICENSE_KEY,
                ExpiryDate = EXPIRY_DATE,
                IsValid = IsLicenseValid(),
                DaysRemaining = GetDaysRemaining(),
                ProductName = "ATSCADA iWinTools - AlarmLogger",
                Version = "1.0.0"
            };
        }

        /// <summary>
        /// Lấy số ngày còn lại
        /// </summary>
        public static int GetDaysRemaining()
        {
            try
            {
                DateTime currentDate = DateTime.Now.Date;
                DateTime expiryDate = EXPIRY_DATE.Date;

                if (currentDate > expiryDate)
                    return 0;

                return (int)(expiryDate - currentDate).TotalDays;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Validate license key (có thể mở rộng thêm)
        /// </summary>
        private static bool ValidateLicenseKey()
        {
            try
            {
                // Simple validation - có thể thêm encryption/decryption phức tạp hơn
                if (string.IsNullOrEmpty(LICENSE_KEY))
                    return false;

                // Kiểm tra format cơ bản
                if (!LICENSE_KEY.StartsWith("ATSCADA-"))
                    return false;

                // Có thể thêm: kiểm tra hardware ID, MAC address, etc.
                // Hiện tại đơn giản thì return true
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tạo warning message khi gần hết hạn
        /// </summary>
        public static string GetLicenseWarningMessage()
        {
            var info = GetLicenseInfo();

            if (!info.IsValid)
            {
                return $"⚠️ LICENSE EXPIRED!\n\nProduct: {info.ProductName}\nExpired on: {info.ExpiryDate:dd/MM/yyyy}\n\nPlease contact support for license renewal.";
            }

            if (info.DaysRemaining <= 7)
            {
                return $"⚠️ LICENSE WARNING!\n\nProduct: {info.ProductName}\nExpires on: {info.ExpiryDate:dd/MM/yyyy}\nDays remaining: {info.DaysRemaining}\n\nPlease renew your license soon.";
            }

            return null; // Không có warning
        }

        /// <summary>
        /// Format license info để hiển thị
        /// </summary>
        public static string GetLicenseDisplayText()
        {
            var info = GetLicenseInfo();

            string status = info.IsValid ? "✅ Valid" : "❌ Expired";

            return $"License: {info.LicenseKey}\n" +
                   $"Product: {info.ProductName}\n" +
                   $"Status: {status}\n" +
                   $"Expires: {info.ExpiryDate:dd/MM/yyyy HH:mm:ss}\n" +
                   $"Days remaining: {info.DaysRemaining}";
        }
    }

    /// <summary>
    /// Thông tin license
    /// </summary>
    public class LicenseInfo
    {
        public string LicenseKey { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsValid { get; set; }
        public int DaysRemaining { get; set; }
        public string ProductName { get; set; }
        public string Version { get; set; }
    }

    /// <summary>
    /// Exception cho license issues
    /// </summary>
    public class LicenseException : Exception
    {
        public LicenseException(string message) : base(message) { }
        public LicenseException(string message, Exception innerException) : base(message, innerException) { }
    }
}