using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantWeaponManager : WeaponManager {

	public Transform bullet;
	float damage = 10;
	public float bulletSpeed = 100;
	public bool canPenetrate = false;
	public Transform muzzle;

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
	}
	protected new void OnDisable()
	{
		base.OnDisable();
	}

	protected override void AttackEventFunc()
	{
		base.AttackEventFunc();
		CreateBullet(self, I_Manager.GetCurWeaponName());
	}

	void CreateBullet(Transform shooter, WeaponNameType weaponName)
	{
		Vector3 firePos = muzzle.position;
		Transform I_Bullet = Instantiate(bullet, firePos, body.rotation);
		BulletController I_BulletController = I_Bullet.GetComponent<BulletController>();
		I_BulletController.Init(shooter, damage, bulletSpeed, canPenetrate);
	}
}
