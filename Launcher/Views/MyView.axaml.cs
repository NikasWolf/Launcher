// Подключаем необходимые пространства имён
using Avalonia;                       // Основной фреймворк Avalonia
using Avalonia.Controls;              // Элементы управления (кнопки, окна и т.д.)
using Avalonia.Input;           // Типы кнопок для MessageBox (YesNo, OK и т.д.)
using Avalonia.Interactivity;         // Обработка событий (клики, наведение)
using Avalonia.Media;                 // Работа с цветами, кистями
using Avalonia.Media.Imaging;         // Загрузка и работа с изображениями (Bitmap)
using Avalonia.Platform;              // Доступ к ресурсам приложения (AssetLoader)
using Launcher.Models;                // Наши модели данных (класс Game)
using Launcher.Services;              // Наши сервисы (GameService, UserGameService)
using MsBox.Avalonia;                 // Библиотека для всплывающих сообщений (MessageBox) 
using MsBox.Avalonia.Enums;
using System;                         // Базовые типы (string, int и т.д.)
using System.Collections.ObjectModel; // ObservableCollection - список с уведомлениями об изменениях
using System.Diagnostics;             // Отладка (Debug.WriteLine)
using System.Linq;                    // LINQ (FirstOrDefault, Where и т.д.)
using System.Threading.Tasks;
namespace Launcher.Views;             // Пространство имён для представлений (экранов)

// Класс MyView - пользовательский элемент управления (UserControl)
// Отображает список добавленных игр и детальную информацию о выбранной игре
public partial class MyView : UserControl
{
    // ========== ПОЛЯ КЛАССА ==========

    private UserGameService _userGameService;        // Сервис для работы с JSON (добавленные игры)
    private ObservableCollection<Game> _userGames;   // Список добавленных игр (автоматически обновляет UI)
    private GameService _gameService;                // Сервис для загрузки всех игр из JSON
    private Game? _currentDisplayedGame;             // Текущая выбранная игра (отображается в правой панели)
    private bool _isInstalling = false;              // Флаг: идёт ли установка сейчас (блокирует повторные клики)

    // ========== КОНСТРУКТОР ==========
    // Вызывается при создании MyView
    public MyView()
    {
        InitializeComponent();                       // Загружает XAML разметку

        // Создаём экземпляры сервисов
        _userGameService = new UserGameService();    // Для работы с user_games.json
        _gameService = new GameService();             // Для работы с games.json
        _userGames = new ObservableCollection<Game>(); // Создаём пустой список для отображения

        LoadUserGames();                              // Загружаем добавленные игры из JSON
        UserGamesList.ItemsSource = _userGames;       // Привязываем список к ItemsControl в XAML
    }

    // ========== МЕТОДЫ РАБОТЫ СО СПИСКОМ ДОБАВЛЕННЫХ ИГР ==========

    // Загружает игры, которые пользователь добавил в "Моё"
    public void LoadUserGames()
    {
        var addedIds = _userGameService.LoadUserGameIds();  // Получаем ID добавленных игр [1, 2, 3]
        var allGames = _gameService.LoadGames();            // Получаем ВСЕ игры из каталога

        _userGames.Clear();                                 // Очищаем текущий список
        foreach (var id in addedIds)                        // Для каждого ID из добавленных...
        {
            var game = allGames.FirstOrDefault(g => g.Id == id); // Находим игру с таким ID
            if (game != null)
            {
                game.RefreshInstallationState();  
                _userGames.Add(game);                       // Добавляем найденную игру в список
            }
        }
    }

    // Добавляет игру в "Моё" (сохраняет ID в JSON)
    public void AddGame(Game game)
    {
        _userGameService.AddGame(game.Id);   // Сохраняем ID игры в user_games.json
        LoadUserGames();                     // Обновляем список на экране
    }

    // Удаляет игру из "Моё" (убирает ID из JSON)
    public void RemoveGame(Game game)
    {
        _userGameService.RemoveGame(game.Id); // Удаляем ID из user_games.json
        LoadUserGames();                      // Обновляем список на экране
    }

    // Проверяет, добавлена ли игра в "Моё"
    public bool IsGameAdded(Game game)
    {
        return _userGameService.IsGameAdded(game.Id); // Проверяем, есть ли ID в JSON
    }

    // ========== МЕТОДЫ ОБНОВЛЕНИЯ КНОПКИ УСТАНОВКИ ==========

    // Обновляет состояние кнопки "Скачать"/"Запустить"
    private void UpdateInstallButtonState()
    {
        if (_currentDisplayedGame == null) return;  // Если нет выбранной игры - выходим

        _currentDisplayedGame.RefreshInstallationState();       // Обновляем статус установки
        InstallButton.Content = _currentDisplayedGame.InstallButtonText; // Меняем текст кнопки
        System.Diagnostics.Debug.WriteLine($"UpdateInstallButtonState: Button text set to {InstallButton.Content}");
    }

