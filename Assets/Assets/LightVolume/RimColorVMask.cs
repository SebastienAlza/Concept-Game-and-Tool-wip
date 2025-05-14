using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RimColorVmask : MonoBehaviour
{
	public MaskTypeSelector MaskType = new MaskTypeSelector();

	[HideInInspector]
	public Vector3 boxSize;

	public float boxRound;

	public float boxSoftBorder;

	public float hardness = 1;

	public Color color = Color.black;



	[HideInInspector]
	public bool IsSphere = true;

	[HideInInspector]
	public bool IsBox = false;

	public enum MaskTypeSelector
	{
		Sphere,
		Box
	};
	private Vector3 currentBoxSize;

	private void OnDrawGizmosSelected()
	{
		// Display the explosion radius when selected

		if (MaskType == MaskTypeSelector.Sphere)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, boxSize.x - 0.5f);
		}
		else
		{
			Matrix4x4 gizmoMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.matrix = gizmoMatrix;
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(Vector3.zero, boxSize);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(Vector3.zero, boxSize + new Vector3(boxSoftBorder, boxSoftBorder, boxSoftBorder));
		}
	}

	private void OnDisable()
	{
		currentBoxSize = boxSize;
		boxSize = new Vector3(0,0,0);
		//SetProperties();
		RimColorVManager.Instance?.UpdateAMM();
	}

	private void OnEnable()
	{
		boxSize = currentBoxSize;
		InitVolume();
	}

	private void InitVolume()
	{
		SetProperties();
		RimColorVManager.Instance?.UpdateAMM();
	}

	public void OnValidate()
	{
		SetProperties();
		RimColorVManager.Instance?.UpdateAMM();
	}

	private void SetProperties()
	{
		//Update inspector display
		if (MaskType == MaskTypeSelector.Sphere)
		{
			IsSphere = true;
			IsBox = false;
		}
		else if (MaskType == MaskTypeSelector.Box)
		{
			IsSphere = false;
			IsBox = true;
		}

	}

	private void Update()
	{
		if (transform.hasChanged && RimColorVManager.Instance != null)
		{
			SetProperties();
			boxSize = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
			RimColorVManager.Instance?.UpdateAMM();
			transform.hasChanged = false;
		}
	}


//#if UNITY_EDITOR

//	[MenuItem("Tools/Refresh VolumeMask Shot %h")]
//	public static void Refresh()
//	{
//		RimColorVManager.Instance?.UpdateAMM();
//		Debug.Log("Refresh");
//	}
//#endif
}