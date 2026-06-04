using Avalonia.Controls;
using Launcher.Models;
using Launcher.Services;
using System.Collections.ObjectModel;

namespace Launcher.Views;

public partial class GamesView : UserControl
{
    private GameService _gameService = new();
    public GamesView()
    {
        InitializeComponent();

        var gameService = new GameService();
        var games = gameService.LoadGames();

        // Проверка: выведи пути в консоль
        foreach (var game in games)
        {
            System.Diagnostics.Debug.WriteLine($"Game: {game.Name}, Path: {game.ImagePath}");
        }

        GamesItemsControl.ItemsSource = games;
    }
}