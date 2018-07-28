using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour {
	Animator animator;
	AudioSource crashAudio;

	bool isBroken = false;
	void Awake()
	{
		animator = GetComponent<Animator>();
		crashAudio = GetComponent<AudioSource>();
	}

	void OnTriggerEnter(Collider other)
	{
		if ( !isBroken && other.tag == "Bullet") {
			isBroken = true;
			animator.SetTrigger("IsBroken");
			crashAudio.Play();
		}
	}
}
