#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(BakeParticleSystemTool))]
[CanEditMultipleObjects]
public class BakeParticleSystemToolInspector : Editor
{
	public override void OnInspectorGUI()
	{
		GUILayout.Space(5);
		EditorGUILayout.LabelField("🎨 Particle Baker Tool", EditorStyles.boldLabel);
		EditorGUILayout.HelpBox("Bake a ParticleSystem (and its sub-emitters) into static meshes.", MessageType.Info);
		GUILayout.Space(5);

		DrawDefaultInspector();
		GUILayout.Space(5);

		var bakers = targets.Cast<BakeParticleSystemTool>().ToList();

		GUI.backgroundColor = new Color(0.2f, 0.6f, 1f);
		if (bakers.Count == 1 && GUILayout.Button("🍞 Bake & Save", GUILayout.Height(30)))
			bakers[0].BakeAndSave();
		else if (bakers.Count > 1 && GUILayout.Button($"🍞 Bake & Save ({bakers.Count})", GUILayout.Height(30)))
			bakers.ForEach(b => b.BakeAndSave());

		GUI.backgroundColor = new Color(0.8f, 0.4f, 0.4f);
		if (bakers.Count == 1 && GUILayout.Button("🗑️ Clear Bake", GUILayout.Height(30)))
			bakers[0].ClearBakedMeshes();
		else if (bakers.Count > 1 && GUILayout.Button($"🗑️ Clear ({bakers.Count})", GUILayout.Height(30)))
			bakers.ForEach(b => b.ClearBakedMeshes());

		GUI.backgroundColor = Color.white;
	}
}
#endif
