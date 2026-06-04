using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Launcher.Views;

public partial class MyView : UserControl
{
    // Поле для хранения текущего прямоугольника (Border)
    private Border? _currentBox;

    public MyView()
    {
        InitializeComponent();  // Загружает XAML разметку
        ShowGameBox();          // Показываем прямоугольник "Игра" при запуске
    }

    // ========== ОБРАБОТЧИКИ КНОПОК ==========

    // Обработчик клика по кнопке "Показать игру"
    private void OnGameClick(object? sender, RoutedEventArgs e) => ShowGameBox();

    // Обработчик клика по кнопке "Показать новость"
    private void OnNewsClick(object? sender, RoutedEventArgs e) => ShowNewsBox();

    // ========== ЛОГИКА ОТОБРАЖЕНИЯ ПРЯМОУГОЛЬНИКОВ ==========

    // Показать прямоугольник "Игра"
    private void ShowGameBox()
    {
        // 1. Удаляем старый прямоугольник, если он есть
        if (_currentBox != null)
            Container.Children.Remove(_currentBox);  

        // 2. Создаём новый прямоугольник (Border)
        _currentBox = new Border
        {
            Background = Brushes.LightBlue,          // Голубой фон
            CornerRadius = new CornerRadius(10),     // Скруглённые углы 10px
            Padding = new Thickness(20),             // Отступы внутри 20px
            Margin = new Thickness(0, 10, 0, 0),     // Отступ сверху 10px
            Child = new TextBlock                    // Внутри прямоугольника текст
            {
                Text = "Игра",                       // Текст
                FontSize = 16,                       // Размер шрифта
                FontWeight = FontWeight.Bold,        // Жирный
                Foreground = Brushes.DarkBlue,       // Цвет текста тёмно-синий
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center  // По центру
            }
        };

        // 3. Добавляем прямоугольник в контейнер
        Container.Children.Add(_currentBox);
    }

    // Показать прямоугольник "Новость" 
    private void ShowNewsBox()
    {
        // Удаляем старый прямоугольник
        if (_currentBox != null)
            Container.Children.Remove(_currentBox);

        // Создаём новый прямоугольник
        _currentBox = new Border
        {
            Background = Brushes.LightGreen,         
            CornerRadius = new CornerRadius(10),     
            Padding = new Thickness(20),             
            Margin = new Thickness(0, 10, 0, 0),    
            Child = new TextBlock
            {
                Text = "Новость",                    
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.DarkGreen,      
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            }
        };

        // Добавляем в контейнер
        Container.Children.Add(_currentBox);
    }
}