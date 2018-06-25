using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FuncRButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public List<WeaponNameType> weapons;
	Dictionary<string, WeaponNameType> d_Weapons;
	WeaponNameType curWeaponName
	{
		get
		{
			return Observer.GetPlayerCurWeaponName();
		}
	}
	Transform player
	{
		get
		{
			return Observer.GetPlayer();
		}
	}

	public WeaponNameType weaponName;

	public delegate void PlayerChangeWeaponEventHandler(Transform player, WeaponNameType weaponName);
	public static event PlayerChangeWeaponEventHandler PlayerChangeWeaponEvent;

	public delegate void PlayerReloadEventHandler(Transform player);
	public static event PlayerReloadEventHandler PlayerReloadEvent;

	void Awake()
	{

	}

	void Start()
	{
		for (int i = 0; i < weapons.Count && i < 4; ++i) {
			string key;
			switch (i) {
				case 0: key = "up"; break;
				case 1: key = "down"; break;
				case 2: key = "left"; break;
				case 3: key = "right"; break;
				default: key = "null"; break;
			}
			if (key != "null" && !d_Weapons.ContainsKey(key)) {
				d_Weapons.Add(key, weapons[i]);
			}
		}
	}

	public FuncRButton(WeaponNameType weaponName)
	{
		this.weaponName = weaponName;
	}

	//public void OnClick_Func_R(string str_WeaponName)
	//{
	//	if (Observer.IsPlayerDead()) return;
	//	WeaponNameType weaponName = (WeaponNameType)System.Enum.Parse(typeof(WeaponNameType), str_WeaponName, true);
	//	FuncRButton func;
	//	if (FuncRButtons.ContainsKey(weaponName)) {
	//		func = FuncRButtons[weaponName];
	//	}
	//	else {
	//		func = new FuncRButton(weaponName);
	//		FuncRButtons[weaponName] = func;
	//	}
	//	func.OnClick();
	//}
	public void OnPointerDown(PointerEventData data)
	{

	}
	public void OnDrag(PointerEventData data)
	{

	}

	float min = 30;
	public void OnPointerUp(PointerEventData data)
	{
		bool isDraged = false;
		WeaponNameType nextWeaponName = WeaponNameType.unknown;
		float dx = data.position.x - data.pressPosition.x;
		float dy = data.position.x - data.pressPosition.x;
		if (Mathf.Abs(dy) > Mathf.Abs(dx)) {
			if (dy < -min) {
				nextWeaponName = d_Weapons["down"];
				isDraged = true;
			}
			else if (dy > min) {
				nextWeaponName = d_Weapons["up"];
				isDraged = true;
			}
		}
		else {
			if (dx < -min) {
				nextWeaponName = d_Weapons["left"];
				isDraged = true;
			}
			else if (dx > min) {
				nextWeaponName = d_Weapons["right"];
				isDraged = true;
			}
		}

		if (isDraged) {
			if (nextWeaponName != curWeaponName) {
				ChangeWeapon(nextWeaponName);
			}
		}
		else {
			ReloadWeapon();
		}
	}

	void ChangeWeapon(WeaponNameType weaponName)
	{
		if (PlayerChangeWeaponEvent != null) {
			PlayerChangeWeaponEvent(player, weaponName);
		}
	}

	void ReloadWeapon()
	{
		if (PlayerReloadEvent != null) {
			PlayerReloadEvent(player);
		}
	}

	public void OnClick()
	{
		WeaponNameType curWeaponName = Observer.GetPlayerCurWeaponName();
		
		if (weaponName == curWeaponName) {
			WeaponType weaponType = Utils.GetWeaponTypeByName(weaponName);
			if (weaponType == WeaponType.autoDistant || weaponType == WeaponType.singleLoader) {
				if (PlayerReloadEvent != null) {
					PlayerReloadEvent(player);
				}
			}
		}
		else {
			if (PlayerChangeWeaponEvent != null) {
				PlayerChangeWeaponEvent(player, weaponName);
			}
		}
	}
}
