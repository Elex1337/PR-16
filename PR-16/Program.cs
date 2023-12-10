using System;
using System.IO;

class Program
{
    static string watchedDirectory;
    static string logFilePath;

    static void Main()
    {
        Console.WriteLine("Добро пожаловать в приложение логирования изменений в файлах!");

        ConfigureWatcher();

        Console.WriteLine("Нажмите любую клавишу для завершения...");
        Console.ReadKey();
    }

    static void ConfigureWatcher()
    {
        Console.WriteLine("Введите путь к отслеживаемой директории:");
        watchedDirectory = Console.ReadLine();

        Console.WriteLine("Введите путь к лог-файлу:");
        logFilePath = Console.ReadLine();

        try
        {
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = watchedDirectory;

                watcher.NotifyFilter = NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;

                watcher.EnableRaisingEvents = true;

                Console.WriteLine($"Отслеживание изменений в директории {watchedDirectory}. Нажмите Q для выхода.");

                while (Console.ReadKey().Key != ConsoleKey.Q) ;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static void OnChanged(object source, FileSystemEventArgs e)
    {
        string changeType = e.ChangeType.ToString();
        string logMessage = $"{DateTime.Now} - {changeType} - {e.FullPath}";

        WriteToLog(logMessage);
    }

    static void OnRenamed(object source, RenamedEventArgs e)
    {
        string logMessage = $"{DateTime.Now} - Renamed - {e.OldFullPath} to {e.FullPath}";

        WriteToLog(logMessage);
    }

    static void WriteToLog(string logMessage)
    {
        try
        {
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.WriteLine(logMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при записи в лог-файл: {ex.Message}");
        }
    }
}
