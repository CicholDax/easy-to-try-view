using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using ETTView.UI;

public class UIViewHistoryDebugWindow : EditorWindow
{
    private Vector2 _scrollPosition;

    // ウィンドウを開くためのメニューオプションを追加
    [MenuItem("Window/UIView/Debug History Window")]
    public static void ShowWindow()
    {
        GetWindow<UIViewHistoryDebugWindow>("Debug History");
    }

    private void OnGUI()
    {
        var manager = UIViewManager.Instance;
        if (manager == null)
        {
            EditorGUILayout.LabelField("UIViewManager instance not found.");
            return;
        }

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);  // Begin the scroll view

        EditorGUILayout.LabelField("History Contents:", EditorStyles.boldLabel);
        foreach (var view in manager.History)
        {
            EditorGUILayout.LabelField(view != null ? view.name : "null");

            EditorGUI.indentLevel++;  // Increase the indent level

            // Display Popups opened in the current UIView
            EditorGUILayout.LabelField("Opened Popups:", EditorStyles.boldLabel);
            foreach (var popup in view.OpenedPopups)
            {
                EditorGUILayout.LabelField(popup != null ? popup.name : "null");
            }

            EditorGUILayout.Space(10);  // Add some space between sections

            // Display UIViewState history for the current UIView
            EditorGUILayout.LabelField("UIViewState History:", EditorStyles.boldLabel);
            foreach (var state in view.StateHistory)
            {
                EditorGUILayout.LabelField(state != null ? state.ToString() : "null");
            }

            EditorGUI.indentLevel--;  // Reset the indent level

            EditorGUILayout.Space(20);  // Add more space between each UIView's details
        }

        EditorGUILayout.EndScrollView();  // End the scroll view
    }
}