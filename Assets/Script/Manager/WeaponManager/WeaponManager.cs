using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
	[HideInInspector]
	public Transform self;   // 此近战武器的拥有者
	[HideInInspector]
	public int weaponName;

	protected Manager I_Manager;  // attacker的Manager管理类
	protected BodyAnimEvents I_BodyAnimEvents;
	protected AudioSource attackAudioSource;  // 攻击音效
	protected Transform body;

	protected void Awake()
	{
		self = Utils.GetOwner(transform, Constant.TAGS.Attacker);
		body = self.Find("Body");
		if (self) {
			I_Manager = self.GetComponent<Manager>();
		}
		I_BodyAnimEvents = I_Manager.I_BodyAnimEvents;
	}

	protected void Start () {
		attackAudioSource = transform.GetComponent<AudioSource>();
	}

	protected void OnEnable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent += new Messenger.DeadNotifyEventHandler(DeadNotifyEventFunc);
		I_BodyAnimEvents.AttackEvent += new BodyAnimEvents.AttackEventHandler(AttackEventFunc);
	}

	protected void OnDisable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent -= DeadNotifyEventFunc;
		I_BodyAnimEvents.AttackEvent -= AttackEventFunc;
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger) return;
		Transform suffer = Utils.GetOwner(other.transform, Constant.TAGS.Attacker);
		// 发出敌人进入攻击范围事件
		if (suffer && suffer.tag != self.tag) {
			if (EnemyInATKRangeEvent != null) {
				EnemyInATKRangeEvent(suffer, true);
			}
		}
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		if (other.isTrigger) return;
		Transform suffer = Utils.GetOwner(other.transform, Constant.TAGS.Attacker);
		// 发出敌人进入攻击范围事件
		if (suffer && suffer.tag != self.tag) {
			if (EnemyInATKRangeEvent != null) {
				EnemyInATKRangeEvent(suffer, false);
			}
		}
	}

	protected virtual void AttackEventFunc()
	{
		PlayAttackShound();
	}

	protected virtual void PlayAttackShound()
	{
		attackAudioSource.Play();
	}

	/*----------------- EnemyInATKRangeEvent ------------------*/
	public delegate void EnemyInATKRangeEventHandler(Transform enemy, bool isInRange);
	public event EnemyInATKRangeEventHandler EnemyInATKRangeEvent;

	/*----------------- EnemyInATKRangeEvent ------------------*/

	/*--------------------- HurtEvent ---------------------*/
		/*--------- Weapon -> PlayerManager ---------*/
	// 受伤事件:WeaponManager通知attacker的Manager
	public delegate void HurtDeclarationEventHandler(Transform attacker, List<Transform> suffers);
	public event HurtDeclarationEventHandler HurtDeclarationEvent;
	
	// 通知PlayerManager伤害了某（些）人
	protected void BasicHurt(Transform attacker, List<Transform> suffers)
	{
		if (HurtDeclarationEvent != null) {
			HurtDeclarationEvent(attacker, suffers);
		}
	}

	protected void BasicHurt(Transform attacker, Transform suffer)
	{
		List<Transform> suffers = new List<Transform> { suffer };
		BasicHurt(attacker, suffers);
	}
	/*--------------------- HurtEvent ---------------------*/

	/*---------------- KillerNotifyEvent ------------------*/
	public virtual void DeadNotifyEventFunc(Transform killer, Transform dead)
	{

	}
	/*---------------- KillerNotifyEvent ------------------*/
}
