using UnityEngine;
using Unity.Cinemachine;

[ExecuteAlways]
public class CameraZoneTrigger : MonoBehaviour
{
	public Transform player;
	public CinemachineCamera idleCam;
	public CinemachineCamera zoneCam;

	[Header("Zones")]
	public float placementDistance = 2f;

	[Header("Blend Settings")]
	[Tooltip("Seul le DefaultBlend du CinemachineBrain sert ici")]
	public float blendDuration = 1f;

	private bool _inZone;

	void Start()
	{
		// Assurez-vous que vos cams ont bien ces priorités initiales
		idleCam.Priority = 10;
		zoneCam.Priority = 5;
	}

	void Update()
	{
		if (player == null) return;
		float dist = Vector3.Distance(player.position, transform.position);

		if (!_inZone && dist <= placementDistance)
		{
			// Passe à la ZoneCam : sa priority devient > IdleCam.Priority
			zoneCam.Priority = idleCam.Priority + 1;
			_inZone = true;
		}
		//else if (_inZone && dist > placementDistance)
		//{
		//	// Revenu en zone idle
		//	zoneCam.Priority = 5;
		//	_inZone = false;
		//}
	}
}
