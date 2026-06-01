using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Launcher.Views;

public partial class MainWindow : Window
{
    private Border? _currentBox;

    public MainWindow()
    {
        InitializeComponent();
        ShowGameBox(); // Показываем игру при запуске
    }

    private void OnGameClick(object? sender, RoutedEventArgs e) => ShowGameBox();
    private void OnNewsClick(object? sender, RoutedEventArgs e) => ShowNewsBox();

    private void ShowGameBox()
    {
        // Удаляем старый
        if (_currentBox != null) Container.Children.Remove(_currentBox);

        // Создаём новый
        _currentBox = new Border
        {
            Background = Brushes.LightBlue,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20),
            Margin = new Thickness(0, 10, 0, 0),
            Child = new TextBlock
            {
                Text = "Игра",
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.DarkBlue,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            }
        };

        Container.Children.Add(_currentBox);
    }

    private void ShowNewsBox()
    {
        // Удаляем старый
        if (_currentBox != null) Container.Children.Remove(_currentBox);

        // Создаём новый
        _currentBox = new Border
        {
            Background = Brushes.LightGreen,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20),
            Margin = new Thickness(0, 10, 0, 0),
            Child = new TextBlock
            {
                Text = "Новость",
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.DarkGreen,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            }
        };

        Container.Children.Add(_currentBox);
    }
}