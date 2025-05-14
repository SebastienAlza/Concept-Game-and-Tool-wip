 using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("ScreenShake")]
public class ScreenShakeSettings : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter intensity = new ClampedFloatParameter(value: 0, min: 0, max: 1, overrideState: true);

    public bool IsActive()
    {
        return active;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}