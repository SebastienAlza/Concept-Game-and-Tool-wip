using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DebugPixel3DController : MonoBehaviour
{
	[Header("Mouvement")]
	public float moveSpeed = 3f;
	public Camera pixelCam;                         // votre caméra orthographique
	public PixelPerfect3DCamera ppCam;              // référence au script PixelPerfect3DCamera

	CharacterController cc;

	void Start()
	{
		cc = GetComponent<CharacterController>();
		if (!pixelCam || !ppCam)
			Debug.LogWarning("Assignez pixelCam et ppCam dans l'Inspector !");
	}

void Update()
{
    int dx = 0, dz = 0;
    if (Input.GetKey(KeyCode.LeftArrow))  dx = -1;
    if (Input.GetKey(KeyCode.RightArrow)) dx = +1;
    if (Input.GetKey(KeyCode.UpArrow))    dz = +1;
    if (Input.GetKey(KeyCode.DownArrow))  dz = -1;

    Vector3 move = new Vector3(dx, 0, dz) * moveSpeed;
    cc.Move(move * Time.deltaTime);

    // rotation vers la direction
    if (dx != 0 || dz != 0)
        transform.rotation = Quaternion.LookRotation(new Vector3(dx, 0, dz));

        // snap ultra-simple
        if (ppCam)
        {
            transform.position = PixelSnap(transform.position);
        }
        else
        {
            transform.position = transform.position;

		}
}

Vector3 PixelSnap(Vector3 worldPos)
{
    float up = 1f / ppCam.zoom;
    worldPos.x = Mathf.Round(worldPos.x * ppCam.zoom) / ppCam.zoom;
    worldPos.z = Mathf.Round(worldPos.z * ppCam.zoom) / ppCam.zoom;
    return worldPos;
}
}
