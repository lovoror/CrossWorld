using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHealthRestore : MonoBehaviour {
	public GameObject instance;
	public Vector3 offset;
	public float dz;

	static GameObject root;
	Transform player;
	bool isActive;


	void Awake()
	{
		
	}

	void Start ()
	{
		root = new GameObject();
		root.name = "HealthRestoreRoot";
		DontDestroyOnLoad(root);
		player = PlayerData.Instance.target;
		isActive = false;
	}

	void OnEnable()
	{
		AttackOB.AddHealthEvent += new AttackOB.AddHealthEventHandler(AddHealthEventFunc);
	}

	void OnDisable()
	{
		AttackOB.AddHealthEvent -= AddHealthEventFunc;
	}

	void Update()
	{
		root.transform.position = player.position;
	}

	void AddHealthEventFunc(Transform attacker, float score)
	{
		if (attacker != player) return;
		GameObject uiScore = Instantiate(instance);
		uiScore.transform.parent = root.transform;
		float deltaZ = root.transform.childCount * dz;
		uiScore.transform.localPosition = offset + new Vector3(0, 0, deltaZ);
		SparkHealthRestore spark = uiScore.GetComponent<SparkHealthRestore>();
		spark.context = "+" + Mathf.Round(score);
	}

	public static void StageEnd()
	{
		Destroy(root);
	}
}
