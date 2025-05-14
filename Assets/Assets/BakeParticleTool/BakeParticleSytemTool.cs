using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[ExecuteInEditMode]
public class BakeParticleSystemTool : MonoBehaviour
{
	public Material overrideMaterial;
	public bool useOverrideMaterial = false;

	public enum ColorBakeMode { ParticleColor, StartColor, None }
	[Header("Color Baking")] public ColorBakeMode colorBakeMode = ColorBakeMode.ParticleColor;

#if UNITY_EDITOR
	[ContextMenu("Bake And Save Meshes (Including Sub-Emitters)")]
	public void BakeAndSave()
	{
		BakeRecursively(transform);
	}

	[ContextMenu("Clear Baked Meshes")]
	public void ClearBakedMeshes()
	{
		ClearRecursively(transform);
	}

	private void BakeRecursively(Transform root)
	{
		var ps = root.GetComponent<ParticleSystem>();
		if (ps != null)
		{
			// 1) Simulate parent system (includes sub-emitters particles)
			var em = ps.emission;
			em.enabled = true;
			if (!ps.isPlaying) ps.Play(true);
			float simTime = Mathf.Max(ps.main.duration, 1f);
			ps.Simulate(0f, true, true, true);
			ps.Simulate(simTime, true, true, true);

			// 2) Bake parent without further simulation
			BakeNoSim(ps);

			// 3) For each sub-emitter, create temp PS with parent modules
			var subs = ps.subEmitters;
			for (int i = 0; i < subs.subEmittersCount; i++)
			{
				var subPS = subs.GetSubEmitterSystem(i);
				if (subPS != null)
				{
					// Create temporary PS on sub-emitter GameObject
					var tempGO = new GameObject(subPS.name + "_TempPS");
					tempGO.transform.SetParent(subPS.transform, false);
					var tempPS = tempGO.AddComponent<ParticleSystem>();
					// Copy parent modules
					CopyMain(ps.main, tempPS.main);
					CopyEmission(ps.emission, tempPS.emission);
					// Optionally copy other modules here (Shape, VelocityOverLifetime...)

					// Simulate tempPS
					var tempEm = tempPS.emission;
					tempEm.enabled = true;
					if (!tempPS.isPlaying) tempPS.Play(true);
					tempPS.Simulate(0f, true, true, true);
					tempPS.Simulate(simTime, true, true, true);

					// Bake and destroy tempPS
					BakeNoSim(tempPS);
					DestroyImmediate(tempGO);
				}
			}

			// 4) Stop parent
			ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			em.enabled = false;
		}

		// Recurse into children hierarchy
		foreach (Transform child in root)
			BakeRecursively(child);
	}

	/// <summary>
	/// Bake a ParticleSystem into a mesh without simulating further.
	/// </summary>
	private void BakeNoSim(ParticleSystem ps)
	{
		if (ps == null)
			return;

		// Inject dummy if empty
		if (ps.particleCount == 0)
			InjectDummyParticles(ps, 10);

		string assetPath = GetFixedAssetPath(ps.transform);
		var result = Bake(ps, assetPath);
		if (result.mesh == null)
		{
			Debug.LogWarning($"Bake failed – no particles at {ps.name}.");
			return;
		}

		// Remove existing baked preview
		var existing = ps.transform.Find(ps.name + "_baked");
		if (existing != null)
			DestroyImmediate(existing.gameObject);

		// Create preview as child to preserve local transform
		var go = new GameObject(ps.name + "_baked");
		go.transform.SetParent(ps.transform, false);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

		var mf = go.AddComponent<MeshFilter>();
		mf.sharedMesh = result.mesh;
		var mr = go.AddComponent<MeshRenderer>();
		var sharedMat = useOverrideMaterial && overrideMaterial != null
			? overrideMaterial
			: ps.GetComponent<ParticleSystemRenderer>()?.sharedMaterial;
		mr.sharedMaterial = sharedMat;
		if (sharedMat != null && sharedMat.HasProperty("_UseBillboard"))
			sharedMat.SetFloat("_UseBillboard", 1f);
	}

	// Copy select MainModule settings
	private void CopyMain(ParticleSystem.MainModule src, ParticleSystem.MainModule dst)
	{
		dst.startSize = src.startSize;
		dst.startSpeed = src.startSpeed;
		dst.startColor = src.startColor;
		dst.duration = src.duration;
		dst.loop = src.loop;
		dst.simulationSpace = src.simulationSpace;
	}

	// Copy select EmissionModule settings
	private void CopyEmission(ParticleSystem.EmissionModule src, ParticleSystem.EmissionModule dst)
	{
		dst.enabled = src.enabled;
		dst.rateOverTime = src.rateOverTime;
		dst.rateOverDistance = src.rateOverDistance;
	}

