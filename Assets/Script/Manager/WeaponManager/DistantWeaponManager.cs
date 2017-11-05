using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantWeaponManager : WeaponManager {

	public Transform bullet;

	protected BodyAnimEvents I_BodyAnimEvents;
	protected Transform body;  // owner的Body子物体

	private Transform muzzle;

	new void Awake()
	{
		base.Awake();
		if (owner) {
			body = owner.Find("Body");
			if (body) {
				I_BodyAnimEvents = body.GetComponent<BodyAnimEvents>();
				foreach (Transform child in body) {
					if (child.tag == "FirePos") {
						muzzle = child;
					}
				}
			}
		}
	}
	void Start ()
	{
		
	}

	void OnEnable()
	{
		I_BodyAnimEvents.BulletShootEvent += new BodyAnimEvents.BulletShootEventHandler(BulletShootEventFunc);
	}
	void OnDisable()
	{
		I_BodyAnimEvents.BulletShootEvent -= BulletShootEventFunc;
	}

	void BulletShootEventFunc(Transform shooter, string weaponName)
	{
		Vector3 firePos = muzzle.position;
		Instantiate(bullet, firePos, body.rotation);
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
