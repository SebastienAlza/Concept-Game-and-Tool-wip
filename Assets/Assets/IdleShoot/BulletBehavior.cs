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
		// Déplacer la balle dans la direction définie
		transform.Translate(direction * speed * Time.deltaTime);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			Destroy(collision.gameObject); // Détruire l'ennemi
			Destroy(gameObject); // Détruire la balle
		}
	}
}
