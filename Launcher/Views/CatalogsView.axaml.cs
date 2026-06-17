using Avalonia.Controls;
using Avalonia.Interactivity;
using Launcher.Models;
using Launcher.Services;
using System.Collections;

namespace Launcher.Views;

public partial class CatalogsView : UserControl
{
    private MyView _myView;
    private GamesView _gamesView;
    private ProgramsView _programsView;

    public CatalogsView()
    {
        InitializeComponent();

        // Создаём вкладки
        _myView = new MyView();
        _gamesView = new GamesView();
        _programsView = new ProgramsView();
        // Передаём ссылку на MyView в GamesView
        _gamesView.SetMyView(_myView);
        
        // По умолчанию показываем "Моё"
        CatalogContent.Content = _myView;
    }

    private void OnMyClick(object? sender, RoutedEventArgs e)
    {
        CatalogContent.Content = _myView;
        _myView.LoadUserGames();
    }

    private void OnGamesClick(object? sender, RoutedEventArgs e)
    {
        CatalogContent.Content = _gamesView;

        // Обновляем состояние кнопок
        var userGameService = new UserGameService();
        var games = _gamesView.GamesItemsControl.ItemsSource as IEnumerable;
        if (games != null)
        {
            foreach (Game game in games)
            {
                game.UpdateButtonState(userGameService);
            }
        }
    }
}