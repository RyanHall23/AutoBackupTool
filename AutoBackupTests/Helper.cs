using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackupTests
{
    internal static class Helper
    {
        private const int MinNumFiles = 1;
        private const int MaxNumFiles = 5;

        public static void GenerateRandomFiles(string dir)
        {
            Random rnd = new();

            for (int i = 0; i < rnd.Next(MinNumFiles, MaxNumFiles); ++i)
                File.Create(Path.Combine(dir, Path.GetRandomFileName())).Dispose();
        }
        public static string GetNonExistentDir(string parent)
        {
            string dir = Path.GetRandomFileName();

            while (Directory.Exists(Path.Combine(parent, dir)))
                dir = Path.GetRandomFileName();

            return Path.Combine(parent, dir);
        }
    }
}
