using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoBackupGUI.Models
{
    public class TargetDirectory
    {
        public string FullPath { get; }
        public TargetDirectory? Parent { get; }

        public string DirName => Path.GetFileName(this.FullPath) ?? string.Empty;
        public IEnumerable<TargetDirectory> SubDirs => Directory.GetDirectories(this.FullPath).Select(subDir => new TargetDirectory(subDir, this));
        public IEnumerable<string> Files => Directory.GetFiles(this.FullPath);

        public TargetDirectory(string path, TargetDirectory? parent = null)
        {
            this.FullPath = path;
            this.Parent = parent;
        }
        public override string ToString()
        {
            string str = this.DirName;
            TargetDirectory? parent = this.Parent;

            while (parent != null)
            {
                str = Path.Combine(parent.DirName, str);
                parent = parent.Parent;
            }
            return str;
        }
    }

    public class BackupManager
    {
        public static List<TargetDirectory> Sources { get; private set; } = new();
        public static List<TargetDirectory> Backups { get; private set; } = new();

        public static void Reset()
        {
            Sources = new();
            Backups = new();
        }

        public static void AddSource(string path)
        {
            if (Directory.Exists(path))
                Sources.Add(new TargetDirectory(path));
        }
        public static void AddBackup(string path)
        {
            if (Directory.Exists(path))
                Backups.Add(new TargetDirectory(path));
        }

        public static void RunBackup()
        {
            if (Sources.Count == 0)
                throw new InvalidOperationException("Source directiories must be set before running a backup");
            if (Backups.Count == 0)
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
