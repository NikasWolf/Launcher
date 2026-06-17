using Avalonia.Controls;          // Элементы управления (Window, ContentControl)
using Avalonia.Input;
using Avalonia.Interactivity;     // Обработка событий (RoutedEventArgs)
using Launcher.Models;            // Модель Game
using Launcher.Services;          // Сервисы (UserGameService)

namespace Launcher.Views;         // Пространство имён представлений


/// Главное окно приложения.
/// Содержит верхнюю панель с кнопками "Моё" и "Игры".
/// Переключает содержимое между вкладками MyView и GamesView.

public partial class MainWindow : Window
{
    // ========== Ссылки ==========

    // Ссылки на вкладки (UserControl'ы)
    private MyView _myView;        // Вкладка "Моё"
    private GamesView _gamesView;  // Вкладка "Игры"
    

    //========= Конструктор ==========
    public MainWindow()
    {
        InitializeComponent();  // Загружает XAML разметку

        // Переносим окно только за верхний бордер
        var titleBar = this.Find<Border>("TitleBar");
        if (titleBar != null)
        {
            titleBar.PointerPressed += OnWindowPointerPressed;
        }


        // 1. Создаём экземпляры вкладок
        _myView = new MyView();
        _gamesView = new GamesView();
        

        // 2. Передаём ссылку на MyView в GamesView
        //    Это нужно, чтобы GamesView мог вызывать методы добавления игр
        _gamesView.SetMyView(_myView);

        // 3. По умолчанию показываем вкладку "Моё" - поменять на 
        MainContent.Content = _myView;
    }



    //================ Методы и обработчики ==========================
    // перетаскивание окна
    private void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
    }


    // ========== обработчики базовых кнопок ================
    private void OnMinimizeClick(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
    private void OnMaximizeClick(object? sender, RoutedEventArgs e)
    {
        this.WindowState = this.WindowState == WindowState.Normal
            ? WindowState.Maximized : WindowState.Normal;
    }
    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }



    // ========== ОБРАБОТЧИКИ ПЕРЕКЛЮЧЕНИЯ ВКЛАДОК ==========

    /// Нажатие на кнопку "Моё".
    /// Переключает на вкладку MyView и обновляет список добавленных игр.
    private void OnMyClick(object? sender, RoutedEventArgs e)
    {
        // 1. Показываем вкладку "Моё"
        MainContent.Content = _myView;
        // 2. Обновляем список игр в "Моё"
        _myView.LoadUserGames();
    }


    /// Переключает на вкладку GamesView и обновляет состояние кнопок.
    private void OnGamesClick(object? sender, RoutedEventArgs e)
    {
        // 1. Показываем вкладку "Игры"
        MainContent.Content = _gamesView;

        // 2. Обновляем состояние всех кнопок "добавить себе"
        //    При переключении на вкладку проверяем, какие игры уже в "Моё"
        var userGameService = new UserGameService();

        // 3. Получаем список игр из GamesView (из ItemsControl)
        var games = _gamesView.GamesItemsControl.ItemsSource as System.Collections.IEnumerable;
        if (games != null)
        {
            // 4. Для каждой игры обновляем состояние кнопки
            foreach (Game game in games)
            {
                game.UpdateButtonState(userGameService);
            }
        }
    }

    private void OnNewsClick(object? sender, RoutedEventArgs e)
    {
        var newsView = new NewsView();
        MainContent.Content = newsView;
    }

    private void OnCatalogsClick(object? sender, RoutedEventArgs e)
    {
        var catalogView = new CatalogsView();
        MainContent.Content = catalogView;
    }

    private void OnSettingsClick(object? sender, RoutedEventArgs e)
    {
        var settingsView = new SettingsView();
        MainContent.Content = settingsView;
    }

    private void OnAccountClick(object? sender, RoutedEventArgs e)
    {
        var accountView = new AccountView();
        MainContent.Content = accountView;
    }
}