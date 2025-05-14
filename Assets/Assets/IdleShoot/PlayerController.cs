using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed = 5f; // Vitesse de déplacement

	void Update()
	{
		// Mouvement manuel
		float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime;
		transform.Translate(moveX, moveY, 0);
	}
}
