using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class solidCameraEffect : cameraEffect
{
	public Shader mainShader;

	public Color regularColor = Color.white;

	public bool useEffectColor;
	public Color effectColor = Color.white;

	public Texture2D mainTexture;

	public string blockCountName = "BlockCount";
	public string blockSizeName = "BlockSize";
	public string tintColorName = "_TintColor";
	public string sprTexName = "_SprTex";
	public string colorName = "_Color";

	Material mainMaterial;

	int blockCountID = -1;
	int blockSizeID = -1;
	int tintColorID = -1;
	int sprTexID = -1;
	int colorID = -1;

	public override void renderEffect (RenderTexture source, RenderTexture destination, Camera mainCamera)
	{
		float w = mainCamera.pixelWidth;
		float h = mainCamera.pixelHeight;
		Vector2 count = new Vector2 (w / mainTexture.height, h / mainTexture.height);
		Vector2 size = new Vector2 (1.0f / count.x, 1.0f / count.y);

		setMaterial ();

		if (blockCountID == -1) {
			blockCountID = Shader.PropertyToID (blockCountName);
		}

		if (blockSizeID == -1) {
			blockSizeID = Shader.PropertyToID (blockSizeName);
		}

		if (tintColorID == -1) {
			tintColorID = Shader.PropertyToID (tintColorName);
		}

		if (sprTexID == -1) {
			sprTexID = Shader.PropertyToID (sprTexName);
		}

		if (colorID == -1) {
			colorID = Shader.PropertyToID (colorName);
		}

		mainMaterial.SetVector (blockCountID, count);
		mainMaterial.SetVector (blockSizeID, size);
		mainMaterial.SetColor (colorID, regularColor);
		mainMaterial.SetTexture (sprTexID, mainTexture);

		if (useEffectColor) {
			mainMaterial.SetColor (tintColorID, effectColor);
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
