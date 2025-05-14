using UnityEngine;

[ExecuteAlways]
public class GetPosDissolve : MonoBehaviour
{
	public Transform player;

	[Header("Dissolve Settings")]
	public float baseRadius = 1f;
	public float targetRadius = 3f;
	public float growthSpeed = 1f;
	public AnimationCurve growthCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

	[Header("Bounce Settings")]
	public float bounceTriggerDistance = 5f;
	public float bounceAmplitude = 0.5f;
	public float bounceSpeed = 2f;
	[Tooltip("Vitesse à laquelle l'amplitude se rapproche de sa cible")]
	public float amplitudeDampingSpeed = 1f;

	[Header("Placement Zone")]
	public float placementDistance = 2f;

	private enum State { Idle, Bouncing, Growing, Done }
	private State _state = State.Idle;

	private float _currentRadius;
	private float _amplitudeCurrent;
	private float _growthElapsed;

	void OnEnable()
	{
		ResetAll();
	}

	void ResetAll()
	{
		_state = State.Idle;
		_currentRadius = baseRadius;
		_amplitudeCurrent = 0f;
		_growthElapsed = 0f;
	}

	void Update()
	{
		if (player == null) return;
		float dist = Vector3.Distance(player.position, transform.position);

		switch (_state)
		{
			case State.Idle:
				_currentRadius = baseRadius;
				_amplitudeCurrent = 0f;
				if (dist <= bounceTriggerDistance && dist > placementDistance)
				{
					_state = State.Bouncing;
					// On démarre le bounce avec amplitude à 0 → il montera tout de suite
					_amplitudeCurrent = 0f;
				}
				else if (dist <= placementDistance)
				{
					_state = State.Growing;
					_growthElapsed = 0f;
				}
				break;

			case State.Bouncing:
				// On choisit la cible d'amplitude
				float ampTarget = (dist <= bounceTriggerDistance && dist > placementDistance)
								  ? bounceAmplitude
								  : 0f;
				// On lisse l'amplitude
				_amplitudeCurrent = Mathf.MoveTowards(
					_amplitudeCurrent,
					ampTarget,
					amplitudeDampingSpeed * Time.deltaTime
				);
				// Calcul du radius avec sinus
				_currentRadius = baseRadius
					+ Mathf.Sin(Time.time * Mathf.PI * 2f * bounceSpeed)
					  * _amplitudeCurrent;

				// Transitions
				if (dist <= placementDistance)
				{
					_state = State.Growing;
					_growthElapsed = 0f;
				}
				else if (_amplitudeCurrent <= 0f && dist > bounceTriggerDistance)
				{
					// Amplitude retombée à 0 et en dehors → plus de bounce
					_state = State.Idle;
				}
				break;

			case State.Growing:
				// Durée pour aller de base→target à vitesse constante
				float duration = (targetRadius - baseRadius) / growthSpeed;
				_growthElapsed += Time.deltaTime;
				float t = Mathf.Clamp01(_growthElapsed / duration);
				// t non-lin via curve
				float tCurve = growthCurve.Evaluate(t);
				_currentRadius = Mathf.Lerp(baseRadius, targetRadius, tCurve);

				if (t >= 1f)
				{
					_state = State.Done;
					_currentRadius = targetRadius;
				}
				break;

			case State.Done:
				_currentRadius = targetRadius;
				break;
		}

		// Applique au shader
		Shader.SetGlobalVector("_wposDisolve", transform.position);
		Shader.SetGlobalFloat("_radiusDisolve", _currentRadius);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Gizmos.DrawWireSphere(transform.position, _currentRadius);

		Gizmos.color = new Color(1, 1, 0, 0.3f);
		Gizmos.DrawWireSphere(transform.position, bounceTriggerDistance);

		Gizmos.color = new Color(0, 1, 0, 0.3f);
		Gizmos.DrawWireSphere(transform.position, placementDistance);
	}
}
