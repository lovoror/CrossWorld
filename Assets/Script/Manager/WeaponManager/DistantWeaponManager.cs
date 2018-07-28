using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantWeaponManager : WeaponManager {

	public Transform bullet;
	public float bulletSpeed = 100;
	public Transform muzzle;

	MyDelegate.vfv ReloadCallback = null;
	int leftBullets
	{
		get
		{
			return I_BaseData.GetCurLeftBullets();
		}
	}
	float damage = 10;  // 并无作用

	protected Transform body
	{
		get
		{
			return I_BaseData.curBodyTransform;
		}
	}

	protected Animator bodyAnim
	{
		get
		{
			return body.GetComponent<Animator>();
		}
	}

	protected new void Awake()
	{
		
		base.Awake();
	}
	protected new void Start ()
	{
		base.Start();
	}

	protected new void OnEnable()
	{
		base.OnEnable();
		I_Manager.I_AnimEventsManager.OnReloadEndEvent += new AnimEventsManager.OnReloadEndEventHandler(OnReloadEndEventFunc);
	}
	protected new void OnDisable()
	{
		base.OnDisable();
		I_Manager.I_AnimEventsManager.OnReloadEndEvent -= OnReloadEndEventFunc;
	}

	/************************* OnReloadEnd **************************/
	void OnReloadEndEventFunc()
	{
		I_BaseData.ReloadWeapon();
		if (ReloadCallback != null) {
			ReloadCallback();
			ReloadCallback = null;
		}
	}
	/************************* OnReloadEnd **************************/

	protected override void AttackEventFunc()
	{
		base.AttackEventFunc();
		if (leftBullets > 0) {
			CreateBullet(self, I_Manager.GetCurWeaponName());
			I_BaseData.ShootOneBullet();
		}
		else {
			Reload();
		}
	}
	protected override void AttackEndEventFunc()
	{
		base.AttackEndEventFunc();
		if (leftBullets <= 0) {
			Reload();
		}
	}

	public void Reload(MyDelegate.vfv callback = null)
	{
		bool reloading = bodyAnim.GetBool("Reload");
		if (!reloading) {
			ReloadCallback = callback;
			I_Manager.I_Controller.ShowReloadAnim();
		}
	}

	protected virtual void CreateBullet(Transform shooter, WeaponNameType weaponName)
	{
		Vector3 firePos = muzzle.position;
		Transform I_Bullet = Instantiate(bullet, firePos, body.rotation);
		BulletController I_BulletController = I_Bullet.GetComponent<BulletController>();
		bool canPenetrate = Utils.CanBulletPenetrate(weaponName, GetWeaponLevel(weaponName));
		I_BulletController.Init(shooter, damage, bulletSpeed, canPenetrate);
	}
}
