using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Launcher.Models;
using Launcher.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace Launcher.Views;

public partial class MyView : UserControl
{
    private Border? _currentBox;
    private UserGameService _userGameService;
    private ObservableCollection<Game> _userGames;
    private GameService _gameService;

    public MyView()
    {
        InitializeComponent();
        ShowGameBox();

        _userGameService = new UserGameService();
        _gameService = new GameService();
        _userGames = new ObservableCollection<Game>();

        LoadUserGames();
        UserGamesList.ItemsSource = _userGames;
    }

    public void LoadUserGames()
    {
        var addedIds = _userGameService.LoadUserGameIds();
        var allGames = _gameService.LoadGames();

        _userGames.Clear();
        foreach (var id in addedIds)
        {
            var game = allGames.FirstOrDefault(g => g.Id == id);
            if (game != null)
            {
                _userGames.Add(game);
            }
        }
    }

    public void AddGame(Game game)
    {
        _userGameService.AddGame(game.Id);
        LoadUserGames();  // обновляем список
    }

    public void RemoveGame(Game game)
    {
        _userGameService.RemoveGame(game.Id);
        LoadUserGames();  // обновляем список
    }

    public bool IsGameAdded(Game game)
    {
        return _userGameService.IsGameAdded(game.Id);
    }

    private void OnGameClick(object? sender, RoutedEventArgs e) => ShowGameBox();
    private void OnNewsClick(object? sender, RoutedEventArgs e) => ShowNewsBox();

    private void ShowGameBox()
    {
        if (_currentBox != null) Container.Children.Remove(_currentBox);

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
        if (_currentBox != null) Container.Children.Remove(_currentBox);

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