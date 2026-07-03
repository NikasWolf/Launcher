using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace Launcher.Models;

public class News
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;

    public Bitmap? NewsImage => LoadImage(ImagePath);
    public bool HasImage => !string.IsNullOrEmpty(ImagePath);

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
}