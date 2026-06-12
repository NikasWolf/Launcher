using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Launcher.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Launcher.Models;
using System.IO; // Для работы с путями и файлами
using System.Net.Http; // Для скачивания
using System.IO.Compression; // Для распаковки архивов
using System.Diagnostics; // Для запуска процессов


// INotifyPropertyChanged - уведомляет UI об изменениях свойств (чтобы обновлялись картинки)
public class Game : INotifyPropertyChanged
{
    // ========== ЧАСТЬ 1: Уведомление об изменениях ==========

    // Событие, которое вызывается при изменении свойств
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // ========== ЧАСТЬ 2: Базовые свойства игры ==========

    public int Id { get; set; }                             // Уникальный ID игры
    public string Name { get; set; } = string.Empty;        // Название игры
    public string Condition { get; set; } = string.Empty;   // Состояние игры (релиз\разработка)
    public string Developer {  get; set; } = string.Empty;  // Разработчик
    public string Description { get; set; } = string.Empty; // Описание
    public string Genre { get; set; } = string.Empty;       // Жанр
    public int Year { get; set; }                           // Год выпуска
    public string ExecutablePath { get; set; } = string.Empty; // Путь к .exe файлу

    // ========== ЧАСТЬ 3: Пути к картинкам ==========

    public string ImagePath { get; set; } = string.Empty;   // Путь к первой (основной) картинке
    public string ImagePath1 { get; set; } = string.Empty;  // Путь к маленькой картинке 1
    public string ImagePath2 { get; set; } = string.Empty;  // Путь к маленькой картинке 2
    public string ImagePath3 { get; set; } = string.Empty;  // Путь к маленькой картинке 3
    public string ImagePath4 { get; set; } = string.Empty;  // Путь к маленькой картинке 4
    public string ImagePath5 { get; set; } = string.Empty;  // Путь к маленькой картинке 5
    public string ImagePath6 { get; set; } = string.Empty;  // Путь к маленькой картинке 5
    public string ImagePath7 { get; set; } = string.Empty;  // Путь к маленькой картинке 5

    // ========== ЧАСТЬ 4: Текущая большая картинка (с уведомлением) ==========

    // Поле для хранения текущей большой картинки
    private string _currentMainImagePath = string.Empty;

    public string CurrentMainImagePath
    {
        get
        {
            // Если _currentMainImagePath пустой → возвращаем ImagePath (первую картинку)
            return string.IsNullOrEmpty(_currentMainImagePath) ? ImagePath : _currentMainImagePath;
        }
        set
        {
            // Сохраняем новое значение
            _currentMainImagePath = value;

            // Уведомляем UI, что CurrentMainImagePath изменился
            OnPropertyChanged();

            // Уведомляем UI, что MainImage тоже изменился
            OnPropertyChanged(nameof(MainImage));
        }
    }

    // ========== ЧАСТЬ 5: Свойства-картинки (Bitmap) для привязки в XAML ==========

    // Большая картинка (загружается из CurrentMainImagePath)
    public Bitmap? MainImage => LoadImage(CurrentMainImagePath);

    // Маленькие картинки (загружаются из соответствующих путей)
    public Bitmap? SmallImage1 => LoadImage(ImagePath1);
    public Bitmap? SmallImage2 => LoadImage(ImagePath2);
    public Bitmap? SmallImage3 => LoadImage(ImagePath3);
    public Bitmap? SmallImage4 => LoadImage(ImagePath4);
    public Bitmap? SmallImage5 => LoadImage(ImagePath5);
    public Bitmap? SmallImage6 => LoadImage(ImagePath5);
    public Bitmap? SmallImage7 => LoadImage(ImagePath5);

    // ========== ЧАСТЬ 6: Загрузка картинки ==========

    // Метод загружает картинку из файла по пути path
    // Возвращает Bitmap (объект изображения) или null, если загрузка не удалась
    private Bitmap? LoadImage(string path)
    {
        try
        {
            // Если путь пустой - возвращаем null
            if (string.IsNullOrEmpty(path)) return null;

            // Создаём Uri (универсальный идентификатор ресурса) из пути
            var uri = new Uri(path);

            // Загружаем картинку: AssetLoader.Open открывает файл, Bitmap создаёт изображение
            return new Bitmap(AssetLoader.Open(uri));
        }
        catch
        {
            // Если ошибка (файл не найден, повреждён и т.д.) - возвращаем null
            return null;
        }
    }

    // ========== ЧАСТЬ 7: метод для смены картинки ==========

    public void SetMainImage(string imagePath)
    {
        CurrentMainImagePath = imagePath;
    }


    // обновление кнопок

    private string _buttonText = "добавить себе";
    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            OnPropertyChanged();
        }
    }

    private string _buttonColor = "#A8D514";
    public string ButtonColor
    {
        get => _buttonColor;
        set
        {
            _buttonColor = value;
            OnPropertyChanged();
        }
    }

    public void UpdateButtonState(UserGameService userGameService)
    {
        if (userGameService.IsGameAdded(Id))
        {
            ButtonText = "добавлено";
            ButtonColor = "#808080";
        }
        else
        {
            ButtonText = "добавить себе";
            ButtonColor = "#A8D514";
        }
    }

    // Добавь эти поля в секцию с путями к картинкам (где ImagePath1-7)
    public string ExecutableName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;

    // Добавь свойства для состояния установки
    private bool _isGameInstalled;
    public bool IsGameInstalled
    {
        get => _isGameInstalled;
        set
        {
            _isGameInstalled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(InstallButtonText));
        }
    }

    public string InstallButtonText => IsGameInstalled ? "Запустить" : "Скачать";

    // Добавь поле для установщика и метод обновления состояния
    private GameInstaller? _installer;
    public GameInstaller Installer => _installer ??= new GameInstaller(Name, DownloadUrl, ExecutableName);

    public void RefreshInstallationState()
    {
        try
        {
            IsGameInstalled = Installer.IsInstalled;
        }
        catch (DirectoryNotFoundException)
        {
            IsGameInstalled = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RefreshInstallationState error: {ex.Message}");
            IsGameInstalled = false;
        }
    }

    public void DeleteGame()
    {
        Installer.DeleteGameFiles();
        RefreshInstallationState();
    }
}