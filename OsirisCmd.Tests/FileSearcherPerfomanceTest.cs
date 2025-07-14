using System.Collections.Concurrent;
using OsirisCmd.SearchingEngine;
using OsirisCmd.SettingsManager;
using Xunit.Abstractions;

namespace OsirisCmd.Tests;

public class FileSearcherPerfomanceTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public FileSearcherPerfomanceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void FileSystemCollectorTest()
    {
        var startTime = DateTime.Now;
        var result = new ConcurrentBag<string>();
        var dirs = new ConcurrentQueue<string>();

        dirs.Enqueue("C:\\");

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        Parallel.ForEach(Partitioner.Create(0, Environment.ProcessorCount * 1000), options, (range, state) =>
        {
            while (dirs.TryDequeue(out var currentDir))
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(currentDir))
                    {
                        result.Add(file);
                    }
                    foreach (var dir in Directory.EnumerateDirectories(currentDir)) {
                        dirs.Enqueue(dir);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error {e}");
                }
            }
        });

        var endTime = DateTime.Now;

        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Total files collected {result.Count()}");
        Console.WriteLine($"Time elapced {(endTime - startTime).TotalMinutes}");
        Console.WriteLine("--------------------------------");

    }

    [Fact]
    public void SecondFSCollecttorTest() {
        var startTime = DateTime.Now;
        var result = GetAllFilesRecursive("C:\\");
        var endTime = DateTime.Now;

        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Total files collected {result.Count()}");
        Console.WriteLine($"Time elapced {(endTime - startTime).TotalMinutes}");
        Console.WriteLine("--------------------------------");
    }

    private List<string> GetAllFilesRecursive(string directoryPath)
    {
        var allFiles = new List<string>();
        var queue = new Queue<string>();
        queue.Enqueue("C:\\");
        while (queue.Count > 0)
        {
            try {
                var currentDirectory = queue.Dequeue();
            
                var files = Directory.EnumerateFiles(currentDirectory);
                allFiles.AddRange(files);
                
                var subdirectories = Directory.EnumerateDirectories(currentDirectory);
                foreach (var subdirectory in subdirectories)
                {
                    queue.Enqueue(subdirectory);
                }
            } catch (Exception e) {
                Console.WriteLine($"Error {e}");
            }
        }
        return allFiles;
    }
}
