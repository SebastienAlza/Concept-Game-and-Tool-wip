
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RimColorVmask))]
public class VolumeMaskCustomInspector : Editor
    {
        public int _volumeType = 1;

        public override void OnInspectorGUI()
        {
		RimColorVmask volumeMask = (RimColorVmask)target;

            //DrawDefaultInspector();
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Mask Types", EditorStyles.boldLabel);
            volumeMask.MaskType = (RimColorVmask.MaskTypeSelector)EditorGUILayout.EnumPopup("Type", volumeMask.MaskType);
            //Mask Type Custom Inspector
            switch (volumeMask.MaskType)
            {
                case RimColorVmask.MaskTypeSelector.Box:
                    volumeMask.boxRound = EditorGUILayout.FloatField("Box Round", volumeMask.boxRound);
                    volumeMask.boxSoftBorder = EditorGUILayout.FloatField("Box Softness", volumeMask.boxSoftBorder);
                    break;

                case RimColorVmask.MaskTypeSelector.Sphere:
                    volumeMask.hardness = EditorGUILayout.FloatField("Sphere Hardness", volumeMask.hardness);
                    break;
            }

            volumeMask.color = EditorGUILayout.ColorField("Color",volumeMask.color);

			if (EditorGUI.EndChangeCheck())
            {
                volumeMask.OnValidate();
                EditorUtility.SetDirty(volumeMask);
            }
        }
    }

public static class VolumeMaskEditor
{
	[MenuItem("GameObject/RimColorOverride/RimColorVMask", false, 10)]
	public static void CreateVolumeMask(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject();
		go.AddComponent<RimColorVmask>();
		go.name = "VolumeLight";
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}
}

