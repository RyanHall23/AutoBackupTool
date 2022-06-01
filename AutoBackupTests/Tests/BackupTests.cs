using AutoBackupGUI.Models;

using System.Text;

namespace AutoBackupTests.Tests
{
    // Tests:
    // - Init
    //   - Should be able to add a backup location to a list of tracked backup locations
    //   - Should NOT be able to add a backup location that does not exist
    // - Parent directory checks
    //   - Should be able to run a backup to copy files from a source directory to a single backup location
    //   - Should be able to run a backup to copy files from a source directory to multiple backup locations
    //   - Should be able to run a backup to copy files from multiple sources to a single backup location
    //   - Should be able to run a backup to copy files from multiple sources to multiple backup locations
    // - Sub-directory checks
    //   - Should be able to run a backup to copy files from a source directory with sub-directories to a single backup location
    //   - Should be able to run a backup to copy files from a source directory with sub-directories to multiple backup locations
    // - Backup error tests
    //   - Should NOT be able to run a backup if source directories are not set
    //   - Should NOT be able to run a backup if backup locations are not set

    public class BackupTests
    {
        private static readonly string BaseDir = Path.GetFullPath(@".\Test\BackupTests");

        private static readonly string SourceDir = Path.Combine(BaseDir, "source");
        private static readonly string Source1 = Path.Combine(SourceDir, "source1");
        private static readonly string Source2 = Path.Combine(SourceDir, "source2");
        private static readonly string SourceWithSubDirs = Path.Combine(SourceDir, "sourceWithSubDirs");
        private static readonly string SourceWithSubDirs_Sub1 = Path.Combine(SourceWithSubDirs, "sub1");

        private static readonly string BackupDir = Path.Combine(BaseDir, "backup");
        private static readonly string Backup1 = Path.Combine(BackupDir, "backup1");
        private static readonly string Backup2 = Path.Combine(BackupDir, "backup2");

        public BackupTests()
        {
            CleanBeforeTest();
        }
        private void CleanBeforeTest()
        {
            BackupManager.Reset();
            ClearBackups();
            ClearTestSources();
            InitTestSources();
        }
        private void ClearBackups()
        {
            if (Directory.Exists(BackupDir))
                Directory.Delete(BackupDir, true);
        }
        private void ClearTestSources()
        {
            if (Directory.Exists(SourceDir))
                Directory.Delete(SourceDir, true);
        }
        private void InitTestSources()
        {
            Directory.CreateDirectory(Source1);
            Directory.CreateDirectory(Source2);
            Directory.CreateDirectory(SourceWithSubDirs_Sub1);

            Directory.CreateDirectory(Backup1);
            Directory.CreateDirectory(Backup2);

            Helper.GenerateRandomFiles(Source1);
            Helper.GenerateRandomFiles(Source2);
            Helper.GenerateRandomFiles(SourceWithSubDirs);
            Helper.GenerateRandomFiles(SourceWithSubDirs_Sub1);
        }

        private void VerifyBackup(TargetDirectory backup, TargetDirectory source)
        {
            // Verify source directory has been copied to backup
            Assert.Contains(source.DirName, backup.SubDirs.Select(dir => dir.DirName));

            // Verify that the copy of source contains all the expected files and sub directories
            TargetDirectory copy = backup.SubDirs.Where(dir => dir.DirName == source.DirName).Single();
            Assert.Equal(source.Files.Select(f => Path.GetFileName(f)), copy.Files.Select(f => Path.GetFileName(f)));
            Assert.Equal(source.SubDirs.Select(dir => dir.DirName), copy.SubDirs.Select(dir => dir.DirName));

            // Verify that the sub directories contain all the expected files and sub directories
            foreach (TargetDirectory subDir in source.SubDirs)
                VerifyBackup(copy, subDir);
        }

        [Fact(DisplayName = "Should be able to add a backup location to a list of tracked backup locations")]
        public void Test_AddingBackupLocation()
        {
            // Arrange
            CleanBeforeTest();

            // Act
            BackupManager.AddBackup(Backup1);

            // Assert
            Assert.Contains(Backup1, BackupManager.Backups.Select(s => s.FullPath));
        }
        [Fact(DisplayName = "Should NOT be able to add a backup location that does not exist")]
        public void Test_AddingNonExistentBackupLocation()
        {
            // Arrange
            CleanBeforeTest();
            string nonExistentSource = Helper.GetNonExistentDir(SourceDir);

            // Act
            BackupManager.AddBackup(nonExistentSource);

            // Assert
            Assert.DoesNotContain(nonExistentSource, BackupManager.Backups.Select(s => s.FullPath));
        }

