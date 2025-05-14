using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
	public float speed = 10f; // Vitesse de la balle
	private Vector3 direction;

	public void SetDirection(Vector3 newDirection)
	{
		direction = newDirection;
	}

	void Update()
	{
		// D�placer la balle dans la direction d�finie
		transform.Translate(direction * speed * Time.deltaTime);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			Destroy(collision.gameObject); // D�truire l'ennemi
			Destroy(gameObject); // D�truire la balle
		}
	}
}
