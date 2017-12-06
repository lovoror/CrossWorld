using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class init : MonoBehaviour {
	void Awake()
	{

	}

	void Start ()
	{
		int enemyColliderLayer = LayerMask.NameToLayer("EnemyCollider");
		int playerColliderLayer = LayerMask.NameToLayer("PlayerCollider");
		Physics.IgnoreLayerCollision(enemyColliderLayer, enemyColliderLayer);
		Physics.IgnoreLayerCollision(playerColliderLayer, enemyColliderLayer);
	}
}
