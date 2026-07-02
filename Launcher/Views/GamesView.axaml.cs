using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using Launcher.Models;
using Launcher.Services;

namespace Launcher.Views;

public partial class GamesView : UserControl
{
    private MyView? _myView;
    private DatabaseService _databaseService = new DatabaseService();

    public GamesView()
    {
        InitializeComponent();

        var userGameService = new UserGameService();
        var games = _databaseService.LoadGames();

        foreach (var game in games)
        {
            game.UpdateButtonState(userGameService);
        }

        GamesItemsControl.ItemsSource = games;
    }

    public void SetMyView(MyView myView)
    {
        _myView = myView;
    }

    public void UpdateAllButtons()
    {
        var userGameService = new UserGameService();
        var games = _databaseService.LoadGames();

        foreach (var game in games)
        {
            game.UpdateButtonState(userGameService);
        }

        GamesItemsControl.ItemsSource = null;
        GamesItemsControl.ItemsSource = games;
    }

    private void OnAddToSelfClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game && _myView != null)
        {
            var userGameService = new UserGameService();
            if (!userGameService.IsGameAdded(game.Id))
            {
                _myView.AddGame(game);
                game.UpdateButtonState(userGameService);
            }
        }
    }

    // ========== ÑÌÅÍÀ ÃËÀÂÍÎÉ ÊÀĞÒÈÍÊÈ ==========
    private void SmallImage_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Bitmap bitmap)
        {
            var parent = btn.Parent;
            while (parent != null)
            {
                if (parent is Border border && border.DataContext is Game game)
                {
                    var index = game.LoadedImages.IndexOf(bitmap);
                    if (index >= 0 && index < game.Images.Count)
                    {
                        game.SetMainImage(game.Images[index]);
                    }
                    break;
                }
                parent = parent.Parent;
            }
        }
    }
}