using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyTxt : MonoBehaviour
{
	public Color c_easy;
	public Color c_normal;
	public Color c_hard;

	Text text;

	void Awake()
	{
		text = GetComponent<Text>();
	}

	void Start ()
	{
		SetText();
	}

	void OnEnable()
	{
		SetText();
	}

	void SetText()
	{
		text.text = GlobalData.difficulty.ToString();
		Color color;
		switch (GlobalData.difficulty) {
			case Difficulty.Hard:
				color = c_hard;
				break;
			case Difficulty.Normal:
				color = c_normal;
				break;
			case Difficulty.Easy:
				color = c_easy;
				break;
			default:
				color = c_normal;
				break;
		}
		text.color = color;
	}
}
