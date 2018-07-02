using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class ImgDictionary
{
	public WeaponNameType weaponName;
	public Sprite image;
}

public class FuncRButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public delegate void PlayerChangeWeaponEventHandler(Transform player, WeaponNameType weaponName);
	public static event PlayerChangeWeaponEventHandler PlayerChangeWeaponEvent;

	public delegate void PlayerReloadEventHandler(Transform player);
	public static event PlayerReloadEventHandler PlayerReloadEvent;

	public List<WeaponNameType> weapons;
	public List<ImgDictionary> weaponImages;
	public Image I_Image;
	public Image I_ImageU;
	public Image I_ImageD;
	public Image I_ImageL;
	public Image I_ImageR;
	public Text I_BulletNumber;
	public WeaponNameType curWeaponNameOnBtn;  // 当前Btn上所展示的武器
	Dictionary<string, WeaponNameType> d_Weapons;  // 上下左右操作对应的武器
	Dictionary<WeaponNameType, Sprite> d_WeaponImages;  // 各种武器所对应的Image
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
	PlayerData I_PlayerData;


	void Awake()
	{
		I_PlayerData = PlayerData.Instance;
	}

	void Start()
	{
		d_Weapons = new Dictionary<string, WeaponNameType>();
		d_WeaponImages = new Dictionary<WeaponNameType, Sprite>();
		foreach (var a in weaponImages) {
			if (!d_WeaponImages.ContainsKey(a.weaponName)) {
				d_WeaponImages.Add(a.weaponName, a.image);
			}
		}
		for (int i = 0; i < weapons.Count && i < 4; ++i) {
			string key;
			Image image = null;
			switch (i) {
				case 0: key = "up";
					image = I_ImageU;
					break;
				case 1: key = "down";
					image = I_ImageD;
					break;
				case 2: key = "left";
					image = I_ImageL;
					break;
				case 3: key = "right";
					image = I_ImageR;
					break;
				default: key = "null"; break;
			}
			if (key != "null" && !d_Weapons.ContainsKey(key)) {
				d_Weapons.Add(key, weapons[i]);
				// 设置上下左右武器图片
				image.sprite = d_WeaponImages[weapons[i]];
			}
		}

		if (d_Weapons.ContainsValue(curWeaponName)) {
			curWeaponNameOnBtn = curWeaponName;
			SetUi(curWeaponNameOnBtn);
		}
	}

	void Update()
	{
		UpdateBulletNumber();
	}

	void SetUi(WeaponNameType weaponName)
	{
		// 设置图像
		if (d_WeaponImages.ContainsKey(weaponName)) {
			I_Image.sprite = d_WeaponImages[curWeaponNameOnBtn];
		}
		// 设置子弹数是否可见
		WeaponType weaponType = Utils.GetWeaponTypeByName(weaponName);
		I_BulletNumber.gameObject.SetActive(weaponType != WeaponType.melee);
	}

	void UpdateBulletNumber()
	{
		if (curWeaponName == curWeaponNameOnBtn) {
			int magazineSize = 0;
			if (Constant.MagazineSize.ContainsKey(curWeaponNameOnBtn)) {
				magazineSize = Constant.MagazineSize[curWeaponNameOnBtn];
			}
			int bullet = I_PlayerData.GetLeftBulletsByName(curWeaponNameOnBtn);
			I_BulletNumber.text = bullet + "/" + magazineSize;
		}
	}

	public void OnPointerDown(PointerEventData data)
	{

	}
	public void OnDrag(PointerEventData data)
	{

	}

	float min = 20;
	public void OnPointerUp(PointerEventData data)
	{
		bool isDraged = false;
		WeaponNameType nextWeaponName = WeaponNameType.unknown;
		float dx = data.position.x - data.pressPosition.x;
		float dy = data.position.y - data.pressPosition.y;
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
			if (nextWeaponName != WeaponNameType.unknown && nextWeaponName != curWeaponName) {
				ChangeWeapon(nextWeaponName);
			}
		}
		else {
			OnClick();
		}
	}

	void ChangeWeapon(WeaponNameType weaponName)
	{
		if (PlayerChangeWeaponEvent != null) {
			PlayerChangeWeaponEvent(player, weaponName);
		}
		if (weaponName == curWeaponNameOnBtn) return;
		curWeaponNameOnBtn = weaponName;
		SetUi(weaponName);
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
		if (curWeaponNameOnBtn == curWeaponName) {
			WeaponType weaponType = Utils.GetWeaponTypeByName(curWeaponNameOnBtn);
			if (weaponType == WeaponType.autoDistant || weaponType == WeaponType.singleLoader) {
				if (PlayerReloadEvent != null) {
					PlayerReloadEvent(player);
				}
			}
		}
		else {
			if (PlayerChangeWeaponEvent != null) {
				PlayerChangeWeaponEvent(player, curWeaponNameOnBtn);
			}
		}
	}
}
