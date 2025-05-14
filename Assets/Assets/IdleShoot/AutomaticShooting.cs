using System.Linq; // Pour les opérations sur les listes
using UnityEngine;

public class AutomaticShooting : MonoBehaviour
{
	public GameObject bulletPrefab; // Préfabriqué de la balle
	public Transform bulletSpawnPoint; // Point d'apparition des balles
	public float fireRate = 1f; // Temps entre les tirs
	private float fireCooldown = 0f;

	void Update()
	{
		fireCooldown -= Time.deltaTime;

		if (fireCooldown <= 0f)
		{
			// Trouver l'ennemi le plus proche
			GameObject nearestEnemy = FindNearestEnemy();

			if (nearestEnemy != null)
			{
				FireAtTarget(nearestEnemy.transform);
				fireCooldown = fireRate;
			}
		}
	}

	GameObject FindNearestEnemy()
	{
		// Cherche tous les objets avec le tag "Enemy"
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

		if (enemies.Length == 0)
			return null;

		// Trouve l'ennemi le plus proche
		GameObject nearestEnemy = null;
		float shortestDistance = Mathf.Infinity;

		foreach (GameObject enemy in enemies)
		{
			float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
			if (distanceToEnemy < shortestDistance)
			{
				shortestDistance = distanceToEnemy;
				nearestEnemy = enemy;
			}
		}

		return nearestEnemy;
	}

	void FireAtTarget(Transform target)
	{
		// Instancier la balle
		GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

		// Calculer la direction vers la cible
		Vector3 direction = (target.position - bulletSpawnPoint.position).normalized;

		// Appliquer la direction à la balle
		bullet.GetComponent<BulletBehavior>().SetDirection(direction);
	}
}
