using System;

namespace OsirisCmd.FileSystem;

public class FileSystemManager
{
    List<FileModel> GetFiles(string path)
    {
        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        var fileModels = new List<FileModel>();
        foreach (var directory in directories)
        {
            var dirInfo = new DirectoryInfo(directory);
            var dirModel = new FileModel
            {
                Name = dirInfo.Name,
                Extension = "<DIR>",
                Size = 0,
                TimeModified = dirInfo.LastWriteTime.Ticks
            };
            fileModels.Add(dirModel);
        }
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            var fileModel = new FileModel
            {
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                Size = fileInfo.Length,
                TimeModified = fileInfo.LastWriteTime.Ticks
            };
            fileModels.Add(fileModel);
        }

        return fileModels;
    }

    void CreateDirectory(string path)
    {
    }

    void CreateFile(string path)
    {
    }

    void DeleteFile(string path)
    {
    }

    void CopyFile(string sourcePath, string destinationPath)
    {
    }

    void MoveFile(string sourcePath, string destinationPath)
    {
    }

    void RenameFile(string oldPath, string newPath)
    {
    }
    
    
}