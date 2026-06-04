using Avalonia.Controls;
using Avalonia.Interactivity;
using Launcher.Models;
using Launcher.Services;

namespace Launcher.Views;

public partial class GamesView : UserControl
{
    // Сервис для работы с играми (загрузка/сохранение)
    // Поле создаётся, но не используется (можно удалить или использовать)
    private GameService _gameService = new();

   
    public GamesView()
    {
        InitializeComponent();  

        // 1. Создаём экземпляр сервиса для работы с JSON
        var gameService = new GameService();

        // 2. Загружаем список игр из JSON файла
        var games = gameService.LoadGames();

        // 3. Передаём список игр в ItemsControl (который отображает карточки)
        GamesItemsControl.ItemsSource = games;
    }

    // ========== ОБРАБОТЧИКИ КЛИКОВ ПО МАЛЕНЬКИМ КАРТИНКАМ ==========
    // Каждый обработчик меняет большую картинку на соответствующую маленькую

    // Клик по 1-й маленькой картинке
    private void SmallImage1_Click(object? sender, RoutedEventArgs e)
    {
        // sender - это кнопка, на которую нажали
        // btn.Tag - хранит объект Game (передан через XAML)
        if (sender is Button btn && btn.Tag is Game game)
        {
            // Устанавливаем большую картинку = первой маленькой
            game.SetMainImage(game.ImagePath1);
        }
    }

    // Клик по 2-й маленькой картинке
    private void SmallImage2_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath2);  // Меняем на вторую
        }
    }

    // Клик по 3-й маленькой картинке
    private void SmallImage3_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath3);  // Меняем на третью
        }
    }

    // Клик по 4-й маленькой картинке
    private void SmallImage4_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath4);  // Меняем на четвёртую
        }
    }

    // Клик по 5-й маленькой картинке
    private void SmallImage5_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Game game)
        {
            game.SetMainImage(game.ImagePath5);  // Меняем на пятую
        }
    }
}