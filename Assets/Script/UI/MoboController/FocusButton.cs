using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusButton : MonoBehaviour {

	public int btnIndex;
	public MoboController I_MoboController;

	[HideInInspector]
	public Transform focusTarget;
	[HideInInspector]
	public bool isFocus = false;
	[HideInInspector]
	public WeaponNameType targetWeaponName = WeaponNameType.unknown;

	Image imgTarget;
	Image imgFocus;

	void Awake()
	{
		Transform imgTargetTransform = transform.Find("ImgTarget");
		if (imgTargetTransform) {
			imgTarget = imgTargetTransform.GetComponent<Image>();
		}
		Transform imgFocusTransform = transform.Find("ImgFocus");
		if (imgFocusTransform) {
			imgFocus = imgFocusTransform.GetComponent<Image>();
		}
		
	}

	public void SetEnable(bool isActive, Transform target)
	{
		transform.gameObject.SetActive(isActive);
		focusTarget = target;
	}

	public void SetFocus(bool isFocus)
	{
		this.isFocus = isFocus;
		imgFocus.gameObject.SetActive(isFocus);
	}

	public void OnClick()
	{
		I_MoboController.OnFocusBtnClick(btnIndex);
		SetFocus(true);
	}
}
