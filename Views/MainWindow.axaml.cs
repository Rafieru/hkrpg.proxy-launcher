using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using HkrpgProxy.Avalonia.ViewModels;
using System;
using System.Threading.Tasks;

namespace HkrpgProxy.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            _viewModel.SetWindow(this);
        }

        private async void BrowseButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.Log("Browse button clicked", MainWindowViewModel.LogLevel.DEBUG);
                var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select Game Executable",
                    AllowMultiple = false,
                    FileTypeFilter = new[] { new FilePickerFileType("Executable") { Patterns = new[] { "*.exe" } } }
                });

                _viewModel.Log($"File picker returned {files.Count} files", MainWindowViewModel.LogLevel.DEBUG);

                if (files.Count > 0)
                {
                    var file = files[0];
                    _viewModel.Log($"Selected file path: {file.Path}", MainWindowViewModel.LogLevel.DEBUG);
                    
                    if (file.Path.IsFile)
                    {
                        var textBox = this.FindControl<TextBox>("GamePathTextBox");
                        _viewModel.Log($"TextBox current text: {textBox?.Text}", MainWindowViewModel.LogLevel.DEBUG);
                        
                        _viewModel.GamePath = file.Path.LocalPath;
                        _viewModel.Log($"Set GamePath to: {_viewModel.GamePath}", MainWindowViewModel.LogLevel.DEBUG);
                        
                        if (textBox != null)
                        {
                            textBox.Text = file.Path.LocalPath;
                            _viewModel.Log($"Set TextBox text to: {textBox.Text}", MainWindowViewModel.LogLevel.DEBUG);
                        }
                        
                        _viewModel.SaveSettings();
                        _viewModel.Log($"Selected game executable: {file.Path.LocalPath}", MainWindowViewModel.LogLevel.INFO);
                    }
                }
            }
            catch (Exception ex)
            {
                _viewModel.Log($"Error selecting game executable: {ex.Message}", MainWindowViewModel.LogLevel.ERROR);
                _viewModel.Log($"Stack trace: {ex.StackTrace}", MainWindowViewModel.LogLevel.DEBUG);
            }
        }
    }
}
