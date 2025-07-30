using System.Diagnostics;
using System.IO;

namespace FinTrackForWindows.Helpers
{
    public static class FileSaver
    {
        public static async Task<string> SaveReportToDocumentsAsync(byte[] fileBytes, string fileName)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string targetFolderPath = Path.Combine(documentsPath, "FinTrackReport");

            Directory.CreateDirectory(targetFolderPath);

            string fullFilePath = Path.Combine(targetFolderPath, fileName);

            await File.WriteAllBytesAsync(fullFilePath, fileBytes);

            return fullFilePath;
        }

        public static void OpenContainingFolder(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
        }
    }
}