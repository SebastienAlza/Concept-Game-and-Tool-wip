using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemyPrefab; // Préfabriqué des ennemis
	public Camera mainCamera; // Référence à la caméra principale
	public float spawnOffset = 2f; // Distance supplémentaire pour que les ennemis spawnent en dehors du frustum
	public float spawnInterval = 2f; // Temps entre chaque spawn

	private float spawnTimer;

	void Start()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main; // Assigne automatiquement la caméra principale
		}

		spawnTimer = spawnInterval;
	}

	void Update()
	{
		spawnTimer -= Time.deltaTime;

		if (spawnTimer <= 0f)
		{
			SpawnEnemyOutsideFrustum();
			spawnTimer = spawnInterval;
		}
	}

	void SpawnEnemyOutsideFrustum()
	{
		// Obtenez les dimensions du frustum de la caméra
		Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));
		float cameraWidth = screenBounds.x * 2;
		float cameraHeight = screenBounds.y * 2;

		// Position aléatoire autour des bords de la caméra
		Vector3 spawnPosition = Vector3.zero;
		int side = Random.Range(0, 4); // 0 = haut, 1 = bas, 2 = gauche, 3 = droite

		switch (side)
		{
			case 0: // Haut
				spawnPosition = new Vector3(Random.Range(-cameraWidth / 2, cameraWidth / 2), screenBounds.y + spawnOffset, 0);
				break;
			case 1: // Bas
				spawnPosition = new Vector3(Random.Range(-cameraWidth / 2, cameraWidth / 2), -screenBounds.y - spawnOffset, 0);
				break;
			case 2: // Gauche
				spawnPosition = new Vector3(-screenBounds.x - spawnOffset, Random.Range(-cameraHeight / 2, cameraHeight / 2), 0);
				break;
			case 3: // Droite
				spawnPosition = new Vector3(screenBounds.x + spawnOffset, Random.Range(-cameraHeight / 2, cameraHeight / 2), 0);
				break;
		}

		// Instancier l'ennemi
		Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
	}
}
