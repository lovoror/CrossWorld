//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	[HideInInspector]
	public Manager I_Manager;
	protected BaseData I_BaseData
	{
		get
		{
			return I_Manager.I_BaseData;
		}
	}
	protected Transform self;
	protected Transform body
	{
		get
		{
			return PlayerData.Instance.curBodyTransform;
		}
	}
	protected Transform leg
	{
		get
		{
			return PlayerData.Instance.legTransform;
		}
	}
	protected SphereCollider bodyCollider;
	protected AudioSource reloadAudio
	{
		get
		{
			Transform weapon = I_BaseData.curWeaponTransform;
			if (weapon) {
				AudioSource[] audios = weapon.GetComponents<AudioSource>();
				if (audios.Length >= 2) {
					return audios[1];
				}
			}
			return null;
		}
	}
	protected Animator legAnim;
	protected Animator bodyAnim
	{
		get
		{
			return body.GetComponent<Animator>();
		}
	}
	protected Camera mainCamera;
	protected AnimatorStateInfo bodyAnimInfo
	{
		get
		{
			return bodyAnim.GetCurrentAnimatorStateInfo(0);
		}
	}
	protected WeaponNameType curWeaponName
	{
		get
		{
			return I_Manager.GetCurWeaponName();
		}
	}

	protected void Awake()
	{
		I_Manager = transform.GetComponent<Manager>();
		self = transform;
		bodyCollider = transform.GetComponent<SphereCollider>();
		legAnim = leg.GetComponent<Animator>();
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}

	protected void Start ()
	{

	}

	protected void OnEnable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent += new Messenger.DeadNotifyEventHandler(DeadNotifyEventFunc);
		I_Manager.I_AnimEventsManager.PlayReloadSoundEvent += new AnimEventsManager.PlayReloadSoundEventHandler(PlayReloadSoundEventFunc);
	}

	protected void OnDisable()
	{
		I_Manager.I_Messenger.DeadNotifyEvent -= DeadNotifyEventFunc;
		I_Manager.I_AnimEventsManager.PlayReloadSoundEvent -= PlayReloadSoundEventFunc;
	}

	protected void Update()
	{

	}

	/*-------------------- DeadEvent ---------------------*/
	protected virtual void DeadNotifyEventFunc(Transform killer, Transform dead)
	{
		if (transform == killer) {

		}
		else if (transform == dead) {
			// 设置死亡状态
			ShowDeadAnim(true);
		}
	}
	/*-------------------- DeadEvent ---------------------*/

	/*-------------------- PlayReloadSoundEvent ---------------------*/
	void PlayReloadSoundEventFunc()
	{
		if (reloadAudio) {
			reloadAudio.Play();
		}
	}
	/*-------------------- PlayReloadSoundEvent ---------------------*/

	/*------------------ 状态机 ------------------*/
	protected void ShowWalkAnim(float speedRate)
	{
		legAnim.SetFloat("Speed", speedRate);
		bodyAnim.SetFloat("Speed", speedRate);
	}

	protected void ShowAttackAnim(bool show)
	{
		bodyAnim.SetBool("Attack", show);
	}

	protected void AttackOnce()
	{
		bodyAnim.SetBool("OnceAttack", true);
	}

	protected void ShowDeadAnim(bool show)
	{
		legAnim.SetBool("Dead", show);
		Random rnd = new Random();
		Random.InitState((int)System.DateTime.Now.Ticks);
		int deadState = Mathf.FloorToInt(Random.value * 8);
		bodyAnim.SetInteger("DeadState", deadState);
		bodyAnim.SetBool("Dead", show);
	}

	public void ShowReloadAnim()
	{
		bodyAnim.SetTrigger("Reload");
		//AnimatorStateInfo stateInfo = bodyAnim.GetCurrentAnimatorStateInfo(0);
		//if (!stateInfo.IsName("Reload")) {
		//	bodyAnim.SetTrigger("Reload");
		//	AudioSource reloadAudio = this.reloadAudio;
		//	if (reloadAudio) {
		//		reloadAudio.Play();
		//	}
		//}
	}
	/*------------------ 状态机 ------------------*/
}
