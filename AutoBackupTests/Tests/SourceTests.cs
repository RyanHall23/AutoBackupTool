using AutoBackupGUI.Models;

namespace AutoBackupTests.Tests
{
    // Tests:
    // - Should be able to add a source directory to a list of tracked sources
    // - Should NOT be able to add a source directory that does not exist

    public class SourceTests
    {
        private static readonly string BaseDir = Path.GetFullPath(@".\Test\SourceTests");

        private static readonly string SourceDir = Path.Combine(BaseDir, "source");
        private static readonly string Source1 = Path.Combine(SourceDir, "source1");

        public SourceTests()
        {
            CleanBeforeTest();
        }
        private void CleanBeforeTest()
        {
            BackupManager.Reset();
            ClearTestSources();
            InitTestSources();
        }
        private void ClearTestSources()
        {
            if (Directory.Exists(SourceDir))
                Directory.Delete(SourceDir, true);
        }
        private void InitTestSources()
        {
            Directory.CreateDirectory(Source1);
        }

        [Fact(DisplayName = "Should be able to add a source directory to a list of tracked sources")]
        public void Test_AddingSource()
        {
            // Arrange
            CleanBeforeTest();

            // Act
            BackupManager.AddSource(Source1);

            // Assert
            Assert.Contains(Source1, BackupManager.Sources.Select(s => s.FullPath));
        }
        [Fact(DisplayName = "Should NOT be able to add a source directory that does not exist")]
        public void Test_AddingNonExistentSource()
        {
            // Arrange
            CleanBeforeTest();
            string nonExistentSource = Helper.GetNonExistentDir(SourceDir);

            // Act
            BackupManager.AddSource(nonExistentSource);

            // Assert
            Assert.DoesNotContain(nonExistentSource, BackupManager.Sources.Select(s => s.FullPath));
        }
    }
}