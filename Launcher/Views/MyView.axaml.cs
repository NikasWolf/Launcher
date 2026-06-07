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
using System;
using System.Diagnostics;  // если нет

namespace Launcher.Views;

public partial class MyView : UserControl
{
    private UserGameService _userGameService;
    private ObservableCollection<Game> _userGames;
    private GameService _gameService;
    private Game? _currentDisplayedGame;
    private bool _isInstalling = false;

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

    private void UpdateInstallButtonState()
    {
        if (_currentDisplayedGame == null) return;

        _currentDisplayedGame.RefreshInstallationState();
        InstallButton.Content = _currentDisplayedGame.InstallButtonText;
        System.Diagnostics.Debug.WriteLine($"UpdateInstallButtonState: Button text set to {InstallButton.Content}");
    }

    // Открыть информацию об игре
    public void ShowGameInfo(Game game)
    {
        if (game == null) return;

        _currentDisplayedGame = game;

        // Обновляем состояние установки
        game.RefreshInstallationState();

        GameInfoPanel.IsVisible = true;

        GameName.Text = game.Name;
        GameGenre.Text = $"Жанр: {game.Genre}";
        GameYear.Text = $"Год: {game.Year}";
        GameCondition.Text = $"Статус: {game.Condition}";
        GameDescription.Text = game.Description;

        // Обновляем текст кнопки
        InstallButton.Content = game.InstallButtonText;

        // Загружаем картинку
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

    // Обработчик кнопки "Удалить из списка"
    private void OnRemoveFromListClick(object? sender, RoutedEventArgs e)
    {
        if (_currentDisplayedGame != null)
        {
            RemoveGame(_currentDisplayedGame);

            // Очищаем панель
            GameInfoPanel.IsVisible = false;
            _currentDisplayedGame = null;

            // Очищаем данные
            GameName.Text = "";
            GameGenre.Text = "";
            GameYear.Text = "";
            GameCondition.Text = "";
            GameDescription.Text = "";
            GameImage.Source = null;
        }
    }

    private async void OnInstallClick(object? sender, RoutedEventArgs e)
    {
        if (_currentDisplayedGame == null) return;

        if (_currentDisplayedGame.IsGameInstalled)
        {
            _currentDisplayedGame.Installer.Launch();
            return;
        }

        if (_isInstalling) return;

        _isInstalling = true;
        InstallButton.IsEnabled = false;

        try
        {
            var progress = new Progress<int>(percent =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
                {
                    InstallButton.Content = $"Загрузка: {percent}%";
                });
            });

            await _currentDisplayedGame.Installer.InstallAsync(progress);

            // Обновление в UI потоке
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _currentDisplayedGame.RefreshInstallationState();
                UpdateInstallButtonState();
            });
        }
        catch (Exception ex)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                InstallButton.Content = "Ошибка";
            });
            System.Diagnostics.Debug.WriteLine($"Ошибка установки: {ex.Message}");
        }
        finally
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _isInstalling = false;
                InstallButton.IsEnabled = true;
            });
        }
    }

}