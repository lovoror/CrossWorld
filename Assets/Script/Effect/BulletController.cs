using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public float speed = 30;
	[HideInInspector]
	public float damage = 10;
	[HideInInspector]
	public bool canPenetrate = false;

	[HideInInspector]
	public Transform shooter;

	private Vector3 direction;
	private Rigidbody rd;

	/*----------------- BulletHitEvent -----------------*/
	public delegate void BulletHitEventHandler(Transform shooter, Transform suffer, float damage);
	public static event BulletHitEventHandler BulletHitEvent;
	/*----------------- BulletHitEvent -----------------*/

	void Start ()
	{
		direction = transform.right;
		rd = transform.GetComponent<Rigidbody>();
		rd.velocity = direction * speed;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger) return;
		Transform suffer = Utils.GetOwner(other.transform, Constant.TAGS.Attackable);
		if (other.tag == "Wall") {
			Destroy(gameObject);
		}
		else if (suffer && suffer.tag != shooter.tag) {
			if (BulletHitEvent != null) {
				BulletHitEvent(shooter, suffer, damage);
			}
			if (!canPenetrate) {
				Destroy(gameObject);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{

	}
	
	void Update ()
	{
		
	}
}
