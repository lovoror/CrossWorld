using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuncRButton {
	public WeaponNameType weaponName;

	public delegate void PlayerChangeWeaponEventHandler(Transform player, WeaponNameType weaponName);
	public static event PlayerChangeWeaponEventHandler PlayerChangeWeaponEvent;

	public delegate void PlayerReloadEventHandler(Transform player);
	public static event PlayerReloadEventHandler PlayerReloadEvent;

	public FuncRButton(WeaponNameType weaponName)
	{
		this.weaponName = weaponName;
	}

	public void OnClick()
	{
		WeaponNameType curWeaponName = Observer.GetPlayerCurWeaponName();
		Transform player = Observer.GetPlayer();
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
