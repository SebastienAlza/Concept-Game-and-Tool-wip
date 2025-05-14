using UnityEngine;

public class ArcadeCarController : MonoBehaviour
{
	[Header("Wheel Colliders")]
	public WheelCollider frontLeftCollider;
	public WheelCollider frontRightCollider;
	public WheelCollider rearLeftCollider;
	public WheelCollider rearRightCollider;

	[Header("Wheel Meshes (Visuals)")]
	public Transform frontLeftWheel;
	public Transform frontRightWheel;
	public Transform rearLeftWheel;
	public Transform rearRightWheel;

	[Header("Car Settings")]
	public float motorForce = 1500f; // Force appliquée sur les roues motrices
	public float maxSteeringAngle = 30f; // Angle maximum de rotation des roues avant
	public float brakeForce = 3000f; // Force de freinage

	[Header("Suspension & Arcade Feel")]
	public float driftFactor = 0.9f; // Réduit l'adhérence pour un effet de drift
	public float suspensionHeight = 0.3f; // Hauteur de la suspension (via WheelCollider)

	private float currentBrakeForce;
	private bool isBraking;

	void Update()
	{
		HandleInput();
		UpdateWheels(); // Synchronise les colliders avec les roues visuelles
	}

	void FixedUpdate()
	{
		HandleMotor();
		HandleSteering();
		HandleBraking();
	}

	void HandleInput()
	{
		// Active le frein à main (Espace)
		isBraking = Input.GetKey(KeyCode.Space);
	}

	void HandleMotor()
	{
		float motorInput = Input.GetAxis("Vertical"); // Z/S ou joystick vertical
		float drift = isBraking ? driftFactor : 1f; // Réduit l'adhérence si on freine pour drifter

		// Applique la force motrice sur les roues arrière
		rearLeftCollider.motorTorque = motorInput * motorForce * drift;
		rearRightCollider.motorTorque = motorInput * motorForce * drift;

		// Réduit la friction latérale pour un effet glissant
		if (isBraking)
		{
			ApplyDrift(rearLeftCollider);
			ApplyDrift(rearRightCollider);
		}
		else
		{
			ResetFriction(rearLeftCollider);
			ResetFriction(rearRightCollider);
		}
	}

	void HandleSteering()
	{
		float steeringInput = Input.GetAxis("Horizontal"); // Q/D ou joystick horizontal
		float steeringAngle = steeringInput * maxSteeringAngle;

		// Applique l'angle de rotation aux roues avant
		frontLeftCollider.steerAngle = steeringAngle;
		frontRightCollider.steerAngle = steeringAngle;
	}

	void HandleBraking()
	{
		currentBrakeForce = isBraking ? brakeForce : 0f;

		// Applique la force de freinage sur toutes les roues
		ApplyBrake(frontLeftCollider);
		ApplyBrake(frontRightCollider);
		ApplyBrake(rearLeftCollider);
		ApplyBrake(rearRightCollider);
	}

	void ApplyBrake(WheelCollider collider)
	{
		collider.brakeTorque = currentBrakeForce;
	}

	void ApplyDrift(WheelCollider collider)
	{
		WheelFrictionCurve sidewaysFriction = collider.sidewaysFriction;
		sidewaysFriction.stiffness = driftFactor; // Réduit la rigidité pour glisser
		collider.sidewaysFriction = sidewaysFriction;
	}

	void ResetFriction(WheelCollider collider)
	{
		WheelFrictionCurve sidewaysFriction = collider.sidewaysFriction;
		sidewaysFriction.stiffness = 1f; // Remet la rigidité normale
		collider.sidewaysFriction = sidewaysFriction;
	}

	void UpdateWheels()
	{
		UpdateWheelPose(frontLeftCollider, frontLeftWheel);
		UpdateWheelPose(frontRightCollider, frontRightWheel);
		UpdateWheelPose(rearLeftCollider, rearLeftWheel);
		UpdateWheelPose(rearRightCollider, rearRightWheel);
	}

	void UpdateWheelPose(WheelCollider collider, Transform wheelTransform)
	{
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);

		wheelTransform.position = position;
		wheelTransform.rotation = rotation;
	}
}
