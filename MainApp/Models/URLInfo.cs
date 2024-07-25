using System.IO;


namespace MainApp.Models
{
    /// <summary>
    /// Contains URL for app usage and method for checking if it ready for unpacking process
    /// </summary>
    internal class URLInfo
    {
        public string? ArchiveURL { get; set; }
        public string? Fallout4URL { get; set; }
        public string? ModOrganizerURL { get; set; }

        public bool IsReady => Directory.Exists(ArchiveURL) && Path.IsPathRooted(Fallout4URL) && Path.IsPathRooted(ModOrganizerURL);
    }
}