	private void ClearRecursively(Transform root)
	{
		var ps = root.GetComponent<ParticleSystem>();
		if (ps != null)
		{
			var em = ps.emission;
			em.enabled = true;
			ps.Clear(true);
			ps.Play(true);

			var smr = ps.GetComponent<ParticleSystemRenderer>();
			if (smr.sharedMaterial != null && smr.sharedMaterial.HasProperty("_UseBillboard"))
				smr.sharedMaterial.SetFloat("_UseBillboard", 0f);
		}

		var baked = root.Find(root.name + "_baked");
		if (baked != null)
			DestroyImmediate(baked.gameObject);

#if UNITY_EDITOR
		string assetPath = GetFixedAssetPath(root);
		if (AssetDatabase.LoadAssetAtPath<Mesh>(assetPath) != null)
			AssetDatabase.DeleteAsset(assetPath);
		string folder = Path.GetDirectoryName(assetPath);
		if (Directory.Exists(folder) && Directory.GetFiles(folder).Length == 0 && Directory.GetDirectories(folder).Length == 0)
			AssetDatabase.DeleteAsset(folder);
#endif

		foreach (Transform child in root)
			ClearRecursively(child);
	}

	private string GetFixedAssetPath(Transform t)
	{
		string rootName = transform.root.name;
		string folder = $"Assets/BakedParticles/{rootName}";
		if (!Directory.Exists(folder))
			Directory.CreateDirectory(folder);
		return $"{folder}/{t.name}_baked.asset";
	}

	private void InjectDummyParticles(ParticleSystem ps, int count)
	{
		var arr = new ParticleSystem.Particle[count];
		for (int i = 0; i < count; i++)
		{
			arr[i].position = Random.insideUnitSphere * 0.5f;
			arr[i].startSize = 1f;
			arr[i].startColor = Color.white;
			arr[i].remainingLifetime = 1f;
			arr[i].startLifetime = 1f;
		}
		ps.SetParticles(arr, arr.Length);
	}

	public struct BakeResult { public Mesh mesh; public bool useBillboard; }

	public BakeResult Bake(ParticleSystem ps, string assetPath = null)
	{
		var psr = ps.GetComponent<ParticleSystemRenderer>();
		bool hasMesh = psr.renderMode == ParticleSystemRenderMode.Mesh && psr.mesh != null;
		bool useBB = !hasMesh;

		var particles = new ParticleSystem.Particle[ps.main.maxParticles];
		int count = ps.GetParticles(particles);
		if (count == 0)
			return new BakeResult { mesh = null, useBillboard = true };

		var verts = new List<Vector3>();
		var tris = new List<int>();
		var uv0 = new List<Vector2>();
		var uv1 = new List<Vector4>();
		var cols = colorBakeMode == ColorBakeMode.None ? null : new List<Color>();

		if (hasMesh)
		{
			var src = psr.mesh;
			for (int i = 0; i < count; i++)
			{
				var p = particles[i];
				var mat = Matrix4x4.TRS(p.position, Quaternion.Euler(p.rotation3D), Vector3.one * p.GetCurrentSize(ps));
				int baseV = verts.Count;
				for (int v = 0; v < src.vertexCount; v++)
				{
					verts.Add(mat.MultiplyPoint3x4(src.vertices[v]));
					uv0.Add(src.uv.Length > 0 ? src.uv[v] : Vector2.zero);
					uv1.Add(new Vector4(p.position.x, p.position.y, p.position.z, 1f));
					if (cols != null)
					{
						Color c = colorBakeMode == ColorBakeMode.ParticleColor
							? p.GetCurrentColor(ps)
							: ps.main.startColor.color;
						cols.Add(c);
					}
				}
				foreach (var t in src.triangles)
					tris.Add(baseV + t);
			}
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				var p = particles[i];
				var r = p.position;
				float s = p.GetCurrentSize(ps) * 0.5f;
				int baseV = verts.Count;
				verts.Add(r + new Vector3(-s, -s));
				verts.Add(r + new Vector3(s, -s));
				verts.Add(r + new Vector3(s, s));
				verts.Add(r + new Vector3(-s, s));
				uv0.AddRange(new[] { Vector2.zero, Vector2.right, Vector2.one, Vector2.up });
				for (int j = 0; j < 4; j++) uv1.Add(new Vector4(r.x, r.y, r.z, 1f));
				if (cols != null)
				{
					Color c = colorBakeMode == ColorBakeMode.ParticleColor
						? p.GetCurrentColor(ps)
						: ps.main.startColor.color;
					cols.AddRange(new[] { c, c, c, c });
				}
				tris.AddRange(new[] { baseV, baseV + 1, baseV + 2, baseV + 2, baseV + 3, baseV });
			}
		}

		var mesh = new Mesh { name = ps.name + "_baked", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
		mesh.SetVertices(verts);
		mesh.SetUVs(0, uv0);
		mesh.SetUVs(1, uv1);
		mesh.SetTriangles(tris, 0);
		if (cols != null) mesh.SetColors(cols);
		mesh.RecalculateNormals();

#if UNITY_EDITOR
		if (!string.IsNullOrEmpty(assetPath))
		{
			var ex = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
			if (ex != null) EditorUtility.CopySerialized(mesh, ex);
			else AssetDatabase.CreateAsset(mesh, assetPath);
			AssetDatabase.SaveAssets();
		}
#endif

		return new BakeResult { mesh = mesh, useBillboard = useBB };
	}
#endif
}
