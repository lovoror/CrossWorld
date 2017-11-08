using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantWeaponManager : WeaponManager {

	public Transform bullet;
	public float damage = 10;
	public float bulletSpeed = 20;
	public bool canPenetrate = false;

	private Transform muzzle;

	new protected void Awake()
	{
		base.Awake();
		if (owner) {
			body = owner.Find("Body");
			if (body) {
				I_BodyAnimEvents = body.GetComponent<BodyAnimEvents>();
				foreach (Transform child in body) {
					if (child.tag == "RangeWeapon") {  // RangeWeapon的所在位置即为射击点
						muzzle = child;
					}
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

	void BulletShootEventFunc(Transform shooter, string weaponName)
	{
		Vector3 firePos = muzzle.position;
		Instantiate(bullet, firePos, body.rotation);
		BulletController I_BulletController = bullet.GetComponent<BulletController>();
		I_BulletController.shooter = shooter;
		I_BulletController.speed = bulletSpeed;
		I_BulletController.damage = damage;
		I_BulletController.canPenetrate = canPenetrate;
	}

	//void OnTriggerEnter(Collider other)
	//{
	//	if(other.tag == "")
	//}

	//void OnTriggerExit(Collider other)
	//{
	//	if (other.tag == "Wall") {

	//	}
	//}
	
	void Update ()
	{
		
	}
}
