using System;                      // Базовые типы (string, int, Exception и т.д.)
using System.Diagnostics;          // Для отладки (Debug.WriteLine)
using System.IO;                   // Работа с файлами и папками (Directory, File, Path)
using System.IO.Compression;       // Работа с ZIP-архивами (ZipFile)
using System.Net.Http;             // HTTP-запросы (скачивание файлов)
using System.Threading.Tasks;      // Асинхронные операции (async/await)

namespace Launcher.Services;       // Пространство имён сервисов

/// <summary>
/// Класс GameInstaller отвечает за установку, запуск, проверку и удаление игры.
/// Каждая игра имеет свой экземпляр этого класса.
/// </summary>
public class GameInstaller
{
    // ========== ПОЛЯ КЛАССА ==========

    private readonly string _installPath;      // Полный путь к папке, куда установлена игра
    private readonly string _downloadUrl;      // Ссылка на ZIP-архив с игрой (GitHub)
    private readonly string _executableName;   // Имя исполняемого файла (например, "Launcher.exe")

    // ========== КОНСТРУКТОР ==========
    // Вызывается при создании установщика для конкретной игры
    public GameInstaller(string gameName, string downloadUrl, string executableName)
    {
        // 1. Получаем папку "Мои документы" пользователя
        //    Например: "C:\Users\Имя\Documents"
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // 2. Строим путь для установки игры:
        //    "Мои документы\MyLauncher\Games\НазваниеИгры"
        _installPath = Path.Combine(documentsPath, "MyLauncher", "Games", SanitizeFolderName(gameName));

        // 3. Сохраняем ссылку на скачивание
        _downloadUrl = downloadUrl;

        // 4. Сохраняем имя exe файла
        _executableName = executableName;
    }

    // ========== ВСПОМОГАТЕЛЬНЫЙ МЕТОД ==========

