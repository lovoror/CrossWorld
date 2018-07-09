using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDifficultyPanel : MonoBehaviour {
	public Transform btn_easy;
	public Transform btn_normal;
	public Transform btn_hard;

	Difficulty selectedDifficulty = Difficulty.unknow;
	Dictionary<Difficulty, Transform> d_Image;

	void Awake()
	{
		d_Image = new Dictionary<Difficulty, Transform>();
		d_Image.Add(Difficulty.Easy, btn_easy.Find("Image"));
		d_Image.Add(Difficulty.Normal, btn_normal.Find("Image"));
		d_Image.Add(Difficulty.Hard, btn_hard.Find("Image"));
		selectedDifficulty = GlobalData.difficulty;
	}

	void Start ()
	{

	}

	void OnEnable()
	{
		selectedDifficulty = GlobalData.difficulty;
		SelectDifficulty(selectedDifficulty);
	}

	public void OnOkBtnClick()
	{
		GlobalData.difficulty = selectedDifficulty;
		Restart();
	}

	public void OnCloseBtnClick()
	{
		selectedDifficulty = GlobalData.difficulty;
		transform.gameObject.SetActive(false);
	}

	public void OnDifficultyBtnClick(string diff)
	{
		Difficulty difficulty = GetDifficultyByString(diff);
		if (difficulty == Difficulty.unknow) return;
		SelectDifficulty(difficulty);
	}

	void SelectDifficulty(Difficulty difficulty)
	{
		HideRects();
		// 显示选框
		d_Image[difficulty].GetComponent<Image>().enabled = true;
		selectedDifficulty = difficulty;
	}


	// 隐藏选框
	void HideRects()
	{
		foreach (Transform image in d_Image.Values) {
			image.GetComponent<Image>().enabled = false;
		}
	}

	Difficulty GetDifficultyByString(string diff)
	{
		Difficulty result = Difficulty.unknow;
		if (diff == "Easy") {
			result = Difficulty.Easy;
		}
		else if (diff == "Normal") {
			result = Difficulty.Normal;
		}
		else if (diff == "Hard") {
			result = Difficulty.Hard;
		}
		return result;
	}

	void Restart()
	{
		GameStageMachine.Instance.ChangeStage(GameStage.LOGIN);
	}
}
