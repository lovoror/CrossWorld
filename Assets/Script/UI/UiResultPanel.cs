using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiResultPanel : MonoBehaviour {
	public Text txtMaxScore;
	public Text txtScore;

	void OnEnable()
	{
		int max = 0;
		if (PlayerPrefs.HasKey("MaxScore" + GlobalData.difficulty)) {
			max = PlayerPrefs.GetInt("MaxScore" + GlobalData.difficulty);
		}
		txtMaxScore.text = "Max:" + max;
		txtScore.text = "Score:" + GlobalData.Instance.curScore;
	}
}
