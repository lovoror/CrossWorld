using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AttackType
{
	untouched, unknown, click, hold, clickPointer, holdPointer
}

public class MoboController : MonoBehaviour
{
	public GameObject BtnFocus;
	GameObject FocusBtns;
	AttackButton ButtonA;
	LeftStick StickL;
	Dictionary<WeaponNameType, FuncRButton> FuncRButtons = new Dictionary<WeaponNameType, FuncRButton>();
	PlayerData I_PlayerData;
	
	public delegate void PlayerMoveEventHandler(Vector2 dir);
	public static event PlayerMoveEventHandler PlayerMoveEvent;

	public delegate void PlayerFaceEventHandler(Vector2 direction);
	public static event PlayerFaceEventHandler PlayerFaceEvent;

	public delegate void AttackDownEventHandler(Vector2 position);
	public static event AttackDownEventHandler AttackDownEvent;

	public delegate void AttackUpEventHandler(float deltaTime);
	public static event AttackUpEventHandler AttackUpEvent;

	public delegate void FocusTargetChangeHandler();
	public static event FocusTargetChangeHandler FocusTargetChangeEvent;

	void Awake()
	{
		ButtonA = GetComponentInChildren<AttackButton>();
		StickL = GetComponentInChildren<LeftStick>();
		I_PlayerData = PlayerData.Instance;
		FocusBtns = GameObject.Find("FocusBtns");
	}

	void OnEnable()
	{
		// UIAttackUpEvent
		AttackButton.UIAttackUpEvent += new AttackButton.UIAttackUpEventHandler(UIAttackUpEventFunc);
		// UIAttackDownEvent
		AttackButton.UIAttackDownEvent += new AttackButton.UIAttackDownEventHandler(UIAttackDownEventFunc);
		// FocusTargetsChangeEvent
		PlayerMessenger.FocusTargetsChangeEvent += new PlayerMessenger.FocusTargetsChangeEventHandler(FocusTargetsChangeEventFunc);
	}

	void OnDisable()
	{
		AttackButton.UIAttackUpEvent -= UIAttackUpEventFunc;
		AttackButton.UIAttackDownEvent -= UIAttackDownEventFunc;
		PlayerMessenger.FocusTargetsChangeEvent -= FocusTargetsChangeEventFunc;
	}

	Vector2 last_bodyDirection = Vector2.zero;
	Vector2 last_moveDirection = Vector2.zero;
	void Update()
	{
		// 人物朝向
		Vector2 bodyDirection = ButtonA.direction;
		if (bodyDirection != Vector2.zero && bodyDirection != last_bodyDirection) {
			last_bodyDirection = bodyDirection;
			// 改变人物朝向
			if (PlayerFaceEvent != null) {
				PlayerFaceEvent(bodyDirection);
			}
		}

		// 移动方向
		Vector2 moveDirection = StickL.direction;
		if (moveDirection != last_moveDirection) {
			last_moveDirection = moveDirection;
			// 改变移动方向
			//if (controlType == ControlType.modern) {
				if (PlayerMoveEvent != null) {
					PlayerMoveEvent(moveDirection);
				}
			//}
			//else if (controlType == ControlType.ancient) {

			//}
		}
	}

	/*--------------------- UIAttackUpEvent ---------------------*/
	void UIAttackUpEventFunc(float deltaTime)
	{
		if (AttackUpEvent != null) {
			AttackUpEvent(deltaTime);
		}
	}
	/*--------------------- UIAttackUpEvent ---------------------*/

	/*--------------------- UIAttackUpEvent ---------------------*/
	void UIAttackDownEventFunc(Vector2 position)
	{
		if (AttackDownEvent != null) {
			AttackDownEvent(position);
		}
	}
	/*--------------------- UIAttackUpEvent ---------------------*/

	/*------------------ FocusTargetChangedEvent -------------------*/
	public void OnFocusBtnClick(int btnIndex)
	{
		if (FocusTargetChangeEvent != null) {
			FocusTargetChangeEvent();
		}
	}
	/*------------------ FocusTargetChangedEvent -------------------*/

	/*------------------ FocusTargetsChangeEvent -------------------*/
	void FocusTargetsChangeEventFunc()
	{
		// 删除先前的FocusBtns
		foreach (Transform child in FocusBtns.transform) {
			Destroy(child.gameObject);
		}
		// 新建FocusBtns
		FocusTargets I_FocusTargets = I_PlayerData.I_FocusTargets;
		float topY = 360;
		float X = -7;
		float deltaY = 124;
		int count = I_FocusTargets.focusTargets.Count;
		if (count > 3) {
			topY += deltaY * (count - 3);
		}
		for (int i = 0; i < count; ++i) {
			// 新建Button
			FocusTarget I_FocusTarget = I_FocusTargets.focusTargets[i];
			GameObject btn = Instantiate(BtnFocus, FocusBtns.transform);
			((RectTransform)btn.transform).anchoredPosition = new Vector2(X, topY - deltaY * i);
			// 是否选中
			if (I_FocusTarget.isFocus) {
				Transform imgFocus = btn.transform.Find("ImgFocus");
				imgFocus.gameObject.SetActive(true);
			}
			// 初始化赋值
			FocusButton I_FocusButton = btn.transform.GetComponent<FocusButton>();
			I_FocusButton.btnIndex = i;
			I_FocusButton.focusTarget = I_FocusTarget.focusTarget;
		}
	}
	/*------------------ FocusTargetsChangeEvent -------------------*/

	public void OnClick_Func_R(string str_WeaponName)
	{
		if (Observer.IsPlayerDead()) return;
		WeaponNameType weaponName = (WeaponNameType)System.Enum.Parse(typeof(WeaponNameType), str_WeaponName, true);
		FuncRButton func;
		if (FuncRButtons.ContainsKey(weaponName)) {
			func = FuncRButtons[weaponName];
		}
		else {
			func = new FuncRButton(weaponName);
			FuncRButtons[weaponName] = func;
		}
		func.OnClick();
	}

	public void Restart()
	{
		GameStageMachine.Instance.ChangeStage(GameStage.LOGIN);
	}
}
