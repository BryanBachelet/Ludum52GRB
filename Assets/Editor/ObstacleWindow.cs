using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObstacleWindow : EditorWindow
{
    [MenuItem("Tools/Grid")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<ObstacleWindow>();
        wnd.titleContent = new GUIContent("Grid Window");
        // Limit size of the window
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
       
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Build Grid"))
        {
            SetInterface();
        }
    }

    private void SetInterface()
    {
        GridElement[] elements = Object.FindObjectsOfType<GridElement>();
        GridManager manager = Object.FindObjectOfType<GridManager>();
        if(manager == null)
        {
            Debug.LogError("You need to have grid manager in your scene to use this feature");
            return;
        }
        manager.SetGridElement(elements);
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].SetGridManager(manager);
        }

        manager.BuildGrid();

    }
}
