using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
	[HideInInspector]
	public Transform self;   // 此近战武器的拥有者
	[HideInInspector]
	public WeaponNameType weaponName { get; set; }
	[HideInInspector]
	public WeaponType weaponType;
	[HideInInspector]
	public int curWeaponLevel
	{
		get
		{
			return I_BaseData.curWeaponLevel;
		}
	}

	protected Manager I_Manager;  // attacker的Manager管理类
	protected AnimEventsManager I_AnimEventsManager { get; set; }
	protected AudioSource attackAudioSource;      // 攻击音效
	protected AudioSource attackDoneAudioSource;  // 攻击结束音效
	protected BaseData I_BaseData { get; set; }
	protected Transform body
	{
		get
		{
			return I_BaseData.curBodyTransform;
		}
	}

	protected void Awake()
	{
		self = Utils.GetOwner(transform, Constant.TAGS.Attacker);
		if (self) {
			I_Manager = self.GetComponent<Manager>();
		}
		I_AnimEventsManager = I_Manager.I_AnimEventsManager;
	}

	protected void Start () {
		I_BaseData = I_Manager.I_BaseData;
		attackAudioSource = transform.GetComponent<AudioSource>();

		Transform sndAttackDone = transform.Find("SndAttackDone");
		if (sndAttackDone != null) {
			attackDoneAudioSource = sndAttackDone.GetComponent<AudioSource>();
		}
	}

	protected void OnEnable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent += new Messenger.DeadNotifyEventHandler(DeadNotifyEventFunc);
		I_AnimEventsManager.AttackEvent += new AnimEventsManager.AttackEventHandler(AttackEventFunc);
		I_AnimEventsManager.AttackEndEvent += new AnimEventsManager.AttackEndEventHandler(AttackEndEventFunc);
	}

	protected void OnDisable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent -= DeadNotifyEventFunc;
		I_AnimEventsManager.AttackEvent -= AttackEventFunc;
		I_AnimEventsManager.AttackEndEvent -= AttackEndEventFunc;
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

	protected virtual void AttackEventFunc(float doneAttackSndTime = 0)
	{
		PlayAttackSound(doneAttackSndTime);
	}

	protected virtual void AttackEndEventFunc()
	{
		
	}

	protected virtual void PlayAttackSound(float doneAttackSndTime)
	{
		attackAudioSource.Play();
		if (doneAttackSndTime > 0) {
			Invoke("PlayAttackDoneSound", doneAttackSndTime);
		}
	}

	protected virtual void PlayAttackDoneSound()
	{
		attackDoneAudioSource.Play();
	}



	/*----------------- EnemyInATKRangeEvent ------------------*/
	public delegate void EnemyInATKRangeEventHandler(Transform enemy, bool isInRange);
	public event EnemyInATKRangeEventHandler EnemyInATKRangeEvent;

	/*----------------- EnemyInATKRangeEvent ------------------*/

	/*--------------------- HurtEvent ---------------------*/
		/*--------- Weapon -> Manager ---------*/
	// 通知PlayerManager伤害了某（些）人
	protected void BasicHurt(Transform attacker, List<Transform> suffers)
	{
		I_Manager.HurtDeclaration(attacker, suffers);
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

	protected int GetWeaponLevel(WeaponNameType weaponName)
	{
		return I_BaseData.GetWeaponLevel(weaponName);
	}
}
