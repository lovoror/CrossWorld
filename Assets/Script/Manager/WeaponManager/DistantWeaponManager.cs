using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantWeaponManager : WeaponManager {

	public Transform bullet;
	public float damage = 10;
	public float bulletSpeed = 100;
	public bool canPenetrate = false;

	private Transform muzzle;

	new protected void Awake()
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
	new void Start ()
	{
		base.Start();
	}

	new void OnEnable()
	{
		base.OnEnable();
		I_BodyAnimEvents.BulletShootEvent += new BodyAnimEvents.BulletShootEventHandler(BulletShootEventFunc);
	}
	new void OnDisable()
	{
		base.OnEnable();
		I_BodyAnimEvents.BulletShootEvent -= BulletShootEventFunc;
	}


	/*------------------ BulletShootEvent ------------------*/
	void BulletShootEventFunc(Transform shooter, string weaponName)
	{
		Vector3 firePos = muzzle.position;
		Transform I_Bullet = Instantiate(bullet, firePos, body.rotation);
		BulletController I_BulletController = I_Bullet.GetComponent<BulletController>();
		I_BulletController.Init(shooter, damage, bulletSpeed, canPenetrate);
	}
	/*------------------ BulletShootEvent ------------------*/
}
