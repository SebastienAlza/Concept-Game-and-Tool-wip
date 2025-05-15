using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Caméra pixel-perfect qui suit une target sur le plan XZ,
/// avec offset et snapping, tout en gardant sa rotation.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class PixelPerfectCameraController : MonoBehaviour
{
	public CinemachineCamera cinemachineCamera;

	[Header("Target à suivre")]
	public Transform target;

	[Header("Offset monde (XZ)")]
	public Vector2 offsetXZ = Vector2.zero;

	[Header("Résolution logique (pixels)")]
	public int targetWidth = 320;
	public int targetHeight = 180;

	[Header("Pixels par unité Unity")]
	public float pixelsPerUnit = 16f;

	[Header("Assignation")]
	public RenderTexture renderTexture;
	public Material MatFullScreen;

	private Camera cam;
	private int currentWidth;
	private int currentHeight;

	public float UnitsPerPixel => (cam.orthographicSize * 2f) / targetHeight;

	void Awake()
	{
		InitializeCamera();
		ApplyResolution(targetWidth, targetHeight);
	}

	void OnValidate()
	{
		InitializeCamera();
		ApplyResolution(targetWidth, targetHeight);
	}

	void LateUpdate()
	{
		if (Application.isPlaying && target != null && cinemachineCamera != null)
		{
			FollowTargetXZWithOffset();
			SnapCameraToGridAccurate();
		}
	}

	void InitializeCamera()
	{
		if (cam == null)
			cam = GetComponent<Camera>();
		cam.orthographic = true;
	}

	public void ApplyResolution(int width, int height)
	{
		targetWidth = width;
		targetHeight = height;

		float unitsPerPixel = 1f / pixelsPerUnit;
		float orthoSize = (targetHeight * 0.5f) * unitsPerPixel;
		cam.orthographicSize = orthoSize;

#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			renderTexture = new RenderTexture(width, height, 0)
			{
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp
			};
			cam.targetTexture = renderTexture;
			if (MatFullScreen != null)
				MatFullScreen.SetTexture("_rt", renderTexture);

			currentWidth = width;
			currentHeight = height;
			return;
		}
#endif

		if (renderTexture != null)
		{
			if (renderTexture.width != width || renderTexture.height != height)
				renderTexture.Release();
		}

		renderTexture = new RenderTexture(width, height, 0)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		renderTexture.Create();
		cam.targetTexture = renderTexture;

		if (MatFullScreen != null)
			MatFullScreen.SetTexture("_rt", renderTexture);

		currentWidth = width;
		currentHeight = height;
	}

	void FollowTargetXZWithOffset()
	{
		Vector3 targetPos = target.position;
		Vector3 camPos = cinemachineCamera.transform.position;

		// Appliquer offset en XZ monde
		Vector3 offsetWorld = new Vector3(offsetXZ.x, 0f, offsetXZ.y);
		Vector3 finalPos = new Vector3(targetPos.x, camPos.y, targetPos.z) + offsetWorld;

		cinemachineCamera.transform.position = finalPos;
	}

	[Header("Ajustement Z si besoin")]
	public float zSnapCorrectionFactor = 1f; // Commence à 1, teste 0.9, 1.1 etc.

	void SnapCameraToGridAccurate()
	{
		// Taille d’un pixel en unités caméra
		float unitsPerPixel = (cam.orthographicSize * 2f) / targetHeight;

		// Prendre la position de la caméra cible
		Vector3 worldPos = cinemachineCamera.transform.position;

		// Transformer en espace caméra
		Vector3 cameraLocalPos = cam.worldToCameraMatrix.MultiplyPoint(worldPos);

		// Snapper dans l’espace caméra (sur X et Y car vue perspective ou ortho)
		cameraLocalPos.x = Mathf.Round(cameraLocalPos.x / unitsPerPixel) * unitsPerPixel;
		cameraLocalPos.y = Mathf.Round(cameraLocalPos.y / unitsPerPixel) * unitsPerPixel;

		// Transformer de nouveau vers le monde
		Vector3 snappedWorldPos = cam.cameraToWorldMatrix.MultiplyPoint(cameraLocalPos);

		// Appliquer
		cinemachineCamera.transform.position = snappedWorldPos;
	}

	public Vector3 SnapWorldXZToPixelGrid(Vector3 worldPos)
	{
		float unitsPerPixel = UnitsPerPixel;

		// On snap visuellement la position en XZ via l’espace caméra
		Vector3 cameraLocal = cam.worldToCameraMatrix.MultiplyPoint(worldPos);

		cameraLocal.x = Mathf.Round(cameraLocal.x / unitsPerPixel) * unitsPerPixel;
		cameraLocal.y = Mathf.Round(cameraLocal.y / unitsPerPixel) * unitsPerPixel;

		Vector3 snappedWorld = cam.cameraToWorldMatrix.MultiplyPoint(cameraLocal);

		// On garde le Y d'origine pour ne pas "tomber sous le sol"
		snappedWorld.y = worldPos.y;
		return snappedWorld;
	}



	public void SetResolution(int width, int height)
	{
		if (width != currentWidth || height != currentHeight)
			ApplyResolution(width, height);
	}
}
