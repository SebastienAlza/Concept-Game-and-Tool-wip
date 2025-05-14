using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimonCombatGame : MonoBehaviour
{
	[Header("UI Elements")]
	public TextMeshProUGUI infoText; // Texte pour afficher les messages du jeu
	public Slider playerHealthBar; // Barre de vie du joueur
	public Slider enemyHealthBar; // Barre de vie de l'ennemi
	public Button attackButton; // Bouton d'attaque
	public Button restartButton; // Bouton Recommencer

	[Header("Simon Game Settings")]
	public GameObject buttonPrefab; // Prefab pour créer les boutons
	public Transform buttonContainer; // Conteneur pour les boutons (Panel dans UI)
	public int numberOfButtons = 4; // Nombre de boutons
	public List<Button> buttons = new List<Button>(); // Liste des boutons générés
	public List<Color> buttonColors = new List<Color>(); // Couleurs pour chaque bouton

	[Header("Player Settings")]
	public GameObject playerCube; // Cube représentant le joueur
	public Transform playerStartPosition; // Position de départ du joueur
	public Transform playerAttackPosition; // Position d'attaque du joueur
	public Transform playerDodgePosition; // Position latérale pour l'esquive du joueur
	public int maxPlayerHealth = 3; // Vie maximale du joueur
	public int playerHealth = 3; // Vie actuelle du joueur

	[Header("Enemy Settings")]
	public GameObject enemyCube; // Cube représentant l'ennemi
	public Transform enemyStartPosition; // Position de départ de l'ennemi
	public Transform enemyAttackPosition; // Position d'attaque de l'ennemi
	public Transform enemyDodgePosition; // Position latérale pour l'esquive de l'ennemi
	public int maxEnemyHealth = 3; // Vie maximale de l'ennemi
	public int enemyHealth = 3; // Vie actuelle de l'ennemi

	private List<int> sequence = new List<int>(); // Séquence Simon
	private int playerIndex = 0; // Progression dans la séquence
	private bool isPlayerTurn = false; // Indique si c'est au joueur de jouer

	void Start()
	{
		// Initialisation
		playerCube.transform.position = playerStartPosition.position;
		enemyCube.transform.position = enemyStartPosition.position;

		InitializeButtonColors();
		CreateButtons();

		attackButton.onClick.AddListener(StartPlayerAttack);
		restartButton.onClick.AddListener(RestartGame);

		attackButton.gameObject.SetActive(true);
		restartButton.gameObject.SetActive(false);

		UpdateHealthBars();
		infoText.text = "Appuyez sur 'Attack' pour commencer.";
	}

	void InitializeButtonColors()
	{
		buttonColors.Clear();
		buttonColors.Add(Color.red);
		buttonColors.Add(Color.blue);
		buttonColors.Add(Color.green);
		buttonColors.Add(Color.yellow);

		while (buttonColors.Count < numberOfButtons)
		{
			buttonColors.Add(new Color(Random.value, Random.value, Random.value));
		}
	}

	void CreateButtons()
	{
		buttons.Clear();

		for (int i = 0; i < numberOfButtons; i++)
		{
			GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
			newButton.name = $"Button_{i + 1}";

			Button buttonComponent = newButton.GetComponent<Button>();
			if (buttonComponent != null)
			{
				buttons.Add(buttonComponent);
				Image buttonImage = buttonComponent.GetComponent<Image>();
				if (buttonImage != null)
				{
					buttonImage.color = buttonColors[i] * 0.8f;
				}

				int index = i;
				buttonComponent.onClick.AddListener(() => OnButtonClicked(index));
			}
		}
	}

	void StartPlayerAttack()
	{
		attackButton.gameObject.SetActive(false);
		sequence.Clear();
		playerIndex = 0;

		for (int i = 0; i < Random.Range(2, 5); i++)
		{
			sequence.Add(Random.Range(0, buttons.Count));
		}

		Debug.Log("Séquence joueur : " + string.Join(", ", sequence));
		StartCoroutine(PlaySequence(() => StartCoroutine(CheckPlayerAttack())));
	}

	IEnumerator CheckPlayerAttack()
	{
		isPlayerTurn = true;
		infoText.text = "Répétez la séquence pour attaquer l'ennemi !";
		yield return new WaitUntil(() => !isPlayerTurn);

		if (playerIndex == sequence.Count)
		{
			StartCoroutine(PlayerAttack());
		}
		else
		{
			infoText.text = "L'ennemi a esquivé votre attaque !";
			yield return StartCoroutine(EnemyDodge());
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator PlayerAttack()
	{
		infoText.text = "Attaque réussie !";
		yield return StartCoroutine(MoveObject(playerCube.transform, playerAttackPosition.position, 0.5f));

		enemyHealth -= 1;
		UpdateHealthBars();

		yield return StartCoroutine(FlashColor(enemyCube, Color.red));
		yield return StartCoroutine(MoveObject(playerCube.transform, playerStartPosition.position, 0.5f));

		if (enemyHealth > 0)
		{
			StartCoroutine(EnemyTurn());
		}
		else
		{
			infoText.text = "Vous avez gagné !";
			restartButton.gameObject.SetActive(true);
		}
	}

	IEnumerator EnemyTurn()
	{
		infoText.text = "Tour de l'ennemi...";
		yield return new WaitForSeconds(1f);

		sequence.Clear();
		for (int i = 0; i < Random.Range(2, 5); i++)
		{
			sequence.Add(Random.Range(0, buttons.Count));
		}

		Debug.Log("Séquence ennemi : " + string.Join(", ", sequence));
		yield return StartCoroutine(PlaySequence(() => StartCoroutine(CheckPlayerDefense())));
	}

	IEnumerator CheckPlayerDefense()
	{
		isPlayerTurn = true;
		playerIndex = 0; // Réinitialiser l'index du joueur
		infoText.text = "Défendez-vous contre l'attaque !";

		// Attendre que le joueur termine la séquence Simon
		yield return new WaitUntil(() => !isPlayerTurn);

		// Afficher l'attaque ennemie (indépendamment du succès ou de l'échec)
		yield return StartCoroutine(MoveObject(enemyCube.transform, enemyAttackPosition.position, 0.5f));

		if (playerIndex == sequence.Count)
		{
			// Si la séquence est entièrement réussie
			infoText.text = "Vous avez esquivé l'attaque !";
			yield return StartCoroutine(PlayerDodge());
		}
		else
		{
			// Si la séquence est incorrecte ou incomplète
			infoText.text = "Vous avez été touché !";
			playerHealth -= 1;
			UpdateHealthBars();

			yield return StartCoroutine(FlashColor(playerCube, Color.red));
		}

		// Retour de l'ennemi à sa position initiale
		yield return StartCoroutine(MoveObject(enemyCube.transform, enemyStartPosition.position, 0.5f));

		// Vérifier si le joueur est encore en vie
		if (playerHealth > 0)
		{
			attackButton.gameObject.SetActive(true); // Activer le bouton d'attaque pour le joueur
		}
		else
		{
			infoText.text = "Vous avez perdu !";
			restartButton.gameObject.SetActive(true); // Activer le bouton de redémarrage
		}
	}

	IEnumerator PlayerDodge()
	{
		yield return StartCoroutine(MoveObject(playerCube.transform, playerDodgePosition.position, 0.3f));
		yield return StartCoroutine(MoveObject(playerCube.transform, playerStartPosition.position, 0.3f));
	}

	IEnumerator EnemyDodge()
	{
		yield return StartCoroutine(MoveObject(enemyCube.transform, enemyDodgePosition.position, 0.3f));
		yield return StartCoroutine(MoveObject(enemyCube.transform, enemyStartPosition.position, 0.3f));
	}

	void OnButtonClicked(int buttonIndex)
	{
		if (!isPlayerTurn) return;

		StartCoroutine(HandleButtonClickAnimation(buttonIndex));

		if (playerIndex < sequence.Count && buttonIndex == sequence[playerIndex])
		{
			// Si le clic est correct, passer à l'élément suivant de la séquence
			playerIndex++;

			// Si le joueur a terminé toute la séquence
			if (playerIndex == sequence.Count)
			{
				isPlayerTurn = false; // Fin du tour du joueur
			}
		}
		else
		{
			// Si le clic est incorrect
			isPlayerTurn = false;
		}
	}

	IEnumerator PlaySequence(System.Action onComplete)
	{
		infoText.text = "Regardez la séquence...";
		for (int i = 0; i < sequence.Count; i++)
		{
			HighlightButton(sequence[i]);
			yield return new WaitForSeconds(0.5f);
			ResetButton(sequence[i]);
			yield return new WaitForSeconds(0.2f);
		}

		onComplete?.Invoke();
	}

	void HighlightButton(int index)
	{
		buttons[index].GetComponent<Image>().color = buttonColors[index];
	}

	void ResetButton(int index)
	{
		buttons[index].GetComponent<Image>().color = buttonColors[index] * 0.8f;
	}

	IEnumerator HandleButtonClickAnimation(int buttonIndex)
	{
		buttons[buttonIndex].GetComponent<Image>().color = buttonColors[buttonIndex];
		yield return StartCoroutine(ScaleButton(buttons[buttonIndex].transform, 1.1f, 0.1f));
		buttons[buttonIndex].GetComponent<Image>().color = buttonColors[buttonIndex] * 0.8f;
		yield return StartCoroutine(ScaleButton(buttons[buttonIndex].transform, 1.0f, 0.1f));
	}

	IEnumerator ScaleButton(Transform buttonTransform, float targetScale, float duration)
	{
		Vector3 initialScale = buttonTransform.localScale;
		Vector3 targetScaleVector = Vector3.one * targetScale;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			buttonTransform.localScale = Vector3.Lerp(initialScale, targetScaleVector, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		buttonTransform.localScale = targetScaleVector;
	}

	IEnumerator MoveObject(Transform obj, Vector3 target, float duration)
	{
		Vector3 start = obj.position;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			obj.position = Vector3.Lerp(start, target, elapsed / duration);
			elapsed += Time.deltaTime;
			yield return null;
		}

		obj.position = target;
	}

	IEnumerator FlashColor(GameObject obj, Color flashColor)
	{
		Renderer renderer = obj.GetComponent<Renderer>();
		Color originalColor = renderer.material.color;

		for (int i = 0; i < 4; i++)
		{
			renderer.material.color = flashColor;
			yield return new WaitForSeconds(0.2f);
			renderer.material.color = originalColor;
			yield return new WaitForSeconds(0.2f);
		}
	}

	void UpdateHealthBars()
	{
		playerHealthBar.value = (float)playerHealth / maxPlayerHealth;
		enemyHealthBar.value = (float)enemyHealth / maxEnemyHealth;
	}

	void RestartGame()
	{
		playerHealth = maxPlayerHealth;
		enemyHealth = maxEnemyHealth;
		sequence.Clear();
		UpdateHealthBars();
		attackButton.gameObject.SetActive(true);
		restartButton.gameObject.SetActive(false);
		infoText.text = "Appuyez sur 'Attack' pour commencer.";
		playerCube.transform.position = playerStartPosition.position;
		enemyCube.transform.position = enemyStartPosition.position;
	}
}
