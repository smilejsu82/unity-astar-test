using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorTestAstarWindow : EditorWindow
{
    public static void ShowWindow(string name)
    {
        GetWindow<EditorTestAstarWindow>(false, name, true);
    }
}
