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

	public int curScore;

	GlobalData()
	{
		
	}

	public void Init()
	{
		curScore = 0;
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
