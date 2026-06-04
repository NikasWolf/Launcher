using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace Launcher.Models;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Year { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty;

    // Добавь это свойство
    public Bitmap? ImageBitmap => LoadImage();

    private Bitmap? LoadImage()
    {
        try
        {
            if (string.IsNullOrEmpty(ImagePath)) return null;
            var uri = new Uri(ImagePath);
            return new Bitmap(AssetLoader.Open(uri));
        }
        catch
        {
            return null;
        }
    }
}