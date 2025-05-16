using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DebugPixel3DController : MonoBehaviour
{
	[Header("Mouvement")]
	public float moveSpeed = 3f;

	[Header("Lien pixel-perfect")]
	public PixelPerfectCameraController pixelCamera; // Assigne ton contrôleur ici

	[Header("Animation")]
	[Tooltip("Animator contenant les états Idle et Walk")]
	public Animator[] animators;
	private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

	private CharacterController cc;
	private Vector3 internalPosition;

	[Header("Smoothing")]
	public float moveSmoothTime = 0.1f;
	private Vector3 currentDir = Vector3.zero;
	private Vector3 dirVelocity = Vector3.zero;

	void Start()
	{
		cc = GetComponent<CharacterController>();
		internalPosition = transform.position;
		if (!pixelCamera)
			Debug.LogWarning("Assigne PixelPerfectCameraController à ce personnage.");
		if (animators == null)
			Debug.LogWarning("Assigne un Animator avec les bools 'isWalking'.");
	}

	void Update()
	{
		// 1) Lecture de l'input et lissage de la direction
		Vector3 targetDir = new Vector3(
			Input.GetAxisRaw("Horizontal"),
			0f,
			Input.GetAxisRaw("Vertical")
		).normalized;

		currentDir = Vector3.SmoothDamp(currentDir, targetDir, ref dirVelocity, moveSmoothTime);

		// 2) Calcul de la nouvelle position interne (float)
		Vector3 move = currentDir * moveSpeed * Time.deltaTime;
		internalPosition += move;

		// 3) Rotation lissée
		if (currentDir.sqrMagnitude > 0.001f)
		{
			Quaternion targetRot = Quaternion.LookRotation(currentDir);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
		}

		// 4) Application du snap pour affichage pixel-perfect
		Vector3 displayPos = internalPosition;
		if (pixelCamera != null)
			displayPos = pixelCamera.SnapWorldXZToPixelGrid(displayPos);

		Vector3 delta = displayPos - transform.position;
		cc.Move(delta);

		// 5) Mise à jour de l'Animator
		if (animators != null)
		{
			bool isWalking = currentDir.sqrMagnitude > 0.001f;
			foreach(Animator animator in animators)
			animator.SetBool(IsWalkingHash, isWalking);
		}
	}
}
