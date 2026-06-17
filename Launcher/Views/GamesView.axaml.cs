using Avalonia;                      // Основной фреймворк Avalonia
using Avalonia.Controls;              // Элементы управления (UserControl, Button, ItemsControl)
using Avalonia.Interactivity;         // Обработка событий (RoutedEventArgs)
using Avalonia.Media;                 // Работа с цветами и кистями (Brushes)
using Launcher.Models;                // Модель данных Game
using Launcher.Services;              // Сервисы (GameService, UserGameService)

namespace Launcher.Views;             // Пространство имён представлений

/// Вкладка "Игры" — отображает все игры в виде карточек.
/// Позволяет добавлять игры в список "Моё".
public partial class GamesView : UserControl
{

    // Ссылка на вкладку "Моё", чтобы вызывать методы добавления игр
    private MyView? _myView;
    public GamesView()
    {
        InitializeComponent();  

        // 1. Создаём сервисы для работы с данными
        var gameService = new GameService();          // Для чтения games.json
        var userGameService = new UserGameService();  // Для проверки добавленных игр

        // 2. Загружаем все игры из каталога
        var games = gameService.LoadGames();

        // 3. Для каждой игры обновляем состояние кнопки "добавить себе"
        //    Проверяем: если игра уже в списке "Моё" — кнопка будет "добавлено"
        foreach (var game in games)
        {
            game.UpdateButtonState(userGameService);
        }

        // 4. Показываем список игр в ItemsControl
        GamesItemsControl.ItemsSource = games;
    }

    // ========== МЕТОДЫ ==========

    /// Устанавливает ссылку на вкладку "Моё".
    /// Вызывается из MainWindow при запуске.
    public void SetMyView(MyView myView)
    {
        _myView = myView;
    }

    /// Обновляет состояние всех кнопок "добавить себе".
    /// Используется при переключении на вкладку "Игры" или после добавления игры.
    public void UpdateAllButtons()
    {
        // 1. Создаём сервисы
        var userGameService = new UserGameService();
        var gameService = new GameService();

        // 2. Загружаем все игры
        var games = gameService.LoadGames();

        // 3. Для каждой игры обновляем состояние кнопки
        foreach (var game in games)
        {
            game.UpdateButtonState(userGameService);
        }

        // 4. Перезагружаем ItemsControl, чтобы кнопки отобразились с новым состоянием
        //    Сначала обнуляем, потом устанавливаем заново
        GamesItemsControl.ItemsSource = null;
        GamesItemsControl.ItemsSource = games;
    }

    // ========== ОБРАБОТЧИКИ СОБЫТИЙ ==========

    /// Обработчик клика по кнопке "добавить себе" в карточке игры.
    private void OnAddToSelfClick(object? sender, RoutedEventArgs e)
    {
        // 1. Извлекаем объект Game из кнопки (через Tag)
        //    sender — это кнопка, на которую нажали
        //    btn.Tag — это объект Game, переданный через Binding
        if (sender is Button btn && btn.Tag is Game game && _myView != null)
        {
            // 2. Проверяем, не добавлена ли уже игра в "Моё"
            var userGameService = new UserGameService();
            if (!userGameService.IsGameAdded(game.Id))
            {
                // 3. Если не добавлена — добавляем через MyView
                _myView.AddGame(game);

                // 4. Обновляем состояние кнопки (меняется на "добавлено")
                game.UpdateButtonState(userGameService);
            }
        }
    }

    // ========== СМЕНА ГЛАВНОЙ КАРТИНКИ ==========
    private void SmallImage1_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath1);  // Меняем на первую маленькую
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

    private void SmallImage6_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath6);
        }
    }

    private void SmallImage7_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath7);
        }
    }
}