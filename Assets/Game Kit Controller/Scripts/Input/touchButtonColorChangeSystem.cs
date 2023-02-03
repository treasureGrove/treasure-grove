using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class touchButtonColorChangeSystem : MonoBehaviour
{
	public RawImage mainImage;

	public Color regularColor;
	public Color pressedColor;

	public void setRegularColor ()
	{
		mainImage.color = regularColor;
	}

	public void setPressedColor ()
	{
		mainImage.color = pressedColor;
	}
}
