using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusButtons : MonoBehaviour {
	public void ResetOtherBtns(int clickBtnIndex)
	{
		foreach (Transform child in transform) {
			FocusButton I_FocusButton = child.GetComponent<FocusButton>();
			if (clickBtnIndex != I_FocusButton.btnIndex) {
				I_FocusButton.ResetBtn();
			}
		}
	}
}
