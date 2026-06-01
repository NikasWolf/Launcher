using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Launcher.Views;

public partial class MainWindow : Window
{
    private MyView _myView;
    private GamesView _gamesView;

    public MainWindow()
    {
        InitializeComponent();

        // Создаём оба представления
        _myView = new MyView();
        _gamesView = new GamesView();

        // Показываем "Моё" при запуске
        MainContent.Content = _myView;
    }

    private void OnMyClick(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = _myView;
    }

    private void OnGamesClick(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = _gamesView;
    }
}