        [Fact(DisplayName = "Should be able to run a backup to copy files from a source directory to a single backup location")]
        public void Test_SingleSourceToSingleBackup()
        {
            // Arrange
            CleanBeforeTest();
            BackupManager.AddSource(Source1);
            BackupManager.AddBackup(Backup1);

            // Act
            BackupManager.RunBackup();

            // Assert
            foreach (TargetDirectory backup in BackupManager.Backups)
                foreach (TargetDirectory source in BackupManager.Sources)
                    VerifyBackup(backup, source);
        }
        [Fact(DisplayName = "Should be able to run a backup to copy files from a source directory to multiple backup locations")]
        public void Test_SingleSourceToMultipleBackups()
        {
            // Arrange
            CleanBeforeTest();
            BackupManager.AddSource(Source1);
            BackupManager.AddBackup(Backup1);
            BackupManager.AddBackup(Backup2);

            // Act
            BackupManager.RunBackup();

            // Assert
            foreach (TargetDirectory backup in BackupManager.Backups)
                foreach (TargetDirectory source in BackupManager.Sources)
                    VerifyBackup(backup, source);
        }
        [Fact(DisplayName = "Should be able to run a backup to copy files from multiple sources to a single backup location")]
        public void Test_MultipleSourcesToSingleBackup()
        {
            // Arrange
            CleanBeforeTest();
            BackupManager.AddSource(Source1);
            BackupManager.AddSource(Source2);
            BackupManager.AddBackup(Backup1);

            // Act
            BackupManager.RunBackup();

            // Assert
            foreach (TargetDirectory backup in BackupManager.Backups)
                foreach (TargetDirectory source in BackupManager.Sources)
                    VerifyBackup(backup, source);
        }
        [Fact(DisplayName = "Should be able to run a backup to copy files from multiple sources to multiple backup locations")]
        public void Test_MultipleSourcesToMultipleBackups()
        {
            // Arrange
            CleanBeforeTest();
            BackupManager.AddSource(Source1);
            BackupManager.AddSource(Source2);
            BackupManager.AddBackup(Backup1);
            BackupManager.AddBackup(Backup2);

            // Act
            BackupManager.RunBackup();

            // Assert
            foreach (TargetDirectory backup in BackupManager.Backups)
                foreach (TargetDirectory source in BackupManager.Sources)
                    VerifyBackup(backup, source);
        }

        [Fact(DisplayName = "Should be able to run a backup to copy files from a source directory with sub-directories to a single backup location")]
        public void Test_SingleSourceToSingleBackup_WithSubDirs()
        {
            // Arrange
            CleanBeforeTest();
            BackupManager.AddSource(SourceWithSubDirs);
            BackupManager.AddBackup(Backup1);

            // Act
            BackupManager.RunBackup();

            // Assert
            foreach (TargetDirectory backup in BackupManager.Backups)
                foreach (TargetDirectory source in BackupManager.Sources)
                    VerifyBackup(backup, source);
        }
        [Fact(DisplayName = "Should be able to run a backup to copy files from a source directory with sub-directories to multiple backup locations")]
        public void Test_SingleSourceToMultipleBackups_WithSubDirs()
        {
            // Arrange
            CleanBeforeTest();
            BackupManager.AddSource(SourceWithSubDirs);
            BackupManager.AddBackup(Backup1);
            BackupManager.AddBackup(Backup2);

            // Act
            BackupManager.RunBackup();

            // Assert
            foreach (TargetDirectory backup in BackupManager.Backups)
                foreach (TargetDirectory source in BackupManager.Sources)
                    VerifyBackup(backup, source);
        }

        [Fact(DisplayName = "Should NOT be able to run a backup if source directories are not set")]
        public void Test_RunWithoutSettingSources()
        {
            // Arrange
            CleanBeforeTest();

            // Act + Assert
            Assert.ThrowsAny<InvalidOperationException>(() => BackupManager.RunBackup());
        }
        [Fact(DisplayName = "Should NOT be able to run a backup if backup locations are not set")]
        public void Test_RunWithoutSettingBackupLocations()
        {
            // Arrange
            CleanBeforeTest();
            BackupManager.AddSource(Source1);

            // Act + Assert
            Assert.ThrowsAny<InvalidOperationException>(() => BackupManager.RunBackup());
        }
    }
}