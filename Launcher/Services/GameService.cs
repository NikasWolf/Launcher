using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Launcher.Models;

namespace Launcher.Services;

public class GameService
{
    // Путь к JSON файлу 
    private readonly string _filePath;

    // Конструктор 
    public GameService()
    {
        //папка, где находится программа (bin/Debug/net8.0/)
        string appDirectory = AppContext.BaseDirectory;

        // Объединяем путь к папке с именем файла "games.json"
        _filePath = Path.Combine(appDirectory, "games.json");
        // Выводим путь в окно Output
        System.Diagnostics.Debug.WriteLine("=========================================");
        System.Diagnostics.Debug.WriteLine($"AppContext.BaseDirectory: {appDirectory}");
        System.Diagnostics.Debug.WriteLine($"Полный путь к games.json: {_filePath}");
        System.Diagnostics.Debug.WriteLine($"Файл существует: {File.Exists(_filePath)}");
        System.Diagnostics.Debug.WriteLine("=========================================");
    }

    // Метод загрузки игр из JSON файла
    public ObservableCollection<Game> LoadGames()
    {
        // 1. Читаем ВЕСЬ текст из файла games.json
        string json = File.ReadAllText(_filePath);

        // 2. Преобразуем JSON строку в список объектов Game
        var games = JsonSerializer.Deserialize<List<Game>>(json);

        // 3. Возвращаем ObservableCollection<Game>
        return new ObservableCollection<Game>(games ?? new List<Game>());
    }

    // Метод сохранения игр в JSON файл
    public void SaveGames(ObservableCollection<Game> games)
    {
        // 1. Преобразуем список игр в JSON строку
        string json = JsonSerializer.Serialize(games, new JsonSerializerOptions
        {
            WriteIndented = true   //читаемый JSON 
        });

        // 2. Записываем JSON строку в файл (перезаписывая старый)
        File.WriteAllText(_filePath, json);
    }
}