using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
    public class RimColorVManager : MonoBehaviour
    {
        private const int MAXVOL = 8;

        private Vector4[] PosArr = new Vector4[MAXVOL];
        private Vector4[] RotArr = new Vector4[MAXVOL];
        private float[] rangeArr = new float[MAXVOL];
        private float[] hardnessArr = new float[MAXVOL];
        private float[] isSphereArr = new float[MAXVOL];
        private float[] boxRoundArr = new float[MAXVOL];
        private float[] boxSoftBorderArr = new float[MAXVOL];
        private Vector4[] boxSizeArr = new Vector4[MAXVOL];
        private Vector4[] ColorArr = new Vector4[MAXVOL];

        private static RimColorVManager s_instance = null;

        public static RimColorVManager Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = GameObject.FindFirstObjectByType<RimColorVManager>();
                return s_instance;
            }
        }

        public void UpdateAMM()
        {
            int indexMVolume = 0;
		RimColorVmask[] volumeMasks = FindObjectsByType<RimColorVmask>(FindObjectsInactive.Include, FindObjectsSortMode.None);

		if (volumeMasks.Length < MAXVOL && volumeMasks != null)
            {
                foreach (RimColorVmask volumeMask in volumeMasks)
                {
                        SendParamMaskVolume(volumeMask, indexMVolume);
                        indexMVolume++;
                }
                Shader.SetGlobalVectorArray("PosArr", PosArr);
                Shader.SetGlobalVectorArray("RotArr", RotArr);
                Shader.SetGlobalFloatArray("RangeArr", rangeArr);
                Shader.SetGlobalFloatArray("HardnessArr", hardnessArr);
                Shader.SetGlobalFloatArray("IsSphereArr", isSphereArr);
                Shader.SetGlobalFloatArray("BoxRoundArr", boxRoundArr);
                Shader.SetGlobalFloatArray("BoxSoftBorderArr", boxSoftBorderArr);
                Shader.SetGlobalVectorArray("BoxSizeArr", boxSizeArr);
                Shader.SetGlobalInt("VMCount", indexMVolume);

                Shader.SetGlobalVectorArray("ColorArr", ColorArr);
            }
            else
            {
                Debug.Log("MaskVolume > " + MAXVOL + " not supported");
            }
        }

        void SendParamMaskVolume(RimColorVmask volumeMask, int indexMVolume)
        {
            PosArr[indexMVolume] = volumeMask.transform.position;
            RotArr[indexMVolume] = - (volumeMask.transform.eulerAngles * Mathf.Deg2Rad);
            rangeArr[indexMVolume] = volumeMask.boxSize.x;
            hardnessArr[indexMVolume] = volumeMask.hardness;
            ColorArr[indexMVolume] = volumeMask.color;


            if (volumeMask.IsSphere)
            {
                isSphereArr[indexMVolume] = 1;
            }
            else
            {
                isSphereArr[indexMVolume] = 0;
            }
            boxRoundArr[indexMVolume] = volumeMask.boxRound;
            boxSoftBorderArr[indexMVolume] = volumeMask.boxSoftBorder;
            boxSizeArr[indexMVolume] = volumeMask.boxSize;
        }
    }
