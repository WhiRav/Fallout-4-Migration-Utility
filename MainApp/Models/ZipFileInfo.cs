using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Models
{
    internal enum FolderSelection
    {
        Mods = 1,
        Profiles = 2,
        Fallout = 3,
        NONE = 0
    }

    /// <summary>
    /// This class contains info about loaded zip files and their destination
    /// </summary>
    internal class FileNode
    {
        public ushort Id { get; set; } = 0;
        public FolderSelection Folder { get; set; } = FolderSelection.NONE;
        public string? FileName { get; set; }
    }
}
