using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sliderInfoSystem : MonoBehaviour
{
	public bool sliderInfoSystemEnabled = true;

	public List<sliderInfo> sliderInfoList = new List<sliderInfo> ();

	public void activateSliderByName (string sliderName)
	{
		if (!sliderInfoSystemEnabled) {
			return;
		}

		int sliderIndex = sliderInfoList.FindIndex (s => s.Name.Equals (sliderName));

		if (sliderIndex > -1) {
			sliderInfo currentSliderInfo = sliderInfoList [sliderIndex];

			if (currentSliderInfo.sliderInfoEnable) {

				currentSliderInfo.sliderActive = true;

				if (!currentSliderInfo.sliderGameObject.activeSelf) {
					currentSliderInfo.sliderGameObject.SetActive (true);
				}

				currentSliderInfo.mainSlider.maxValue = currentSliderInfo.sliderMaxValue;

				if (currentSliderInfo.decreaseSliderValue) {
					currentSliderInfo.mainSlider.value = currentSliderInfo.sliderMaxValue;
				} else {
					currentSliderInfo.mainSlider.value = 0;
				}

				currentSliderInfo.sliderCoroutine = StartCoroutine (updateUseSliderCoroutine (currentSliderInfo));
			}
		}
	}

	public void deactivateSliderByName (string sliderName)
	{
		if (!sliderInfoSystemEnabled) {
			return;
		}

		int sliderIndex = sliderInfoList.FindIndex (s => s.Name.Equals (sliderName));

		if (sliderIndex > -1) {
			sliderInfo currentSliderInfo = sliderInfoList [sliderIndex];

			if (currentSliderInfo.sliderInfoEnable) {
				if (currentSliderInfo.sliderGameObject.activeSelf) {
					currentSliderInfo.sliderGameObject.SetActive (false);
				}

				if (currentSliderInfo.sliderCoroutine != null) {
					StopCoroutine (currentSliderInfo.sliderCoroutine);
				}

				currentSliderInfo.sliderActive = false;
			}
		}
	}

	IEnumerator updateUseSliderCoroutine (sliderInfo currentSliderInfo)
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			float sliderValue = 0;

			if (currentSliderInfo.useUnscaledTime) {
				sliderValue = Time.unscaledDeltaTime;
			} else {
				sliderValue = Time.deltaTime;
			}

			float multiplierSign = 1;

			if (currentSliderInfo.decreaseSliderValue) {
				multiplierSign = -1;
			}

			currentSliderInfo.mainSlider.value += sliderValue * currentSliderInfo.useSliderValueRate * multiplierSign;
		}
	}

	[System.Serializable]
	public class sliderInfo
	{
		public string Name;

		public bool sliderInfoEnable = true;

		public bool decreaseSliderValue;

		public float sliderMaxValue;

		public float useSliderValueRate;

		public bool useUnscaledTime;

		public bool sliderActive;

		public Coroutine sliderCoroutine;

		public Slider mainSlider;

		public GameObject sliderGameObject;
	}
}
