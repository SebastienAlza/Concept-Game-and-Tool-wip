using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DebugPixel3DController : MonoBehaviour
{
	[Header("Mouvement")]
	public float moveSpeed = 3f;

	[Header("Lien pixel-perfect")]
	public PixelPerfectCameraController pixelCamera; // Assigne ton contrôleur ici

	private CharacterController cc;

	private Vector3 internalPosition;

	void Start()
	{
		cc = GetComponent<CharacterController>();
		internalPosition = transform.position;
		if (!pixelCamera)
			Debug.LogWarning("Assigne PixelPerfectCameraController à ce personnage.");
	}

	[Header("Smoothing")]
	public float moveSmoothTime = 0.1f;
	private Vector3 currentDir = Vector3.zero;
	private Vector3 dirVelocity = Vector3.zero;

	void Update()
	{
		// 1) Input + lissage
		Vector3 targetDir = new Vector3(
			Input.GetAxisRaw("Horizontal"),
			0f,
			Input.GetAxisRaw("Vertical")
		).normalized;

		currentDir = Vector3.SmoothDamp(currentDir, targetDir, ref dirVelocity, moveSmoothTime);

		// 2) Calcul de la nouvelle internalPosition
		Vector3 move = currentDir * moveSpeed * Time.deltaTime;
		internalPosition += move;

		// 3) Rotation lissée
		if (currentDir.sqrMagnitude > 0.001f)
		{
			Quaternion targetRot = Quaternion.LookRotation(currentDir);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
		}

		// 4) Snap pour affichage    
		Vector3 displayPos = internalPosition;
		if (pixelCamera != null)
			displayPos = pixelCamera.SnapWorldXZToPixelGrid(displayPos);

		// 5) Appliquer au CharacterController
		Vector3 delta = displayPos - transform.position;
		cc.Move(delta);
	}


}
