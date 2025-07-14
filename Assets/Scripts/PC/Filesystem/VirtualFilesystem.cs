using System.Collections.Generic;
using UnityEngine.Events;

public class VirtualFilesystem
{
    private readonly Dictionary<string, VirtualFile> files = new();

    public UnityEvent<string> OnCreateFile { get; } = new();
    public UnityEvent<string> OnDeleteFile { get; } = new();

    public bool Exists(string name)
        => files.ContainsKey(name);
    public VirtualFile Get(string name)
        => files.GetValueOrDefault(name);

    public VirtualFile Create(string name, VirtualFileContents contents)
    {
        if (Exists(name))
        {
            AppManager.Instance.ShowError($"File {name} already exists!");
            return Get(name);
        }

        var file = new VirtualFile
        {
            Name = name,
            Contents = contents
        };

        files[name] = file;

        OnCreateFile.Invoke(name);
        return file;
    }

    public IReadOnlyCollection<VirtualFile> AllFiles
        => files.Values;

    public void Delete(VirtualFile file)
    {
        files.Remove(file.Name);
        OnDeleteFile.Invoke(file.Name);
    }
}
