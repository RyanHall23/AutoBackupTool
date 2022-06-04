using AutoBackupGUI.Models;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                BackupManager.AddSource(dialog.FileName);
        }
        [ICommand]
        private void AddBackup()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                BackupManager.AddBackup(dialog.FileName);
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
