using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Launcher.Models;
using Launcher.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Launcher.Views;

public partial class MyView : UserControl
{
    private UserGameService _userGameService;
    private ObservableCollection<Game> _userGames;
    private GameService _gameService;

    public MyView()
    {
        InitializeComponent();

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
        LoadUserGames();
    }

    public void RemoveGame(Game game)
    {
        _userGameService.RemoveGame(game.Id);
        LoadUserGames();
    }

    public bool IsGameAdded(Game game)
    {
        return _userGameService.IsGameAdded(game.Id);
    }

    // Открыть информацию об игре
    public void ShowGameInfo(Game game)
    {
        if (game == null) return;

        GameInfoPanel.IsVisible = true;

        GameName.Text = game.Name;
        GameGenre.Text = $"Жанр: {game.Genre}";
        GameYear.Text = $"Год: {game.Year}";
        GameCondition.Text = $"Статус: {game.Condition}";
        GameDescription.Text = game.Description;

        if (!string.IsNullOrEmpty(game.ImagePath))
        {
            try
            {
                var uri = new Uri(game.ImagePath);
                GameImage.Source = new Bitmap(AssetLoader.Open(uri));
            }
            catch
            {
                GameImage.Source = null;
            }
        }
    }

    // Обработчик клика по игре в списке
    private void OnGameClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            ShowGameInfo(game);
        }
    }
}