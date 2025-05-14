#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BakeParticleSystemWindow : EditorWindow
{
	class Info { public ParticleSystem ps; public int level; public bool selected; }
	List<Info> list = new List<Info>();
	Vector2 scroll;
	bool showChildren, selectAll;

	[MenuItem("Tools/Particle Baker 🎨")]
	static void Open() => GetWindow<BakeParticleSystemWindow>("🎨 Particle Baker Tool").minSize = new Vector2(450, 300);

	void OnEnable() => Refresh();

	void OnGUI()
	{
		GUILayout.Space(5);
		EditorGUILayout.LabelField("🎨 Particle Baker Tool", EditorStyles.boldLabel);
		EditorGUILayout.HelpBox("Select and bake ParticleSystems (with sub-emitters).", MessageType.Info);
		GUILayout.Space(5);

		if (GUILayout.Button("🔄 Refresh List", GUILayout.Height(30))) Refresh();
		showChildren = EditorGUILayout.ToggleLeft("👀 Show Children", showChildren);
		GUILayout.Space(5);

		if (list.Count == 0) { EditorGUILayout.HelpBox("No ParticleSystems found.", MessageType.Warning); return; }

		GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
		if (GUILayout.Button("🍞 Bake Selected", GUILayout.Height(35))) BakeSelected();
		GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
		if (GUILayout.Button("🗑️ Clear Selected", GUILayout.Height(35))) ClearSelected();
		GUI.backgroundColor = Color.white;
		GUILayout.Space(5);

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button(selectAll ? "☑️ Unselect All" : "✅ Select All", GUILayout.Height(25)))
		{
			foreach (var i in list)
				if (showChildren || i.level == 0) i.selected = !selectAll;
			selectAll = !selectAll;
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(5);

		scroll = EditorGUILayout.BeginScrollView(scroll);
		foreach (var info in list)
		{
			if (info.ps == null) continue;
			if (!showChildren && info.level > 0) continue;
			EditorGUILayout.BeginHorizontal("box");
			GUILayout.Space(20 * info.level);
			info.selected = EditorGUILayout.Toggle(info.selected, GUILayout.Width(20));
			GUILayout.Label(info.ps.name, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
	}

	void Refresh()
	{
		list.Clear();
		foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
			Scan(root.transform, 0);
	}

	void Scan(Transform t, int lvl)
	{
		var ps = t.GetComponent<ParticleSystem>();
		if (ps != null) list.Add(new Info { ps = ps, level = lvl, selected = false });
		foreach (Transform c in t) Scan(c, lvl + 1);
	}

	void BakeSelected()
	{
		foreach (var info in list)
			if (info.selected && info.ps != null)
			{
				var tool = info.ps.GetComponent<BakeParticleSystemTool>()
						   ?? info.ps.gameObject.AddComponent<BakeParticleSystemTool>();
				tool.BakeAndSave();
				DestroyImmediate(tool);
			}
	}

	void ClearSelected()
	{
		foreach (var info in list)
			if (info.selected && info.ps != null)
			{
				var tool = info.ps.GetComponent<BakeParticleSystemTool>()
						   ?? info.ps.gameObject.AddComponent<BakeParticleSystemTool>();
				tool.ClearBakedMeshes();
				DestroyImmediate(tool);
			}
	}
}
#endif
