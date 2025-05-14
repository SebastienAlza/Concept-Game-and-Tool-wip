using UnityEngine;

public class ZoneManager : MonoBehaviour
{
	public int playerAttackPower = 10; // Puissance d'attaque du joueur
	public int currentZonePower = 5; // Puissance requise pour la zone actuelle
	public GameObject idleIndicator; // Indicateur visuel pour le mode idle

	void Update()
	{
		if (playerAttackPower >= currentZonePower)
		{
			idleIndicator.SetActive(true); // Mode idle actif
		}
		else
		{
			idleIndicator.SetActive(false); // Mode action requis
		}
	}

	public void ChangeZone(int newZonePower)
	{
		currentZonePower = newZonePower;
	}
}
