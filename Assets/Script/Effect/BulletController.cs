using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	//[HideInInspector]
	public float speed = 100;
	[HideInInspector]
	public float damage = 10;
	[HideInInspector]
	public bool canPenetrate = false;

	[HideInInspector]
	public Transform shooter;

	private Vector3 direction;
	private Rigidbody rd;

	public void Init(Transform shooter, float damage = 10, float speed = 100, bool canPenetrate = false)
	{
		this.shooter = shooter;
		this.damage = damage;
		this.speed = speed;
		this.canPenetrate = canPenetrate;
	}

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
		else if (suffer != null && suffer.tag != shooter.tag) {
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
