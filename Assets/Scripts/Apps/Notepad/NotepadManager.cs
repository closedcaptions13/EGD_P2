using TMPro;
using UnityEngine;

public class NotepadManager : MonoBehaviour
{
    TMP_InputField notepad;

    string filename;
    AppRoot root;
    TextContents contents;
    bool isSaved;

    void Awake()
    {
        notepad = GetComponent<TMP_InputField>();
        notepad.onValueChanged.AddListener(OnChange);
        
        root = AppRoot.ForObject(this);
    }

    void OnChange(string value)
    {
        isSaved = false;
    }

    void Start()
    {
        var args = root.Arguments;
        filename = args[0] as string;

        var file = AppManager.Filesystem.Get(filename);
        contents = file.Contents as TextContents;

        notepad.text = contents.Value;
        isSaved = true;
    }

    string GetTitle()
    {
        var result = $"Notepad — {filename}";

        if (!isSaved)
            result += "*";

        return result;
    }

    void Update()
    {
        root.AssignedInstance.SetTitle(GetTitle());

        if (!AppRoot.SelectedForObject(this))
            return;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                contents.Value = notepad.text;
                isSaved = true;
            }
        }
    }
}
