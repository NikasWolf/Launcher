using Avalonia.Controls;
using Avalonia.Interactivity;
using Launcher.Models;
using Launcher.Services;

namespace Launcher.Views;

public partial class MainWindow : Window
{
    private MyView _myView;
    private GamesView _gamesView;

    public MainWindow()
    {
        InitializeComponent();

        _myView = new MyView();
        _gamesView = new GamesView();

        _gamesView.SetMyView(_myView);

        MainContent.Content = _myView;
    }

    private void OnMyClick(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = _myView;
        _myView.LoadUserGames();  // обновляем список при возврате
    }

    private void OnGamesClick(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = _gamesView;

        // Обновляем состояние всех кнопок при переходе на вкладку
        var userGameService = new UserGameService();
        var games = _gamesView.GamesItemsControl.ItemsSource as System.Collections.IEnumerable;
        if (games != null)
        {
            foreach (Game game in games)
            {
                game.UpdateButtonState(userGameService);
            }
        }
    }
}