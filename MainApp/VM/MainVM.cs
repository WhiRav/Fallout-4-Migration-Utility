using MainApp.Models;
using Microsoft.Win32;
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Xml.Linq;

namespace MainApp.VM
{
    partial class MainVM : DefaultVM
    {
        #region MemoryBound

        private URLInfo _urlInfo = new() { Fallout4URL = @"c:\Games\Fallout 4", ModOrganizerURL = @"c:\Games\Fallout 4\MO" };

        private DelegateCommand? _selectArchives;
        private DelegateCommand? _selectDestination;
        private DelegateCommand? _removeEntry;
        private DelegateCommand? _runUnzip;

        private double progressValue = 0;

        #endregion

        #region ViewBound

        public string? ArchiveURL { get => _urlInfo.ArchiveURL; set { _urlInfo.ArchiveURL = value; OnPropertyChanged(); } }
        public string? Fallout4URL { get => _urlInfo.Fallout4URL; set { _urlInfo.Fallout4URL = value; OnPropertyChanged(); } }
        public string? ModOrganizerURL { get => _urlInfo.ModOrganizerURL; set { _urlInfo.ModOrganizerURL = value; OnPropertyChanged(); } }

        public ICommand SelectArchives => _selectArchives ?? new DelegateCommand(PerformSelectionWithLoad);
        public ICommand SelectDestination => _selectDestination ?? new DelegateCommand(PerformSelection);
        public ICommand RemoveEntry => _removeEntry ?? new DelegateCommand(PerformRemove);
        public ICommand RunUnzip => _runUnzip ??= new DelegateCommand(PerformUnzip);

        public double ProgressValue { get => progressValue; set { progressValue = value; OnPropertyChanged(); } }

        #endregion

        private void PerformSelectionWithLoad(object _)
        {
            OpenFolderDialog dlg = new();
            dlg.ShowDialog();
            if (!Directory.Exists(dlg.FolderName))
            {
                ArchiveURL = "Invalid Path";
                return;
            }
            ArchiveURL = dlg.FolderName;
            string[] filenames = Directory.GetFiles(ArchiveURL, "*.7z");
            Files.Clear();
            for (ushort i = 1; i < filenames.Length; ++i)
                Files.Add(new() { Id = i, Folder = FolderSelection.Mods, FileName = filenames[i] });
        }

        private void PerformSelection(object sender)
        {
            OpenFolderDialog dlg = new();
            dlg.ShowDialog();
            if (sender.ToString() == "Fo4")
                Fallout4URL = Path.IsPathRooted(dlg.FolderName) ? dlg.FolderName : "Invalid Path";
            else
                ModOrganizerURL = Path.IsPathRooted(dlg.FolderName) ? dlg.FolderName : "Invalid Path";
        }

        private void PerformRemove(object obj)
        {
            Files.Where(node => node.Id > ((FileNode)obj).Id).ForEach(node => --node.Id);
            OnPropertyChanged(nameof(Files));
            Files.Remove(obj as FileNode);
        }

        private async void PerformUnzip(object _)
        {
            if (!_urlInfo.IsReady)
                return;
            ProgressValue = 5;
            double step = 90 / Files.Count;
            Dictionary<FolderSelection, string?> foldersLocation = new()
            {
                {FolderSelection.Mods, _urlInfo.ModOrganizerURL + @"\mods" },
                {FolderSelection.Profiles, _urlInfo.ModOrganizerURL + @"\profiles\Default" },
                {FolderSelection.Fallout, _urlInfo.Fallout4URL }
            };
            await Task.Run(() => Parallel.ForEach(Files.Where(node => node.Folder != FolderSelection.NONE), node =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                SevenZipArchive.Open(new FileInfo(node.FileName)).ExtractToDirectory(foldersLocation[node.Folder]);
                ProgressValue += step;
            }));
            ProgressValue = 100;
            Thread.Sleep(100);
            ProgressValue = 0;
        }


        private ObservableCollection<FileNode> _files = [];
        public ObservableCollection<FileNode> Files { get => _files; set { _files = value; OnPropertyChanged(); } }


    }
}
