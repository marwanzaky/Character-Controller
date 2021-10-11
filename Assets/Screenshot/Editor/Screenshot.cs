using UnityEngine;
using UnityEditor;
using System.IO;

public class Screenshot : EditorWindow
{
    bool useKeyCode = false;
    KeyCode keyCode = KeyCode.F12;
    string fileName = "Screenshot";

    [MenuItem("Tools/Screenshot")]
    static void Init()
    {
        var window = (Screenshot)EditorWindow.GetWindow(typeof(Screenshot));
        window.Show();
    }

    void Update()
    {
        if (useKeyCode)
        {
            if (Input.GetKeyDown(keyCode))
                Shot();

            Repaint();
        }
    }

    void OnGUI()
    {
        ScreenshotSettings();

        OtherSettings();
    }

    void ScreenshotSettings()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        HotKeys();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Take a Screenshot"))
            Shot();

        if (GUILayout.Button("Select Folder"))
            SelectFilePath();

        GUILayout.EndHorizontal();
    }

    void OtherSettings()
    {
        GUILayout.Label("Other Settings", EditorStyles.boldLabel);

        fileName = EditorGUILayout.TextField("File Name", fileName);
    }

    void HotKeys()
    {
        useKeyCode = EditorGUILayout.BeginToggleGroup("Use Key Code", useKeyCode);

        keyCode = (KeyCode)EditorGUILayout.EnumPopup("Key Code", keyCode);

        EditorGUILayout.EndToggleGroup();
    }

    void Shot()
    {
        if (string.IsNullOrEmpty(FilePath))
            SelectFilePath();

        if (string.IsNullOrEmpty(FilePath))
            return;

        const string FILE_FORMAT = ".png";
        var fullName = $"{fileName + Index + FILE_FORMAT}";

        ScreenCapture.CaptureScreenshot(Path.Combine(FilePath, fullName));

        Index++;
    }

    void SelectFilePath()
    {
        FilePath = EditorUtility.OpenFolderPanel("Select a folder", "", "Screenshot");
    }

    int Index
    {
        get
        {
            const int DEFAULT_VALUE = 1;
            return PlayerPrefs.GetInt("screenshot-index", DEFAULT_VALUE);
        }
        set
        {
            PlayerPrefs.SetInt("screenshot-index", value);
        }
    }

    string FilePath
    {
        get
        {
            const string defaultValue = "";
            return PlayerPrefs.GetString("screenshot-path", defaultValue);
        }

        set
        {
            PlayerPrefs.SetString("screenshot-path", value);
        }
    }
}