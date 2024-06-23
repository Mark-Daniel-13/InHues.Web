using Microsoft.AspNetCore.Components.Forms;

namespace InHues.Components.Helpers
{
    public static class BrowserfileHelper
    {
        public static string GetBase64(string file) {
            if (string.IsNullOrEmpty(file) || !file.Contains(',')) return string.Empty;
            return file.Split(",")[1];
        }
        public static string GetFileType(IBrowserFile file) {
            if (string.IsNullOrEmpty(file.ContentType) || !file.ContentType.Contains('/')) return string.Empty;
            return file.ContentType.Split("/")[1];
        }
        public static byte[] GetByteArray(string file) {
            if (string.IsNullOrEmpty(file)) return null;
            return Convert.FromBase64String(GetBase64(file));
        }
    }
}
