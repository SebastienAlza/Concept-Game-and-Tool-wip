using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefabWindow : EditorWindow
{
	private GameObject prefabToReplace;

	[MenuItem("Tools/Replace Selected with Prefab")]
	public static void ShowWindow()
	{
		GetWindow<ReplaceWithPrefabWindow>("Replace With Prefab");
	}

	private void OnGUI()
	{
		GUILayout.Label("Replace Selected GameObjects with Prefab", EditorStyles.boldLabel);
		prefabToReplace = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToReplace, typeof(GameObject), false);

		if (GUILayout.Button("Replace Selected"))
		{
			if (prefabToReplace == null)
			{
				EditorUtility.DisplayDialog("Error", "Please assign a prefab!", "OK");
			}
			else
			{
				ReplaceSelected();
			}
		}
	}

	private void ReplaceSelected()
	{
		var selectedObjects = Selection.gameObjects;
		if (selectedObjects.Length == 0)
		{
			EditorUtility.DisplayDialog("Warning", "No GameObjects selected!", "OK");
			return;
		}

		// Start undo group
		int undoGroup = Undo.GetCurrentGroup();
		Undo.SetCurrentGroupName("Replace with Prefab");

		foreach (var oldObj in selectedObjects)
		{
			// Instantiate the prefab in the same scene
			GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToReplace, oldObj.scene);

			// Match transform
			newObj.transform.SetParent(oldObj.transform.parent);
			newObj.transform.localPosition = oldObj.transform.localPosition;
			newObj.transform.localRotation = oldObj.transform.localRotation;
			newObj.transform.localScale = oldObj.transform.localScale;

			// Register undo for creation
			Undo.RegisterCreatedObjectUndo(newObj, "Replace with Prefab");

			// Destroy the old object
			Undo.DestroyObjectImmediate(oldObj);
		}

		// Collapse undo operations into one
		Undo.CollapseUndoOperations(undoGroup);
	}
}
