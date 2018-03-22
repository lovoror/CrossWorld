using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletNumberUpdate : MonoBehaviour {
	public WeaponNameType weaponName;
	PlayerData I_PlayerData;
	Text bulletNumberText;
	int magazineSize = 0;
	void Awake()
	{
		bulletNumberText = GetComponent<Text>();
		I_PlayerData = PlayerData.Instance;
	}

	void Start()
	{
		if (Constant.MagazineSize.ContainsKey(weaponName)) {
			magazineSize = Constant.MagazineSize[weaponName];
		}
		int bullet = I_PlayerData.GetLeftBulletsByName(weaponName);
		bulletNumberText.text = bullet + "/" + magazineSize;
	}
	
	void Update ()
	{
		if (I_PlayerData.curWeaponName == weaponName) {
			int bullet = I_PlayerData.GetLeftBulletsByName(weaponName);
			bulletNumberText.text = bullet + "/" + magazineSize;
		}
	}
}
