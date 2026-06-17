using Avalonia.Controls;          // Элементы управления (Window, ContentControl)
using Avalonia.Input;
using Avalonia.Interactivity;     // Обработка событий (RoutedEventArgs)
using Launcher.Models;            // Модель Game
using Launcher.Services;          // Сервисы (UserGameService)

namespace Launcher.Views;         // Пространство имён представлений


/// Главное окно приложения.
//содержит настройки верхней понели и шлав навигации

public partial class MainWindow : Window
{
    // ========== Ссылки ==========

    // Ссылки на вкладки (UserControl'ы)
    private CatalogsView _catalogsView;//каталог

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
        // по умолчанию открываем каталог
        _catalogsView = new CatalogsView();
        MainContent.Content = _catalogsView;
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
    //                   настройки
    private void OnSettingsClick(object? sender, RoutedEventArgs e)
    {
        var settingsView = new SettingsView();
        MainContent.Content = settingsView;
    }
    //                   аккаунт
    private void OnAccountClick(object? sender, RoutedEventArgs e)
    {
        var accountView = new AccountView();
        MainContent.Content = accountView;
    }
    //                  каталоги
    private void OnCatalogsClick(object? sender, RoutedEventArgs e)
    {
        var catalogView = new CatalogsView();
        MainContent.Content = catalogView;
    }
    //                  новости
    private void OnNewsClick(object? sender, RoutedEventArgs e)
    {
        var newsView = new NewsView();
        MainContent.Content = newsView;
    }
}