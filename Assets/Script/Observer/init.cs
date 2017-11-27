using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class init : MonoBehaviour {
	public Collider enemyCollider;
	void Awake()
	{

	}

	void Start ()
	{
		int enemyColliderLayer = LayerMask.NameToLayer("EnemyCollider");
		Physics.IgnoreLayerCollision(enemyColliderLayer, enemyColliderLayer);
	}
}
