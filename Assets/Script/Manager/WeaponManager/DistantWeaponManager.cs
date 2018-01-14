using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantWeaponManager : WeaponManager {

	public Transform bullet;
	public float damage = 10;
	public float bulletSpeed = 100;
	public bool canPenetrate = false;

	private Transform muzzle;

	protected new void Awake()
	{
		base.Awake();
		if (self) {
			body = self.Find("Body");
			if (body) {
				I_BodyAnimEvents = body.GetComponent<BodyAnimEvents>();
			}
			foreach (Transform child in self) {
				if (child.tag == "RangeWeapon") {  // RangeWeapon的所在位置即为射击点
					muzzle = child;
				}
			}
		}
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
		base.OnEnable();
	}

	protected override void AttackEventFunc()
	{
		base.AttackEventFunc();
		CreateBullet(self, I_Manager.GetWeaponName());
	}

	void CreateBullet(Transform shooter, WeaponNameType weaponName)
	{
		Vector3 firePos = muzzle.position;
		Transform I_Bullet = Instantiate(bullet, firePos, body.rotation);
		BulletController I_BulletController = I_Bullet.GetComponent<BulletController>();
		I_BulletController.Init(shooter, damage, bulletSpeed, canPenetrate);
	}
}
