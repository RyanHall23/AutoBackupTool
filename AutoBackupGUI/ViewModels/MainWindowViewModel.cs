using AutoBackupGUI.Models;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Windows;

namespace AutoBackupGUI.ViewModels
{
    [INotifyPropertyChanged]
    public partial class MainWindowViewModel
    {
        public static BackupManager BackupManager => BackupManager.Instance;

        [ICommand]
        private void AddSource()
        {
            // TODO: Open BrowseDialog and select a folder
            BackupManager.AddSource(@"D:\Dev\~TESTING\source");
        }
        [ICommand]
        private void AddBackup()
        {
            // TODO: Open BrowseDialog and select a folder
            BackupManager.AddBackup(@"D:\Dev\~TESTING\dest");
        }
        [ICommand]
        private void RunBackup()
        {
            // TODO: Show progress window
            try
            {
                BackupManager.RunBackup();
                MessageBox.Show("Backup completed", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
