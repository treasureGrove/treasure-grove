using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pixelCameraEffect : cameraEffect
{
	public Shader mainShader;

	public bool useEffectColor;
	public Color effectColor = Color.white;

	public string blockCountName = "BlockCount";
	public string blockSizeName = "BlockSize";
	public string colorName = "_TintColor";

	[Range (64.0f, 1024.0f)] public float BlockCount = 128;
	Material mainMaterial;

	int blockCountID = -1;
	int blockSizeID = -1;
	int colorID = -1;

	public override void renderEffect (RenderTexture source, RenderTexture destination, Camera mainCamera)
	{
		float k = mainCamera.aspect;
		Vector2 count = new Vector2 (BlockCount, BlockCount / k);
		Vector2 size = new Vector2 (1.0f / count.x, 1.0f / count.y);

		setMaterial ();

		if (blockCountID == -1) {
			blockCountID = Shader.PropertyToID (blockCountName);
		}
			
		if (blockSizeID == -1) {
			blockSizeID = Shader.PropertyToID (blockSizeName);
		}

		if (colorID == -1) {
			colorID = Shader.PropertyToID (colorName);
		}

		mainMaterial.SetVector (blockCountID, count);
		mainMaterial.SetVector (blockSizeID, size);

		if (useEffectColor) {
			mainMaterial.SetColor (colorID, effectColor);
		}

		Graphics.Blit (source, destination, mainMaterial);
	}

	public void setMaterial ()
	{
		if (mainMaterial == null) {
			mainMaterial = new Material (mainShader);
			mainMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}
}
