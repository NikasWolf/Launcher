using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Launcher.Models;


namespace Launcher.Services;

public class GameService
{
    private readonly string _filePath;

    public GameService()
    {
        // Файл будет в папке с программой
        string appDirectory = AppContext.BaseDirectory;
        _filePath = Path.Combine(appDirectory, "games.json");
    }

    // Загрузить все игры
    public ObservableCollection<Game> LoadGames()
    {
        if (!File.Exists(_filePath))
        {
            // Создаём пример данных, если файла нет
            CreateSampleData();
        }

        string json = File.ReadAllText(_filePath);
        var games = JsonSerializer.Deserialize<List<Game>>(json);
        return new ObservableCollection<Game>(games ?? new List<Game>());
    }

    // Сохранить игры
    public void SaveGames(ObservableCollection<Game> games)
    {
        string json = JsonSerializer.Serialize(games, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_filePath, json);
    }

    // Создать пример данных для начала
    private void CreateSampleData()
    {
        var sampleGames = new List<Game>
        {
            new Game
            {
                Id = 1,
                Name = "Tanki Online",
                Description = "Танковый шутер с PvP-битвами",
                Genre = "Shooter",
                Year = 2009,
                ImagePath = "avares://Launcher/Assets/ImagePath/066.png",
                ExecutablePath = ""
            },
            new Game
            {
                Id = 2,
                Name = "Пример игры 2",
                Description = "Описание второй игры",
                Genre = "RPG",
                Year = 2020,
                ImagePath = "/Assets/games/game2.jpg",
                ExecutablePath = ""
            }
        };

        SaveGames(new ObservableCollection<Game>(sampleGames));
    }
}