using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Launcher.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace Launcher.Models;

public class Game : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string AgeRest { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty;
    public int Stasus { get; set; }

    public string Icon { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string ImagePath1 { get; set; } = string.Empty;
    public string ImagePath2 { get; set; } = string.Empty;
    public string ImagePath3 { get; set; } = string.Empty;
    public string ImagePath4 { get; set; } = string.Empty;
    public string ImagePath5 { get; set; } = string.Empty;
    public string ImagePath6 { get; set; } = string.Empty;
    public string ImagePath7 { get; set; } = string.Empty;

    private string _currentMainImagePath = string.Empty;
    public string CurrentMainImagePath
    {
        get => string.IsNullOrEmpty(_currentMainImagePath) ? ImagePath : _currentMainImagePath;
        set
        {
            _currentMainImagePath = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MainImage));
        }
    }

    public Bitmap? GameIcon => LoadImage(Icon);
    public Bitmap? MainImage => LoadImage(CurrentMainImagePath);

    private Bitmap? LoadImage(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path)) return null;
            var uri = new Uri(path);
            return new Bitmap(AssetLoader.Open(uri));
        }
        catch
        {
            return null;
        }
    }

    public void SetMainImage(string imagePath) => CurrentMainImagePath = imagePath;

    public List<string> Images { get; set; } = new List<string>();
    private List<Bitmap?> _loadedImages = new();
    public List<Bitmap?> LoadedImages
    {
        get
        {
            if (_loadedImages.Count == 0 && Images.Count > 0)
            {
                foreach (var path in Images)
                {
                    _loadedImages.Add(LoadImage(path));
                }
            }
            return _loadedImages;
        }
    }

    private string _buttonText = "добавить себе";
    public string ButtonText
    {
        get => _buttonText;
        set { _buttonText = value; OnPropertyChanged(); }
    }

    private string _buttonColor = "#A8D514";
    public string ButtonColor
    {
        get => _buttonColor;
        set { _buttonColor = value; OnPropertyChanged(); }
    }

    // ========== ИСПРАВЛЕННЫЙ МЕТОД ==========
    public void UpdateButtonState(DatabaseService databaseService)
    {
        if (databaseService.IsGameAdded(Id))
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

    public string ExecutableName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;

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

    private GameInstaller? _installer;
    public GameInstaller Installer => _installer ??= new GameInstaller(Name, DownloadUrl, ExecutableName);

    public void RefreshInstallationState()
    {
        try
        {
            IsGameInstalled = Installer.IsInstalled;
        }
        catch
        {
            IsGameInstalled = false;
        }
    }

    public void DeleteGame()
    {
        Installer.DeleteGameFiles();
        RefreshInstallationState();
    }

    private string _tags = string.Empty;
    public string Tags
    {
        get => _tags;
        set
        {
            _tags = value;
            OnPropertyChanged();
            ParseTags();
        }
    }

    private bool _tag0;
    public bool Tag0
    {
        get => _tag0;
        set { _tag0 = value; OnPropertyChanged(); }
    }

    private bool _tag1;
    public bool Tag1
    {
        get => _tag1;
        set { _tag1 = value; OnPropertyChanged(); }
    }

    private bool _tag2;
    public bool Tag2
    {
        get => _tag2;
        set { _tag2 = value; OnPropertyChanged(); }
    }

    private bool _tag3;
    public bool Tag3
    {
        get => _tag3;
        set { _tag3 = value; OnPropertyChanged(); }
    }

    private void ParseTags()
    {
        Tag0 = false;
        Tag1 = false;
        Tag2 = false;
        Tag3 = false;

        if (string.IsNullOrEmpty(Tags)) return;

        var parts = Tags.Split(',');
        foreach (var part in parts)
        {
            if (int.TryParse(part.Trim(), out int index))
            {
                switch (index)
                {
                    case 0: Tag0 = true; break;
                    case 1: Tag1 = true; break;
                    case 2: Tag2 = true; break;
                    case 3: Tag3 = true; break;
                }
            }
        }
    }
}