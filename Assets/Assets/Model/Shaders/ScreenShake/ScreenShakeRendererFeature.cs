using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenShakeRendererFeature : ScriptableRendererFeature
{
	class ScreenShakeRenderPass : ScriptableRenderPass
	{
		Material material;
		ScreenShakeSettings screenShakeSettings;
		ScriptableRenderer renderer;
		RenderTargetIdentifier source;
		RTHandle screenShakeTex;

		int screenShakeTexID;

		public bool Setup(ScriptableRenderer renderer)
		{
			//source = renderer.cameraColorTargetHandle;
			screenShakeSettings = VolumeManager.instance.stack.GetComponent<ScreenShakeSettings>();
			this.renderer = renderer;
			if (screenShakeSettings != null && screenShakeSettings.IsActive())
			{
				material = new Material(Shader.Find("ScreenShake"));
				return true;
			}

			return false;
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			if (screenShakeSettings == null || !screenShakeSettings.IsActive())
			{
				return;
			}

			screenShakeTex = RTHandles.Alloc("_ScreenShakeTex", name: "_ScreenShakeTex");

			// screenShakeTexID = Shader.PropertyToID("_ScreenShakeTex");
			// screenShakeTex = new RTHandle();
			// screenShakeTex.id = screenShakeTexID;

			cmd.GetTemporaryRT(Shader.PropertyToID(screenShakeTex.name), cameraTextureDescriptor);
			//cmd.GetTemporaryRT(screenShakeTex.id, cameraTextureDescriptor);

			ConfigureInput(ScriptableRenderPassInput.Depth);

			base.Configure(cmd, cameraTextureDescriptor);
		}

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			base.OnCameraSetup(cmd, ref renderingData);
			source = renderer.cameraColorTargetHandle;
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (screenShakeSettings == null || !screenShakeSettings.IsActive())
			{
				return;
			}

			CommandBuffer cmd = CommandBufferPool.Get("ScreenShake");

			material.SetFloat("_Intensity", screenShakeSettings.intensity.value);

			cmd.Blit(source, screenShakeTex.nameID, material, 0);
			cmd.Blit(screenShakeTex.nameID, source);

			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
			CommandBufferPool.Release(cmd);
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{
			cmd.ReleaseTemporaryRT(Shader.PropertyToID(screenShakeTex.name));
			//cmd.ReleaseTemporaryRT(screenShakeTexID);
			base.FrameCleanup(cmd);
		}
	}

	ScreenShakeRenderPass screenShakeRenderPass;
	public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

	public override void Create()
	{
		screenShakeRenderPass = new ScreenShakeRenderPass();
		screenShakeRenderPass.renderPassEvent = renderPassEvent;
		name = "ScreenShake";
	}


	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (screenShakeRenderPass.Setup(renderer))
		{
			renderer.EnqueuePass(screenShakeRenderPass);
		}
	}
}