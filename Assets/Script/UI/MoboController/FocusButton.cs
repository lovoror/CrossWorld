using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusButton : MonoBehaviour {

	public int btnIndex;

	[HideInInspector]
	public Transform focusTarget;
	[HideInInspector]
	public bool isFocus
	{
		get
		{
			return I_PlayerData.IsBtnFocused(btnIndex);
		}
	}
	[HideInInspector]
	public WeaponNameType targetWeaponName = WeaponNameType.unknown;

	FocusButtons I_FocusButtons;
	PlayerData I_PlayerData;
	MoboController I_MoboController;
	//Image imgTarget;
	Image imgFocus;

	void Awake()
	{
		Transform imgTargetTransform = transform.Find("ImgTarget");
		if (imgTargetTransform) {
			//imgTarget = imgTargetTransform.GetComponent<Image>();
		}
		Transform imgFocusTransform = transform.Find("ImgFocus");
		if (imgFocusTransform) {
			imgFocus = imgFocusTransform.GetComponent<Image>();
		}
		I_MoboController = transform.parent.GetComponentInParent<MoboController>();
		I_PlayerData = PlayerData.Instance;
		I_FocusButtons = transform.GetComponentInParent<FocusButtons>();
	}

	public void SetEnable(bool isActive, Transform target)
	{
		transform.gameObject.SetActive(isActive);
		focusTarget = target;
	}

	public void OnClick()
	{
		I_PlayerData.SetBtnFocus(!isFocus, btnIndex);
		imgFocus.gameObject.SetActive(isFocus);
		I_MoboController.OnFocusBtnClick(btnIndex);
		if (isFocus) {
			I_FocusButtons.ResetOtherBtns(btnIndex);
		}
	}

	public void ResetBtn()
	{
		imgFocus.gameObject.SetActive(isFocus);
	}
}
