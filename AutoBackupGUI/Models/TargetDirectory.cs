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
            this.FullPath = Path.GetFullPath(path);
            this.Parent = parent;
        }

        public override string ToString() => this.FullPath;
    }
}
