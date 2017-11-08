using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
	[HideInInspector]
	public Transform owner;   // 此近战武器的拥有者
	[HideInInspector]
	public string weaponName;

	protected PlayerManager I_PlayerManager;  // attacker的PlayerManager管理类
	protected BodyAnimEvents I_BodyAnimEvents;
	protected Transform body;  // owner的Body子物体
	protected AudioSource attackAudioSource;  // 攻击音效

	protected void Awake()
	{
		owner = Utils.GetOwner(transform, Constant.TAGS.Attacker);
		if (owner) {
			I_PlayerManager = owner.GetComponent<PlayerManager>();
			body = owner.Find("Body");
			if (body) {
				I_BodyAnimEvents = body.GetComponent<BodyAnimEvents>();
			}
		}
	}

	protected void Start () {
		attackAudioSource = transform.GetComponent<AudioSource>();
	}

	protected void OnEnable()
	{
		I_BodyAnimEvents.PlayAttackSoundEvent += new BodyAnimEvents.PlayAttackSoundEventHandler(PlayAttackShoundEventFunc);
	}

	protected void OnDisable()
	{
		I_BodyAnimEvents.PlayAttackSoundEvent -= PlayAttackShoundEventFunc;
	}

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
	}
	
}
