using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour {
	Animator animator;
	AudioSource audio;

	bool isBroken = false;
	void Awake()
	{
		animator = GetComponent<Animator>();
		audio = GetComponent<AudioSource>();
	}

	void OnTriggerEnter(Collider other)
	{
		if ( !isBroken && other.tag == "Bullet") {
			isBroken = true;
			animator.SetTrigger("IsBroken");
			audio.Play();
		}
	}
}
