using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
	public float speed = 2f; // Vitesse de d�placement de l'ennemi
	private Transform player; // R�f�rence au joueur

	void Start()
	{
		// Trouver le joueur via le tag "Player"
		player = GameObject.FindWithTag("Player").transform;
	}

	void Update()
	{
		// D�placement vers le joueur en limitant l'axe Z
		if (player != null)
		{
			// Calcul de la direction uniquement sur les axes X et Y
			Vector3 direction = new Vector3(player.position.x - transform.position.x, player.position.y - transform.position.y, 0).normalized;

			// D�placement de l'ennemi
			transform.Translate(direction * speed * Time.deltaTime, Space.World);

			// Assurez-vous que l'ennemi reste sur l'axe Z = 0
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			Destroy(gameObject); // D�truire l'ennemi en touchant le joueur
								 // Ajouter des d�g�ts au joueur ici
		}

		if (collision.gameObject.CompareTag("Bullet"))
		{
			Destroy(gameObject); // D�truire l'ennemi
			Destroy(collision.gameObject); // D�truire la balle
		}
	}
}
