using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIColorChange : MonoBehaviour {

	public Image[] images;
	public Image clock;

	public void ChangeImages(Color color)
	{
		foreach (Image image in images)
		{
			image.color = color;
		}
	}

	void Update()
	{
		clock.fillAmount = 1f - (TimeManager.global.time % 1f);
	}
}
