using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData {
	protected static GlobalData m_instance = null;
	public static GlobalData Instance
	{
		get
		{
			if (m_instance == null)
				m_instance = new GlobalData();
			return m_instance;
		}
	}

	public static Difficulty difficulty = Difficulty.Hard;
	public static float diffRate { get; set; }
	public int curScore { get; set; }
	int killedEnemy;

	GlobalData()
	{
		
	}

	public void Init()
	{
		curScore = 0;
		killedEnemy = 0;
		difficulty = GetDifficulty();
		if (difficulty != Difficulty.unknow && Constant.DifficultyRate.ContainsKey(difficulty)) {
			diffRate = Constant.DifficultyRate[difficulty];
		}
		else {
			diffRate = 1;
		}
	}

	Difficulty GetDifficulty()
	{
		Difficulty difficulty = Difficulty.Hard;
		if (PlayerPrefs.HasKey("Difficulty")) {
			int i_dif = PlayerPrefs.GetInt("Difficulty");
			difficulty = (Difficulty)i_dif;
		}
		return difficulty;
	}

	public static void SetDifficulty(Difficulty dift)
	{
		difficulty = dift;
		PlayerPrefs.SetInt("Difficulty", (int)difficulty);
	}

	public void AddKilledEnemy(int num = 0)
	{
		killedEnemy += num;
	}

	public int GetKilledNum()
	{
		return killedEnemy;
	}

	public void AddScore(int delta)
	{
		curScore += delta;
	}

	public void StageEnd()
	{
		m_instance = null;
	}
}
