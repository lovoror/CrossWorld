using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
	[HideInInspector]
	public Transform self { get; set; }
	[HideInInspector]
	public WeaponManager I_WeaponManager
	{
		get
		{
			return I_DataManager.curWeaponTransform.GetComponent<WeaponManager>();
		}
	}
	[HideInInspector]
	public Messenger I_Messenger;
	[HideInInspector]
	public DataManager I_DataManager;
	[HideInInspector]
	public Controller I_Controller;
	[HideInInspector]
	public AnimEventsManager I_AnimEventsManager { get; set; }
	[HideInInspector]
	public Animator I_Animator { get; set; }
	[HideInInspector]
	public bool isPlayer;
	[HideInInspector]
	public BaseData I_BaseData { get; set; }

	private List<string> ownerTags = new List<string> { "Player", "Enemy" };

	protected void Awake()
	{
		self = Utils.GetOwner(transform, ownerTags);
		I_AnimEventsManager = transform.GetComponentInChildren<AnimEventsManager>();
		I_Messenger = self.GetComponent<Messenger>();
		I_DataManager = self.GetComponent<DataManager>();
		I_Controller = self.GetComponent<Controller>();
		I_Animator = self.GetComponentInChildren<Animator>();
	}

	protected void Start()
	{
		I_BaseData = Utils.GetBaseData(self);
	}
	

	void Update () {
		
	}

	void OnEnable()
	{

	}

	void OnDisable()
	{

	}

	/*--------------------- HurtEvent ---------------------*/
	public void HurtDeclaration(Transform attacker, List<Transform> suffers)
	{
		// 通知Observe受伤事件
		if (I_Messenger != null) {
			I_Messenger.HurtDeclaration(attacker, suffers);
		}
	}

	public virtual void HurtNotifyEventDeal(Transform attacker, Transform suffer)
	{

	}
	/*--------------------- HurtEvent ---------------------*/


	/*----------------------- Utils -----------------------*/
	public bool IsDead()
	{
		return I_DataManager.isDead;
	}

	public WeaponNameType GetKilledWeapon()
	{
		return I_DataManager.killedWeaponName;
	}

	public void SetKilledWeapon(WeaponNameType weaponName)
	{
		I_DataManager.killedWeaponName = weaponName;
	}

	public WeaponNameType GetCurWeaponName()
	{
		return I_DataManager.curWeaponName;
	}

	public WeaponType GetWeaponType()
	{
		return I_WeaponManager.weaponType;
	}
	/*----------------------- Utils -----------------------*/
}
