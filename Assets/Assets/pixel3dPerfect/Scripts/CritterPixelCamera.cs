using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways, RequireComponent(typeof(Camera))]
public class PixelPerfect3DCamera : MonoBehaviour
{
	public enum FollowMode { XY, XZ }

	[Header("Cible (pan fluide)")]
	public Transform followTarget;       // ton perso ou ton CameraRig
	public FollowMode followAxis = FollowMode.XZ;
	[Tooltip("Hauteur fixe de la caméra en mode XZ")]
	public float cameraHeight = 10f;

	[Header("Résolution basse")]
	public int pixelWidth = 320;
	public int pixelHeight = 180;

	[Header("Zoom (entier)")]
	[Min(1)] public int zoom = 1;

	[Header("UI (Canvas sans CanvasScaler)")]
	public Canvas screenCanvas;
	public RawImage screenImage;

	Camera cam;
	RenderTexture rt;
	bool isSetup;
	Vector3 initialOffset;

	void Awake()
	{
		transform.SetParent(null, true);
		cam = GetComponent<Camera>();
		cam.orthographic = true;

		if (followTarget != null)
			initialOffset = transform.position - followTarget.position;

		Setup();
	}

	void OnEnable()
	{
		if (cam == null) cam = GetComponent<Camera>();
		isSetup = false;
		Setup();
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		isSetup = false;
		if (!Application.isPlaying) Setup();
	}
#endif

	void LateUpdate()
	{
		if (!isSetup || followTarget == null) return;

		// 1) Position world idéale
		Vector3 tgt = followTarget.position;
		Vector3 baseWorld = followAxis == FollowMode.XY
			? tgt + initialOffset
			: new Vector3(tgt.x + initialOffset.x, cameraHeight, tgt.z + initialOffset.z);

		// 2) Passage en espace local caméra
		Vector3 localPos = cam.transform.InverseTransformPoint(baseWorld);

		// 3) Calcul unités monde / pixel
		float worldH = cam.orthographicSize * 2f;
		float worldW = worldH * cam.aspect;
		float upx = worldW / pixelWidth;
		float upy = worldH / pixelHeight;

		// 4) Coordonnées flottantes en pixel-space
		float px = localPos.x / upx;
		float py = localPos.y / upy;

		// 5) Snap en pixel-space
		float snappedPx = Mathf.Round(px / zoom) * zoom;
		float snappedPy = Mathf.Round(py / zoom) * zoom;

		// 6) Reprojection en espace local
		localPos.x = snappedPx * upx;
		localPos.y = snappedPy * upy;

		// 7) Reprojection world et positionnement de la caméra
		Vector3 snappedWorld = cam.transform.TransformPoint(localPos);
		cam.transform.position = snappedWorld;

		// 8) Compensation sub-pixel via UV
		float offX = px - snappedPx;
		float offY = py - snappedPy;
		screenImage.uvRect = new Rect(
			offX / pixelWidth,
			offY / pixelHeight,
			1f, 1f
		);
	}

	void Setup()
	{
		if (isSetup) return;
		isSetup = true;

		// --- Cleanup ancienne RT ---
		if (rt != null)
		{
			cam.targetTexture = null;
			rt.Release();
			DestroyImmediate(rt);
		}

		// --- Création nouvelle RT basse résolution ---
		rt = new RenderTexture(pixelWidth, pixelHeight, 0)
		{
			filterMode = FilterMode.Point,
			useMipMap = false,
			autoGenerateMips = false,
			antiAliasing = 1
		};
		rt.Create();

		// --- Ajustement orthographicSize et targetTexture ---
		cam.orthographicSize = pixelHeight / (2f * zoom);
		cam.targetTexture = rt;

		// --- Canvas & RawImage (sans CanvasScaler) ---
		if (screenCanvas == null)
		{
			var go = new GameObject("PixelCanvas");
			go.transform.SetParent(null, false);
			screenCanvas = go.AddComponent<Canvas>();
			screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}
		if (screenImage == null)
		{
			var go = new GameObject("PixelScreen");
			go.transform.SetParent(screenCanvas.transform, false);
			screenImage = go.AddComponent<RawImage>();
		}

		// --- Configuration du RawImage full-stretch ---
		screenImage.texture = rt;
		RectTransform rtTf = screenImage.rectTransform;
		rtTf.anchorMin = Vector2.zero;
		rtTf.anchorMax = Vector2.one;
		rtTf.offsetMin = Vector2.zero;
		rtTf.offsetMax = Vector2.zero;
		screenImage.material = null;

		Debug.Log("[PixelPerfect3DCamera] Setup terminé");
	}

	void OnDestroy()
	{
		if (rt != null)
		{
			cam.targetTexture = null;
			rt.Release();
			DestroyImmediate(rt);
		}
	}
}
