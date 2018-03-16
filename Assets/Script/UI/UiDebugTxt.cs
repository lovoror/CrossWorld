using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDebugTxt : MonoBehaviour {
	static Text text;
	static string context = "";
	void Awake()
	{
		text = GetComponent<Text>();
		text.text = context;
	}

	public static void SetContext(string info)
	{
		context = info;
		text.text = info;
	}
}
