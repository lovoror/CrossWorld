using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
	[HideInInspector]
	public Transform owner;
	[HideInInspector]
	public Transform body;
	[HideInInspector]
	public Transform weapon;

	public WeaponManager I_WeaponManager;
	public Messenger I_Messenger;
	public DataManager I_DataManager;
	public Controller I_Controller;


	private List<string> ownerTags = new List<string> { "Player", "Enemy" };

	protected void Awake()
	{
		owner = Utils.GetOwner(transform, ownerTags);
		body = owner.Find("Body");
		foreach (Transform child in body) {
			if (child.tag == "MeleeWeapon" || child.tag == "RangeWeapon") {
				weapon = child;
				break;
			}
		}
		if (weapon) {
			I_WeaponManager = weapon.GetComponent<WeaponManager>();
		}
		I_Messenger = owner.GetComponent<Messenger>();
		I_DataManager = owner.GetComponent<DataManager>();
		I_Controller = owner.GetComponent<Controller>();
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
		if (owner == suffer) {
			if (I_DataManager) {
				I_DataManager.SetHealth(health);
			}
		}
	}
	/*--------------------- HurtEvent ---------------------*/

	/*--------------------- DeadEvent ---------------------*/
	public void DeadNotifyEventDeal(Transform target, bool isDead)
	{
		I_Controller.DeadEventFunc(target);
	}
	/*--------------------- DeadEvent ---------------------*/


}
