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
    private DatabaseService _databaseService = new DatabaseService();  //  добавить

    public CatalogsView()
    {
        InitializeComponent();

        _myView = new MyView();
        _gamesView = new GamesView();
        _programsView = new ProgramsView();

        
        _gamesView.SetMyView(_myView);
        _programsView.SetMyView(_myView);

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

        // Обновляем состояние кнопок через БД
        var games = _gamesView.GamesItemsControl.ItemsSource as IEnumerable;
        if (games != null)
        {
            foreach (Game game in games)
            {
                game.UpdateButtonState(_databaseService);  //  заменили
            }
        }
    }
    private void OnProgramsClick(object? sender, RoutedEventArgs e)
    {
        CatalogContent.Content = _programsView;
        _programsView.UpdateAllButtons();
    }
}