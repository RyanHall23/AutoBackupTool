using Microsoft.Toolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AutoBackupGUI.Models
{
    [ObservableObject]
    public partial class BackupManager
    {
        [ObservableProperty]
        private ObservableCollection<TargetDirectory> sources;
        [ObservableProperty]
        private ObservableCollection<TargetDirectory> backups;

        #region Singleton
        private static BackupManager? instance;
        public static BackupManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new BackupManager();
                return instance;
            }
        }

        private BackupManager()
        {
            this.sources = new ObservableCollection<TargetDirectory>();
            this.backups = new ObservableCollection<TargetDirectory>();

            //this.AddSource(@"D:\Dev\~TESTING\source");
            //this.AddBackup(@"D:\Dev\~TESTING\dest");
        }
        #endregion

        public void Reset()
        {
            this.Sources = new();
            this.Backups = new();
        }

        public void AddSource(string path)
        {
            if (this.Sources.Select(source => source.FullPath).Contains(path))
                return;

            if (Directory.Exists(path))
                this.Sources.Add(new TargetDirectory(path));
        }
        public void AddBackup(string path)
        {
            if (this.Backups.Select(backup => backup.FullPath).Contains(path))
                return;

            if (Directory.Exists(path))
                this.Backups.Add(new TargetDirectory(path));
        }

        public void RunBackup()
        {
            if (this.Sources.Count == 0)
                throw new InvalidOperationException("Source directiories must be set before running a backup");
            if (this.Backups.Count == 0)
                throw new InvalidOperationException("Backup locations must be set before running a backup");

            foreach (TargetDirectory backup in Backups)
                foreach (TargetDirectory source in Sources)
                    Copy(new DirectoryInfo(source.FullPath), new DirectoryInfo(Path.Combine(backup.FullPath, source.DirName)));
        }
        private static void Copy(DirectoryInfo source, DirectoryInfo backup)
        {
            Directory.CreateDirectory(backup.FullName);

            foreach (FileInfo fi in source.GetFiles())
                fi.CopyTo(Path.Combine(backup.FullName, fi.Name), true);

            foreach (DirectoryInfo sourceSubDir in source.GetDirectories())
            {
                DirectoryInfo backupSubDir = backup.CreateSubdirectory(sourceSubDir.Name);
                Copy(sourceSubDir, backupSubDir);
            }
        }
    }
}
