using Avalonia.Controls;
using HkrpgProxy.Avalonia.ViewModels;

namespace HkrpgProxy.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.OnWindowClosing();
            }
        }
    }
} 