using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Launcher.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";
    }
}
