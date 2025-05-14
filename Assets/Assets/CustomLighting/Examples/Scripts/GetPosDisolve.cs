using UnityEngine;

[ExecuteAlways]
public class GetPosDisolve : MonoBehaviour
{
	public Transform player;

	[Header("Dissolve Settings")]
	[Tooltip("Rayon de base (minimum)")]
	public float baseRadius = 1f;
	[Tooltip("Rayon maximal à atteindre en placement réussi")]
	public float targetRadius = 3f;
	[Tooltip("Vitesse de croissance du rayon (unités/sec)")]
	public float growthSpeed = 1f;

	[Header("Bounce Settings")]
	[Tooltip("Distance à partir de laquelle on commence le bounce")]
	public float bounceTriggerDistance = 5f;
	[Tooltip("Amplitude du bounce autour de baseRadius")]
	public float bounceAmplitude = 0.5f;
	[Tooltip("Vitesse du bounce (cycles/sec)")]
	public float bounceSpeed = 2f;

	[Header("Placement Zone")]
	[Tooltip("Distance pour considérer la zone de bon placement")]
	public float placementDistance = 2f;

	// État interne
	private enum State { Idle, Bouncing, Growing, Done }
	private State _state = State.Idle;
	private float _currentRadius;

	void OnEnable()
	{
		_currentRadius = baseRadius;
		_state = State.Idle;
	}

	void Update()
	{
		if (player == null)
			return;

		float dist = Vector3.Distance(player.position, transform.position);

		switch (_state)
		{
			case State.Idle:
				if (dist <= bounceTriggerDistance && dist > placementDistance)
					_state = State.Bouncing;
				else if (dist <= placementDistance)
					_state = State.Growing;
				break;

			case State.Bouncing:
				// effet sinusoidal autour de baseRadius
				_currentRadius = baseRadius + Mathf.Sin(Time.time * Mathf.PI * 2f * bounceSpeed) * bounceAmplitude;
				if (dist <= placementDistance)
					_state = State.Growing;
				else if (dist > bounceTriggerDistance)
				{
					_state = State.Idle;
					_currentRadius = baseRadius;
				}
				break;

			case State.Growing:
				// fait grandir le rayon vers targetRadius
				_currentRadius = Mathf.MoveTowards(_currentRadius, targetRadius, growthSpeed * Time.deltaTime);
				if (_currentRadius >= targetRadius)
				{
					_currentRadius = targetRadius;
					_state = State.Done;
				}
				break;

			case State.Done:
				// reste bloqué à targetRadius
				_currentRadius = targetRadius;
				break;
		}

		// Applique au shader
		Shader.SetGlobalVector("_wposDisolve", transform.position);
		Shader.SetGlobalFloat("_radiusDisolve", _currentRadius);
		Debug.Log(_currentRadius);
	}

	private void OnDrawGizmos()
	{
		// Rayon courant
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawWireSphere(transform.position, _currentRadius);

		// Optionnel : affiche les zones de trigger
		Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
		Gizmos.DrawWireSphere(transform.position, bounceTriggerDistance);
		Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
		Gizmos.DrawWireSphere(transform.position, placementDistance);
	}
}
