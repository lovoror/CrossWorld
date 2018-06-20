using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiScoreTxt : MonoBehaviour {

	Text text;
	int score
	{
		get
		{
			return GlobalData.Instance.curScore;
		}
	}

	public delegate void MaxScoreEventHandler();
	public static event MaxScoreEventHandler MaxScoreEvent;

	void Awake()
	{
		text = GetComponent<Text>();
	}

	void OnEnable()
	{
		AttackOB.AddScoreEvent += new AttackOB.AddScoreEventHandler(AddScoreEventFunc);
	}

	void OnDisable()
	{
		AttackOB.AddScoreEvent -= AddScoreEventFunc;
	}

	void Start ()
	{
		text.text = "Score:0";
	}
	
	void Update ()
	{
		
	}

	void AddScoreEventFunc()
	{
		text.text = "Score:" + this.score;
		if (MaxScoreEvent != null) {
			MaxScoreEvent();
		}
	}
}
