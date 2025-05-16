using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

[ExecuteAlways]
public class CameraZoneTrigger : MonoBehaviour
{
	[Header("References")]
	public Transform player;
	public CinemachineCamera idleCam;
	public CinemachineCamera zoneCam;
	[Tooltip("Le matériau dont le shader contient _scaleUVCartoon et _fade")]
	public Material cartoonMaterial;

	[Header("Zones")]
	public float placementDistance = 2f;

	[Header("Blend Settings")]
	[Tooltip("Durée du blend et de l’animation shader")]
	public float blendDuration = 1f;

	[Header("Shader Settings")]
	[Tooltip("Valeur mini du scale dans le shader")]
	public float minScale = 0.3f;
	[Tooltip("Valeur maxi du scale dans le shader")]
	public float maxScale = 1f;
	[Tooltip("Courbe d’animation du scale UV (0→1) relative à blendDuration")]
	public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	[Tooltip("Courbe d’animation du fade (0→1) relative à blendDuration")]
	public AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private bool _inZone = false;
	private Coroutine _currentTween;

	void Start()
	{
		if (idleCam == null || zoneCam == null || cartoonMaterial == null)
			Debug.LogWarning("Assignez idleCam, zoneCam et cartoonMaterial !");

		idleCam.Priority = 10;
		zoneCam.Priority = 5;
		// Valeurs initiales du shader
		cartoonMaterial.SetFloat("_scaleUVCartoon", minScale);
		cartoonMaterial.SetFloat("_fade", _inZone ? 1f : 0f);
	}

	void Update()
	{
		if (player == null) return;

		float dist = Vector3.Distance(player.position, transform.position);

		if (!_inZone && dist <= placementDistance)
		{
			SwitchCamAndShader(true);
			_inZone = true;
		}
		//else if (_inZone && dist > placementDistance)
		//{
		//	SwitchCamAndShader(false);
		//	_inZone = false;
		//}
	}

	private void SwitchCamAndShader(bool toZone)
	{
		// Bascule la priorité de la zoneCam
		if (toZone)
			zoneCam.Priority = idleCam.Priority + 1;
		else
			zoneCam.Priority = idleCam.Priority - 1;

		// Stoppe l’éventuel tween en cours
		if (_currentTween != null)
			StopCoroutine(_currentTween);

		// Lance le nouveau tween shader
		float fromScale = toZone ? minScale : maxScale;
		float toScale = toZone ? maxScale : minScale;
		float fromFade = toZone ? 0f : 1f;
		float toFade = toZone ? 1f : 0f;
		_currentTween = StartCoroutine(AnimateShader(fromScale, toScale, fromFade, toFade));
	}

	private IEnumerator AnimateShader(float fromScale, float toScale, float fromFade, float toFade)
	{
		float elapsed = 0f;
		while (elapsed < blendDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / blendDuration);

			// Appliquer les courbes séparément
			float curveScale = scaleCurve.Evaluate(t);
			float curveFade = fadeCurve.Evaluate(t);

			float vScale = Mathf.LerpUnclamped(fromScale, toScale, curveScale);
			float vFade = Mathf.LerpUnclamped(fromFade, toFade, curveFade);

			cartoonMaterial.SetFloat("_scaleUVCartoon", vScale);
			cartoonMaterial.SetFloat("_fade", vFade);

			yield return null;
		}

		// Garantir les valeurs finales exactes
		cartoonMaterial.SetFloat("_scaleUVCartoon", toScale);
		cartoonMaterial.SetFloat("_fade", toFade);
		_currentTween = null;
	}
}
