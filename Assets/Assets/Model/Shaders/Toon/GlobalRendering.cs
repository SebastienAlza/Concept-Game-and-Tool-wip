using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlobalRendering : MonoBehaviour
{
	public Color ambientColor;

	[Range(0f, 1f)]
	public float sobelThickness;
	[Range(0f, 1f)]
	public float sobelStrength;
	[Range(0f, 1f)]
	public float sobelPower;

	public GameObject MainDirLightGO;
	public GameObject RimLightGO;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//Main Light
		if (MainDirLightGO && MainDirLightGO.GetComponent<Light>() != null)
		{
			Light MainLightComp = MainDirLightGO.GetComponent<Light>();
			Vector3 MainDirLightDirection = -MainLightComp.transform.forward;
			float _shadowStrength = MainLightComp.shadowStrength;
			Color _lightColor = MainLightComp.color;
			Shader.SetGlobalVector("_MainDirLightDir", MainDirLightDirection);
			Shader.SetGlobalFloat("_shadowIntensity", _shadowStrength);
			Shader.SetGlobalColor("_lightColor", _lightColor);
			Shader.SetGlobalFloat("_lightIntensity", MainLightComp.intensity);
		}
		else
		{
			Shader.SetGlobalColor("_lightColor", Color.red);
			Shader.SetGlobalFloat("_shadowIntensity", 0);
			Debug.Log("No Main Light Attached");
		}

		if (RimLightGO != null)
		{
			Light RimLightComp = RimLightGO.GetComponent<Light>();
			Vector3 RimLightDirection = -RimLightComp.transform.forward;
			Shader.SetGlobalVector("_RimDirLightDir", RimLightDirection);
			Color RimColor = RimLightComp.color;
			Shader.SetGlobalColor("_RimColor", RimColor);
		}

		Shader.SetGlobalFloat("_Sobel_Thickness", sobelThickness);
		Shader.SetGlobalFloat("_Sobel_Strength", sobelStrength);
		Shader.SetGlobalFloat("_Sobel_Power", sobelPower);
		///Ambiant Color
		Shader.SetGlobalColor("_AmbientColor", ambientColor);
	}
}
