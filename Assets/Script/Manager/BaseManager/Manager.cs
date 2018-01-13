using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
	melee, singleLoader, autoDistant
}

public class Manager : MonoBehaviour {
	[HideInInspector]
	public Transform self;
	[HideInInspector]
	public Transform body;
	[HideInInspector]
	public Transform weapon;
	[HideInInspector]
	public WeaponManager I_WeaponManager;
	[HideInInspector]
	public Messenger I_Messenger;
	[HideInInspector]
	public DataManager I_DataManager;
	[HideInInspector]
	public Controller I_Controller;
	[HideInInspector]
	public BodyAnimEvents I_BodyAnimEvents;

	private List<string> ownerTags = new List<string> { "Player", "Enemy" };

	protected void Awake()
	{
		self = Utils.GetOwner(transform, ownerTags);
		body = self.Find("Body");
		foreach (Transform child in self) {
			if (child.tag == "MeleeWeapon" || child.tag == "RangeWeapon") {
				weapon = child;
				break;
			}
		}
		if (weapon) {
			I_WeaponManager = weapon.GetComponent<WeaponManager>();
		}
		if (body) {
			I_BodyAnimEvents = body.GetComponent<BodyAnimEvents>();
		}
		I_Messenger = self.GetComponent<Messenger>();
		I_DataManager = self.GetComponent<DataManager>();
		I_Controller = self.GetComponent<Controller>();
	}

	protected void Start()
	{

	}
	

	void Update () {
		
	}

	void OnEnable()
	{
		// 受伤事件:WeaponManager通知Self
		if (I_WeaponManager != null) {
			I_WeaponManager.HurtDeclarationEvent += new WeaponManager.HurtDeclarationEventHandler(HurtDeclarationEventFunc);
		}
	}

	void OnDisable()
	{
		if (I_WeaponManager != null) {
			I_WeaponManager.HurtDeclarationEvent -= HurtDeclarationEventFunc;
		}
	}

	/*----------------------- Utils -----------------------*/
	public bool IsDead()
	{
		return I_DataManager.isDead;
	}

	public int GetKilledWeapon()
	{
		return I_DataManager.killedWeaponName;
	}

	public void SetKilledWeapon(int weaponName)
	{
		I_DataManager.killedWeaponName = weaponName;
	}

	public void SetPlayerDead(bool isDead)
	{
		I_DataManager.isDead = isDead;
	}

	public int GetWeaponName()
	{
		return I_WeaponManager.weaponName;
	}

	public int GetWeaponType()
	{
		return I_WeaponManager.weaponType;
	}
	
	
	/*----------------------- Utils -----------------------*/

	/*--------------------- HurtEvent ---------------------*/
		/*------------ Manager -> Observer ------------*/
	void HurtDeclarationEventFunc(Transform attacker, List<Transform> suffers)
	{
		// 通知Observe受伤事件
		if (I_Messenger != null) {
			I_Messenger.HurtDeclaration(attacker, suffers);
		}
	}

		/*------------ Observer -> Manager ------------*/
	public void HurtNotifyEventDeal(Transform attacker, Transform suffer, float health)
	{
		// 受伤相关处理
		if (self == suffer) {
			if (I_DataManager) {
				I_DataManager.SetHealth(health);
			}
		}
	}
	/*--------------------- HurtEvent ---------------------*/
}
