using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FinTrack.Core
{
    public class SecureTokenStorage : ISecureTokenStorage
    {
        private readonly string _filePath;

        private static readonly byte[] s_entropy = Encoding.UTF8.GetBytes("E5A3B8B8_4A8C_4F1D_9F0B_2B3A7F9C1D0E"); // [TEST]

        public SecureTokenStorage()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "FinTrack");
            Directory.CreateDirectory(appFolder);
            _filePath = Path.Combine(appFolder, "user.token");
        }

        public void SaveToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            try
            {
                var encryptedToken = ProtectedData.Protect(Encoding.UTF8.GetBytes(token), s_entropy, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(_filePath, encryptedToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving token: {ex.Message}");
                throw;
            }
        }

        public string? GetToken()
        {
            if (!File.Exists(_filePath))
                return null;

            try
            {
                var encryptedBytes = File.ReadAllBytes(_filePath);
                var tokenBytes = ProtectedData.Unprotect(encryptedBytes, s_entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(tokenBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving token: {ex.Message}");
                ClearToken();
                return null;
            }
        }

        public void ClearToken()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    File.Delete(_filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error clearing token: {ex.Message}");
                }
            }
        }
    }
}