    // ========== МЕТОДЫ ОТОБРАЖЕНИЯ ИНФОРМАЦИИ ==========

    // Открывает информацию об игре в правой панели
    public void ShowGameInfo(Game game)
    {
        if (game == null) return;                     // Если нет игры - выходим

        _currentDisplayedGame = game;                 // Запоминаем выбранную игру

        game.RefreshInstallationState();              // Обновляем статус установки

        GameInfoPanel.IsVisible = true;               // Показываем панель с информацией

        RightShadow.IsVisible = true;                 // Показываем тень

        // Заполняем поля данными из JSON
        GameName.Text = game.Name;
        GameGenre.Text = $"Жанр: {game.Genre}";
        GameYear.Text = $"Год: {game.Year}";
        GameCondition.Text = $"Статус: {game.Condition}";
        GameDescription.Text = game.Description;
        GameDeveloper.Text = $"Разработчик: {game.Developer}";
        GameAgeRest.Text = game.AgeRest;
        // Устанавливаем текст кнопки (Скачать/Запустить)
        InstallButton.Content = game.InstallButtonText;

        // Загружаем и отображаем картинку игры
        if (!string.IsNullOrEmpty(game.ImagePath))
        {
            try
            {
                var uri = new Uri(game.ImagePath);                     // Создаём путь к файлу
                GameImage.Source = new Bitmap(AssetLoader.Open(uri));  // Загружаем картинку
            }
            catch
            {
                GameImage.Source = null;                               // Если ошибка - картинки нет
            }
        }
    }

    // ========== ОБРАБОТЧИКИ СОБЫТИЙ (КЛИКИ ПО КНОПКАМ) ==========
    // ========== Левая понел
    // Клик по игре в левом списке - открываем информацию
    private void OnGameClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)  // Из кнопки достаём объект Game
        {
            _currentDisplayedGame = game;
            ShowGameInfo(game);                            // Показываем информацию об игре
        }
    }

    // Клик по кнопке "Убрать из списка" - удаляем игру из "Моё"
    private void OnRemoveFromListClick(object? sender, RoutedEventArgs e)
    {
        if (_currentDisplayedGame != null)                 // Если есть выбранная игра
        {
            RemoveGame(_currentDisplayedGame);             // Удаляем её из JSON и списка

            // Очищаем и скрываем панель информации
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
    //======================== Правая понель 
    // ----------------------- скачать
    private bool _isHoveringMenu = false;

    private void OnButtonPointerEnter(object? sender, PointerEventArgs e)
    {
        MenuPopup.IsOpen = true;
    }

    private void OnButtonPointerLeave(object? sender, PointerEventArgs e)
    {
        // Не закрываем сразу — даём время зайти на меню
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
        _isHoveringMenu = true;
    }

    private void OnMenuPointerLeave(object? sender, PointerEventArgs e)
    {
        _isHoveringMenu = false;
        MenuPopup.IsOpen = false;
    }


    // скачать с гита
    private async void OnInstalGitHubClick(object? sender, RoutedEventArgs e)
    {//не забыть что этот метод вызывается в маленькой кнопке в списке игр
        // Получаем игру из Tag кнопки
        if (sender is Button btn && btn.Tag is Game game)
        {
            _currentDisplayedGame = game;
        }

        if (_currentDisplayedGame == null) return;

        // Если игра уже установлена - просто запускаем
        if (_currentDisplayedGame.IsGameInstalled)
        {
            _currentDisplayedGame.Installer.Launch();
            return;
        }

        // Если уже идёт установка - не начинаем новую
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
    // скачать с сервера
    private void OnInstalServerClick(object sender, RoutedEventArgs e)
    {
        // заготовка
    }





    // Клик по кнопке "Удалить игру" - удаляем файлы игры с диска
    private async void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_currentDisplayedGame == null) return;

        // Показываем диалог подтверждения
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Подтверждение удаления",
            $"Вы уверены, что хотите удалить игру \"{_currentDisplayedGame.Name}\" с компьютера?",
            ButtonEnum.YesNo
        );

        var result = await box.ShowAsync();  // Ждём ответа пользователя

        if (result == ButtonResult.Yes)      // Если нажал "Да"
        {
            try
            {
                _currentDisplayedGame.DeleteGame();      // Удаляем папку с игрой
                UpdateInstallButtonState();               // Кнопка меняется на "Скачать"
                System.Diagnostics.Debug.WriteLine("Игра удалена");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления: {ex.Message}");
            }
        }
    }

    // маленькая кнопка скачать
    private void OnMiniClick(object? sender, RoutedEventArgs e)
    {
        OnInstalGitHubClick(sender, e);  
        OnGameClick(sender, e);     
    }

    //=============================маленькое меню=============

}