    /// <summary>
    /// Удаляет недопустимые символы из названия папки.
    /// Например: "Лаунчер" -> "Лаунчер", но "Игра: версия" -> "Игра_версия"
    /// </summary>
    private static string SanitizeFolderName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())  // Перебираем все запрещённые символы
        {
            name = name.Replace(c, '_');                   // Заменяем их на подчёркивание
        }
        return name;
    }

    // ========== ПРОВЕРКА УСТАНОВКИ ==========

    /// <summary>
    /// Проверяет, установлена ли игра (существует ли папка и exe файл).
    /// Используется для определения текста кнопки: "Скачать" или "Запустить"
    /// </summary>
    public bool IsInstalled
    {
        get
        {
            // 1. Проверяем, существует ли папка с игрой
            bool dirExists = Directory.Exists(_installPath);

            // Отладочный вывод (показываем путь)
            System.Diagnostics.Debug.WriteLine("=== GAME INSTALLER DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"_installPath: {_installPath}");
            System.Diagnostics.Debug.WriteLine($"Directory exists: {dirExists}");

            // 2. Если папки нет → игра точно не установлена
            if (!dirExists)
            {
                System.Diagnostics.Debug.WriteLine("Directory does not exist - returning false");
                System.Diagnostics.Debug.WriteLine("============================");
                return false;
            }

            // 3. Если папка есть → ищем внутри неё exe файл
            string exePath = GetExecutablePath();
            bool fileExists = File.Exists(exePath);

            System.Diagnostics.Debug.WriteLine($"_executableName: {_executableName}");
            System.Diagnostics.Debug.WriteLine($"exePath: {exePath}");
            System.Diagnostics.Debug.WriteLine($"File exists: {fileExists}");
            System.Diagnostics.Debug.WriteLine("============================");

            // 4. Возвращаем true, только если И папка И exe файл существуют
            return fileExists;
        }
    }

    // ========== ПОИСК EXE ФАЙЛА ==========

    /// <summary>
    /// Возвращает полный путь к исполняемому файлу игры.
    /// Ищет exe сначала в корне папки, потом во всех подпапках.
    /// </summary>
    public string GetExecutablePath()
    {
        // 1. Если папки с игрой вообще нет → возвращаем прямой путь (он не существует)
        if (!Directory.Exists(_installPath))
        {
            return Path.Combine(_installPath, _executableName);
        }

        // 2. Сначала ищем exe прямо в корне папки
        //    Например: "MyLauncher\Games\Лаунчер\Launcher.exe"
        string directPath = Path.Combine(_installPath, _executableName);
        if (File.Exists(directPath))
        {
            return directPath;
        }

        // 3. Если не нашли в корне → ищем во всех подпапках рекурсивно
        //    Например: "MyLauncher\Games\Лаунчер\exe_Test_Launcher\Launcher.exe"
        var files = Directory.GetFiles(_installPath, _executableName, SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            return files[0];  // Возвращаем первый найденный exe
        }

        // 4. Если ничего не нашли → возвращаем путь, который всё равно не существует
        return directPath;
    }

    // ========== ЗАПУСК ИГРЫ ==========

    /// <summary>
    /// Запускает игру, если она установлена.
    /// Возвращает true, если запуск удался, иначе false.
    /// </summary>
    public bool Launch()
    {
        // 1. Проверяем, установлена ли игра
        if (!IsInstalled) return false;

        // 2. Получаем путь к exe файлу
        string exePath = GetExecutablePath();

        // 3. Определяем рабочую папку (где лежит exe)
        //    Нужно, чтобы игра могла найти свои DLL и другие файлы
        string workingDirectory = Path.GetDirectoryName(exePath);

        System.Diagnostics.Debug.WriteLine($"Launch: exePath={exePath}");
        System.Diagnostics.Debug.WriteLine($"Launch: workingDirectory={workingDirectory}");

        // 4. Настраиваем запуск процесса
        var processStartInfo = new ProcessStartInfo
        {
            FileName = exePath,                              // Какой файл запускать
            UseShellExecute = true,                          // Использовать оболочку Windows
            WorkingDirectory = workingDirectory              // Рабочая папка = папка с exe
        };

        // 5. Запускаем процесс
        Process.Start(processStartInfo);
        return true;
    }

    // ========== СКАЧИВАНИЕ И УСТАНОВКА ==========

    /// <summary>
    /// Асинхронно скачивает игру с GitHub и устанавливает её.
    /// progress — объект для отчёта о прогрессе (0-100%).
    /// </summary>
    public async Task InstallAsync(IProgress<int> progress)
    {
        // 1. Создаём папку для установки (если её ещё нет)
        Directory.CreateDirectory(_installPath);

        // 2. Создаём временный ZIP-файл с уникальным именем
        //    Пример: "C:\Users\Имя\AppData\Local\Temp\a1b2c3d4-e5f6.zip"
        string zipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

        try
        {
            // 3. Скачиваем ZIP-файл из интернета
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();  // Если статус не 200 — выбрасывает исключение
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;  // Общий размер файла
                var downloadedBytes = 0L;            // Сколько уже скачали

                // Читаем поток и записываем в файл
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];     // Буфер для чтения (8KB)
                    int bytesRead;
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        // Вычисляем процент и передаём в UI
                        if (totalBytes > 0)
                        {
                            var percent = (int)((double)downloadedBytes / totalBytes * 100);
                            progress?.Report(percent);
                        }
                    }
                }
            }

            // 4. Небольшая задержка, чтобы файл освободился после записи
            await Task.Delay(100);

            // 5. Распаковываем ZIP-архив в папку установки
            ZipFile.ExtractToDirectory(zipPath, _installPath, true);
            progress?.Report(100);  // Установка завершена
        }
        catch (Exception ex)
        {
            // Если ошибка → удаляем папку, чтобы не осталось мусора
            try { Uninstall(); } catch { }
            throw new Exception($"Ошибка установки игры: {ex.Message}", ex);
        }
        finally
        {
            // 6. Удаляем временный ZIP-файл (всегда, даже при ошибке)
            try
            {
                if (File.Exists(zipPath))
                    File.Delete(zipPath);
            }
            catch { }
        }
    }

    // ========== УДАЛЕНИЕ ==========

    /// <summary>
    /// Удаляет игру с диска (всю папку со всем содержимым).
    /// </summary>
    public void Uninstall()
    {
        if (Directory.Exists(_installPath))
        {
            Directory.Delete(_installPath, true);  // true = удалять рекурсивно (все файлы и папки)
        }
    }

    /// <summary>
    /// Удаляет игру с диска с обработкой ошибок и отладочным выводом.
    /// </summary>
    public void DeleteGameFiles()
    {
        if (Directory.Exists(_installPath))
        {
            try
            {
                Directory.Delete(_installPath, true);
                System.Diagnostics.Debug.WriteLine($"Игра удалена: {_installPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления: {ex.Message}");
                throw;  // Пробрасываем исключение выше
            }
        }
    }
}