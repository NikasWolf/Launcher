using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Launcher.Models;

namespace Launcher.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService()
    {
        string dbPath = Path.Combine(AppContext.BaseDirectory, "Launcher.db");
        _connectionString = $"Data Source={dbPath}";
    }

    // ========== GAMES ==========
    public List<Game> LoadGames()
    {
        var games = new List<Game>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Games";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            games.Add(new Game
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Genre = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Condition = reader.IsDBNull(4) ? "" : reader.GetString(4),
                Developer = reader.IsDBNull(5) ? "" : reader.GetString(5),
                AgeRest = reader.IsDBNull(6) ? "" : reader.GetString(6),
                Year = reader.IsDBNull(7) ? "" : reader.GetString(7),
                Icon = reader.IsDBNull(8) ? "" : reader.GetString(8),
                ImagePath = reader.IsDBNull(9) ? "" : reader.GetString(9),
                ExecutableName = reader.IsDBNull(10) ? "" : reader.GetString(10),
                DownloadUrl = reader.IsDBNull(11) ? "" : reader.GetString(11),
                Tags = reader.IsDBNull(12) ? "" : reader.GetString(12)
            });
        }

        foreach (var game in games)
        {
            var imageCommand = connection.CreateCommand();
            imageCommand.CommandText = "SELECT ImagePath FROM GameImages WHERE GameId = @GameId ORDER BY ImageIndex";
            imageCommand.Parameters.AddWithValue("@GameId", game.Id);

            using var imageReader = imageCommand.ExecuteReader();
            game.Images = new List<string>();
            while (imageReader.Read())
            {
                game.Images.Add(imageReader.GetString(0));
            }
        }

        return games;
    }

    // ========== USER GAMES ==========
    public List<int> LoadUserGameIds()
    {
        var ids = new List<int>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT GameId FROM UserGames";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ids.Add(reader.GetInt32(0));
        }

        return ids;
    }

    public bool IsGameAdded(int gameId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM UserGames WHERE GameId = @GameId";
        command.Parameters.AddWithValue("@GameId", gameId);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public void AddUserGame(int gameId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO UserGames (GameId) VALUES (@GameId)";
        command.Parameters.AddWithValue("@GameId", gameId);
        command.ExecuteNonQuery();
    }

    public void RemoveUserGame(int gameId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM UserGames WHERE GameId = @GameId";
        command.Parameters.AddWithValue("@GameId", gameId);
        command.ExecuteNonQuery();
    }
}