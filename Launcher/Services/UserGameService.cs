using System;                      // Базовые типы (string, int, Exception)
using System.Collections.Generic;  // List<T>
using System.IO;                   // Работа с файлами (File, Path)
using System.Text.Json;            // Сериализация/десериализация JSON

namespace Launcher.Services;       // Пространство имён сервисов

/// Сервис для работы с user_games.json — файлом, который хранит ID игр,
/// добавленных пользователем в раздел "Моё".
public class UserGameService
{

    // Полный путь к файлу user_games.json
    private readonly string _filePath;


    /// Определяет путь к файлу user_games.json.
    /// Файл будет лежать рядом с EXE файлом программы.
    public UserGameService()
    {
        // AppContext.BaseDirectory — папка, где запущена программа
        string appDirectory = AppContext.BaseDirectory;

        // Объединяем путь к папке с именем файла
        _filePath = Path.Combine(appDirectory, "user_games.json");
    }

    // ========== ЗАГРУЗКА ==========
    public List<int> LoadUserGameIds()
    {
        // 1. Проверяем, существует ли файл
        if (!File.Exists(_filePath))
        {
            // Если файла нет → возвращаем пустой список
            return new List<int>();
        }

        // 2. Читаем весь текст из файла
        string json = File.ReadAllText(_filePath);

        // 3. Превращаем JSON строку в список чисел
        return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
    }

    // ========== СОХРАНЕНИЕ ==========
    public void SaveUserGameIds(List<int> ids)
    {
        // 1. Превращаем список чисел в JSON строку с отступами
        //    { 1, 3, 5 } → "[\n  1,\n  3,\n  5\n]"
        string json = JsonSerializer.Serialize(ids, new JsonSerializerOptions
        {
            WriteIndented = true   // Делает JSON красивым и читаемым
        });

        // 2. Записываем JSON в файл (перезаписывая старый)
        //    Если файла нет — создаётся новый
        File.WriteAllText(_filePath, json);
    }

    // ========== ОПЕРАЦИИ СО СПИСКОМ ==========

    /// Добавляет игру в список "Моё" по её ID.
    public void AddGame(int gameId)
    {
        // 1. Загружаем текущий список ID
        var ids = LoadUserGameIds();

        // 2. Проверяем, нет ли уже такого ID в списке
        //    Чтобы избежать дубликатов
        if (!ids.Contains(gameId))
        {
            // 3. Добавляем ID
            ids.Add(gameId);

            // 4. Сохраняем обновлённый список обратно в файл
            SaveUserGameIds(ids);
        }
    }

    /// Удаляет игру из списка "Моё" по её ID.
    public void RemoveGame(int gameId)
    {
        // 1. Загружаем текущий список ID
        var ids = LoadUserGameIds();

        // 2. Проверяем, есть ли такой ID в списке
        if (ids.Contains(gameId))
        {
            // 3. Удаляем ID
            ids.Remove(gameId);

            // 4. Сохраняем обновлённый список обратно в файл
            SaveUserGameIds(ids);
        }
    }

    /// Проверяет, добавлена ли игра в "Моё" по её ID.
    /// Возвращает true, если игра уже в списке.
    public bool IsGameAdded(int gameId)
    {
        // 1. Загружаем список ID
        var ids = LoadUserGameIds();

        // 2. Проверяем, есть ли искомый ID в списке
        return ids.Contains(gameId);
    }
}