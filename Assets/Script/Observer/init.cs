using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class init : MonoBehaviour {
	void Awake()
	{

	}

	void Start ()
	{
		int enemyCollider = LayerMask.NameToLayer("Enemy");
		//int playerCollider = LayerMask.NameToLayer("Player");
		Physics.IgnoreLayerCollision(enemyCollider, enemyCollider);
		//Physics.IgnoreLayerCollision(playerCollider, enemyCollider);
	}
}
