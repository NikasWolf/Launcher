using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Launcher.Services;

public class UserGameService
{
    private readonly string _filePath;

    public UserGameService()
    {
        string appDirectory = AppContext.BaseDirectory;
        _filePath = Path.Combine(appDirectory, "user_games.json");
    }

    public List<int> LoadUserGameIds()
    {
        if (!File.Exists(_filePath))
        {
            return new List<int>();
        }
        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
    }

    public void SaveUserGameIds(List<int> ids)
    {
        string json = JsonSerializer.Serialize(ids, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_filePath, json);
    }

    public void AddGame(int gameId)
    {
        var ids = LoadUserGameIds();
        if (!ids.Contains(gameId))
        {
            ids.Add(gameId);
            SaveUserGameIds(ids);
        }
    }

    public void RemoveGame(int gameId)
    {
        var ids = LoadUserGameIds();
        if (ids.Contains(gameId))
        {
            ids.Remove(gameId);
            SaveUserGameIds(ids);
        }
    }

    public bool IsGameAdded(int gameId)
    {
        var ids = LoadUserGameIds();
        return ids.Contains(gameId);
    }
}