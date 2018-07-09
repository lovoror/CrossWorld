using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxScoreTxt : MonoBehaviour {

	Text text;
	int maxScore = 0;
	int curScore
	{
		get
		{
			return GlobalData.Instance.curScore;
		}
	}

	void Awake()
	{
		text = GetComponent<Text>();
		if (PlayerPrefs.HasKey("MaxScore" + GlobalData.difficulty)) {
			maxScore = PlayerPrefs.GetInt("MaxScore" + GlobalData.difficulty);
		}
	}

	void OnEnable()
	{
		UiScoreTxt.MaxScoreEvent += new UiScoreTxt.MaxScoreEventHandler(MaxScoreEventFunc);
	}

	void OnDisable()
	{
		UiScoreTxt.MaxScoreEvent -= MaxScoreEventFunc;
	}

	void Start()
	{
		text.text = "Max:" + maxScore;
	}

	void MaxScoreEventFunc()
	{
		if (curScore > maxScore) {
			maxScore = curScore;
			text.text = "Max:" + curScore;
			PlayerPrefs.SetInt("MaxScore" + GlobalData.difficulty, curScore);
		}
	}

}
