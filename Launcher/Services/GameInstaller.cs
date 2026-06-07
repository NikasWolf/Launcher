using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Launcher.Services;

public class GameInstaller
{
    private readonly string _installPath;
    private readonly string _downloadUrl;
    private readonly string _executableName;

    public GameInstaller(string gameName, string downloadUrl, string executableName)
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _installPath = Path.Combine(documentsPath, "MyLauncher", "Games", SanitizeFolderName(gameName));
        _downloadUrl = downloadUrl;
        _executableName = executableName;
    }

    private static string SanitizeFolderName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }

    // ДОБАВЬ ЭТО СВОЙСТВО
    public bool IsInstalled
    {
        get
        {
            // Сначала проверяем, существует ли папка
            bool dirExists = Directory.Exists(_installPath);

            System.Diagnostics.Debug.WriteLine("=== GAME INSTALLER DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"_installPath: {_installPath}");
            System.Diagnostics.Debug.WriteLine($"Directory exists: {dirExists}");

            if (!dirExists)
            {
                System.Diagnostics.Debug.WriteLine("Directory does not exist - returning false");
                System.Diagnostics.Debug.WriteLine("============================");
                return false;
            }

            // Только если папка существует, ищем exe файл
            string exePath = GetExecutablePath();
            bool fileExists = File.Exists(exePath);

            System.Diagnostics.Debug.WriteLine($"_executableName: {_executableName}");
            System.Diagnostics.Debug.WriteLine($"exePath: {exePath}");
            System.Diagnostics.Debug.WriteLine($"File exists: {fileExists}");
            System.Diagnostics.Debug.WriteLine("============================");

            return fileExists;
        }
    }
    public string GetExecutablePath()
    {
        // Если папки не существует, сразу возвращаем прямой путь
        if (!Directory.Exists(_installPath))
        {
            return Path.Combine(_installPath, _executableName);
        }

        // Сначала проверяем корень папки
        string directPath = Path.Combine(_installPath, _executableName);
        if (File.Exists(directPath))
        {
            return directPath;
        }

        // Если не нашли - ищем рекурсивно во всех подпапках
        var files = Directory.GetFiles(_installPath, _executableName, SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            return files[0];
        }

        return directPath;
    }
    public bool Launch()
    {
        if (!IsInstalled) return false;

        var processStartInfo = new ProcessStartInfo
        {
            FileName = GetExecutablePath(),
            UseShellExecute = true,
            WorkingDirectory = _installPath
        };

        Process.Start(processStartInfo);
        return true;
    }

    public void Uninstall()
    {
        if (Directory.Exists(_installPath))
        {
            Directory.Delete(_installPath, true);
        }
    }

    public async Task InstallAsync(IProgress<int> progress)
    {
        // 1. Создаем папку для установки
        Directory.CreateDirectory(_installPath);

        // 2. Создаём временный файл с уникальным именем (чтобы избежать конфликтов)
        string zipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

        try
        {
            // 3. Скачиваем ZIP-файл
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var downloadedBytes = 0L;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        if (totalBytes > 0)
                        {
                            var percent = (int)((double)downloadedBytes / totalBytes * 100);
                            progress?.Report(percent);
                        }
                    }
                }
            }

            // 4. Небольшая задержка для освобождения файла
            await Task.Delay(100);

            // 5. Распаковываем архив
            ZipFile.ExtractToDirectory(zipPath, _installPath, true);
            progress?.Report(100);
        }
        catch (Exception ex)
        {
            // В случае ошибки удаляем папку установки
            try { Uninstall(); } catch { }
            throw new Exception($"Ошибка установки игры: {ex.Message}", ex);
        }
        finally
        {
            // 6. Удаляем временный ZIP-файл
            try
            {
                if (File.Exists(zipPath))
                    File.Delete(zipPath);
            }
            catch { }
        }
    }
    /// <summary>
    /// Удаляет игру с диска
    /// </summary>
    public void DeleteGameFiles()
    {
        if (Directory.Exists(_installPath))
        {
            try
            {
                Directory.Delete(_installPath, true); // true = рекурсивное удаление
                System.Diagnostics.Debug.WriteLine($"Игра удалена: {_installPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления: {ex.Message}");
                throw;
            }
        }
    }

}