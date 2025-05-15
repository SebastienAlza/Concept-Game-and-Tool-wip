using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DebugPixel3DController : MonoBehaviour
{
	[Header("Mouvement")]
	public float moveSpeed = 3f;

	[Header("Lien pixel-perfect")]
	public PixelPerfectCameraController pixelCamera; // Assigne ton contrôleur ici

	private CharacterController cc;

	void Start()
	{
		cc = GetComponent<CharacterController>();
		if (!pixelCamera)
			Debug.LogWarning("Assigne PixelPerfectCameraController à ce personnage.");
	}

	void Update()
	{
		int dx = 0, dz = 0;
		if (Input.GetKey(KeyCode.A)) dx = -1;
		if (Input.GetKey(KeyCode.D)) dx = +1;
		if (Input.GetKey(KeyCode.W)) dz = +1;
		if (Input.GetKey(KeyCode.S)) dz = -1;

		Vector3 moveDir = new Vector3(dx, 0f, dz).normalized;

		if (moveDir != Vector3.zero)
		{
			Vector3 move = moveDir * moveSpeed * Time.deltaTime;
			Vector3 targetPos = transform.position + move;

			// Snap visuel en XZ (respecte Y réel)
			if (pixelCamera != null)
				targetPos = pixelCamera.SnapWorldXZToPixelGrid(targetPos);

			Vector3 delta = targetPos - transform.position;
			cc.Move(delta);

			transform.rotation = Quaternion.LookRotation(moveDir);
		}
	}

}
