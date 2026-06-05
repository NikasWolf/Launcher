using Avalonia.Controls;
using Avalonia.Interactivity;
using Launcher.Models;
using Launcher.Services;

namespace Launcher.Views;

public partial class GamesView : UserControl
{
    private MyView? _myView;

    public GamesView()
    {
        InitializeComponent();

        var gameService = new GameService();
        var games = gameService.LoadGames();
        GamesItemsControl.ItemsSource = games;
    }

    public void SetMyView(MyView myView)
    {
        _myView = myView;
        UpdateAllButtons();
    }

    public void UpdateAllButtons()
    {
        // Перезагружаем ItemsControl, чтобы обновить состояние кнопок
        var gameService = new GameService();
        var games = gameService.LoadGames();
        GamesItemsControl.ItemsSource = null;
        GamesItemsControl.ItemsSource = games;
    }

    private void OnAddToSelfClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game && _myView != null)
        {
            if (!_myView.IsGameAdded(game))
            {
                _myView.AddGame(game);
                UpdateAllButtons();  // обновляем все кнопки
            }
        }
    }

    private void SmallImage1_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath1);
        }
    }

    private void SmallImage2_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath2);
        }
    }

    private void SmallImage3_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath3);
        }
    }

    private void SmallImage4_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath4);
        }
    }

    private void SmallImage5_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath5);
        }
    }
}