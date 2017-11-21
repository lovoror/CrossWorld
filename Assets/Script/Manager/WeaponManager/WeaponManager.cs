using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
	[HideInInspector]
	public Transform self;   // 此近战武器的拥有者
	[HideInInspector]
	public int weaponName;
	public float attackNoiseRadius = 0;

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
		I_BodyAnimEvents.PlayAttackSoundEvent += new BodyAnimEvents.PlayAttackSoundEventHandler(PlayAttackShoundEventFunc);
		I_Manager.I_Messenger.DeadNotifyEvent += new Messenger.DeadNotifyEventHandler(DeadNotifyEventFunc);
	}

	protected void OnDisable()
	{
		I_BodyAnimEvents.PlayAttackSoundEvent -= PlayAttackShoundEventFunc;
		I_Manager.I_Messenger.DeadNotifyEvent -= DeadNotifyEventFunc;
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

	/*--------------- PlayAttackSoundEvent ----------------*/
		/*--------- BodyAnimEvents -> Weapon ----------*/
	protected virtual void PlayAttackShoundEventFunc()
	{
		attackAudioSource.Play();
		WeaponNoiseDeclaration();
	}
	/*--------------- PlayAttackSoundEvent ----------------*/

	/*---------------- KillerNotifyEvent ------------------*/
	public virtual void DeadNotifyEventFunc(Transform killer, Transform dead)
	{

	}
	/*---------------- KillerNotifyEvent ------------------*/

	/*----------------- WeaponNoiseEvent ------------------*/
	void WeaponNoiseDeclaration()
	{
		if (attackNoiseRadius > 0) {
			I_Manager.WeaponNoiseDeclaration(self, attackNoiseRadius);
		}
	}
	/*----------------- WeaponNoiseEvent ------------------*/
}
