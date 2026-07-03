using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Launcher.Models;
using Launcher.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Launcher.Views;

public partial class MyView : UserControl
{
    // ========== ПОЛЯ КЛАССА ==========
    private ObservableCollection<Game> _userGames;
    private DatabaseService _databaseService;
    private Game? _currentDisplayedGame;
    private bool _isInstalling = false;
    public bool ShowDeleteButton { get; set; }

    // ========== КОНСТРУКТОР ==========
    public MyView()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
        _userGames = new ObservableCollection<Game>();

        LoadUserGames();
        UserGamesList.ItemsSource = _userGames;
    }

    // ========== МЕТОДЫ РАБОТЫ СО СПИСКОМ ДОБАВЛЕННЫХ ИГР ==========

    public void LoadUserGames()
    {
        var addedIds = _databaseService.LoadUserGameIds();
        var allGames = _databaseService.LoadGames();

        _userGames.Clear();
        foreach (var id in addedIds)
        {
            var game = allGames.FirstOrDefault(g => g.Id == id);
            if (game != null)
            {
                game.RefreshInstallationState();
                _userGames.Add(game);
            }
        }
    }

    public void AddGame(Game game)
    {
        _databaseService.AddUserGame(game.Id);
        LoadUserGames();
    }

    public void RemoveGame(Game game)
    {
        _databaseService.RemoveUserGame(game.Id);
        LoadUserGames();
    }

    public bool IsGameAdded(Game game)
    {
        return _databaseService.IsGameAdded(game.Id);
    }

    // ========== МЕТОДЫ ОБНОВЛЕНИЯ КНОПКИ УСТАНОВКИ ==========

    private void UpdateInstallButtonState()
    {
        if (_currentDisplayedGame == null) return;

        _currentDisplayedGame.RefreshInstallationState();
        InstallButton.Content = _currentDisplayedGame.InstallButtonText;
        Debug.WriteLine($"UpdateInstallButtonState: Button text set to {InstallButton.Content}");
    }

    // ========== МЕТОДЫ ОТОБРАЖЕНИЯ ИНФОРМАЦИИ ==========

    public void ShowGameInfo(Game game)
    {
        if (game == null) return;

        _currentDisplayedGame = game;
        game.RefreshInstallationState();

        GameInfoPanel.IsVisible = true;
        RightShadow.IsVisible = true;

        GameName.Text = game.Name;
        GameGenre.Text = $"Жанр: {game.Genre}";
        GameYear.Text = $"Дата релиза: {game.Year}";
        GameCondition.Text = $"Версия: {game.Condition}";
        GameDescription.Text = game.Description;
        GameDeveloper.Text = $"Разработчик: {game.Developer}";
        GameAgeRest.Text = game.AgeRest;

        InstallButton.Content = game.InstallButtonText;

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

        Tag0Border.IsVisible = game.Tag0;
        Tag1Border.IsVisible = game.Tag1;
        Tag2Border.IsVisible = game.Tag2;
        Tag3Border.IsVisible = game.Tag3;

        // Загружаем новости для игры
        var news = _databaseService.LoadNewsForGame(game.Id);
        GameNewsControl.ItemsSource = news;
    }

    // ========== ОБРАБОТЧИКИ СОБЫТИЙ ==========

    private void OnGameClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            _currentDisplayedGame = game;
            ShowGameInfo(game);
        }
    }

    private void OnRemoveFromListClick(object? sender, RoutedEventArgs e)
    {
        if (_currentDisplayedGame != null)
        {
            RemoveGame(_currentDisplayedGame);

            GameInfoPanel.IsVisible = false;
            RightShadow.IsVisible = false;
            _currentDisplayedGame = null;

            GameName.Text = "";
            GameGenre.Text = "";
            GameYear.Text = "";
            GameCondition.Text = "";
            GameDescription.Text = "";
            GameImage.Source = null;
        }
    }

    // ========================= ПРАВАЯ ПАНЕЛЬ =========================

    // ----------------------- Скачать -----------------------
    private bool _isHoveringMenu = false;

    private void OnButtonPointerEnter(object? sender, PointerEventArgs e)
    {
        if (!_currentDisplayedGame.IsGameInstalled)
        {
            MenuPopup.IsOpen = true;
            return;
        }
    }

    private void OnButtonPointerLeave(object? sender, PointerEventArgs e)
    {
        Task.Delay(200).ContinueWith(_ =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
            {
                if (!_isHoveringMenu)
                {
                    MenuPopup.IsOpen = false;
                }
            });
        });
    }

    private void OnMenuPointerEnter(object? sender, PointerEventArgs e)
    {
        if (!_currentDisplayedGame.IsGameInstalled)
        {
            _isHoveringMenu = true;
            return;
        }
    }

    private void OnMenuPointerLeave(object? sender, PointerEventArgs e)
    {
        _isHoveringMenu = false;
        MenuPopup.IsOpen = false;
    }

    private void OnPlayClick(object? sender, RoutedEventArgs e)
    {
        if (_currentDisplayedGame.IsGameInstalled)
        {
            _currentDisplayedGame.Installer.Launch();
        }
    }

    private async void OnInstalGitHubClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            _currentDisplayedGame = game;
        }

        if (_currentDisplayedGame == null) return;

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
            Debug.WriteLine($"Ошибка установки: {ex.Message}");
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

    private void OnInstalServerClick(object sender, RoutedEventArgs e)
    {
        // заготовка
    }

    // ----------------------- Удаление -----------------------
    private bool _isHoveringMenu2 = false;

    private void OnButtonPointerEnter2(object? sender, PointerEventArgs e)
    {
        MenuPopup2.IsOpen = true;
    }

    private void OnButtonPointerLeave2(object? sender, PointerEventArgs e)
    {
        Task.Delay(200).ContinueWith(_ =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
            {
                if (!_isHoveringMenu2)
                {
                    MenuPopup2.IsOpen = false;
                }
            });
        });
    }

    private void OnMenuPointerEnter2(object? sender, PointerEventArgs e)
    {
        _isHoveringMenu2 = true;
    }

    private void OnMenuPointerLeave2(object? sender, PointerEventArgs e)
    {
        _isHoveringMenu2 = false;
        MenuPopup2.IsOpen = false;
    }

    private async void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_currentDisplayedGame.IsGameInstalled)
        {
            if (_currentDisplayedGame == null) return;

            var box = MessageBoxManager.GetMessageBoxStandard(
                "Подтверждение удаления",
                $"Вы уверены, что хотите удалить игру \"{_currentDisplayedGame.Name}\" с компьютера?",
                ButtonEnum.YesNo
            );

            var result = await box.ShowAsync();

            if (result == ButtonResult.Yes)
            {
                try
                {
                    _currentDisplayedGame.DeleteGame();
                    UpdateInstallButtonState();
                    Debug.WriteLine("Игра удалена");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка удаления: {ex.Message}");
                }
            }
        }
        else
        {
            if (_currentDisplayedGame == null) return;
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Ошибка - Игры нет :/",
                "Этой игры и так нет на вашем устройстве, удалять нечего",
                ButtonEnum.Ok,
                Icon.Warning
            );

            await box.ShowAsync();
        }
    }

    private void OnMiniClick(object? sender, RoutedEventArgs e)
    {
        OnInstalGitHubClick(sender, e);
        OnGameClick(sender, e);
    }

    // ========================= ИНФОРМАЦИЯ =========================
    private void OnInfoPointerEnter(object? sender, PointerEventArgs e)
    {
        MenuPopup3.IsOpen = true;
    }

    private void OnInfoPointerLeave(object? sender, PointerEventArgs e)
    {
        MenuPopup3.IsOpen = false;
    }

    // ========================= НОВОСТИ / ОТЗЫВЫ =========================
    public bool NewCom = false;

    private void OnNewComButton(object? sender, RoutedEventArgs e)
    {
        NewCom = !NewCom;
        if (NewCom)
        {
            NewComButton.Content = "Отзывы";
            NewComText.Text = "Новости";
        }
        else
        {
            NewComButton.Content = "Новости";
            NewComText.Text = "Отзывы";
        }
    }
}