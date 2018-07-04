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
		int i_score = (int)Mathf.Round(score);
		if (i_score <= 0) return;
		// 上移所有子节点
		foreach (Transform child in root.transform) {
			Vector3 p = child.localPosition;
			child.localPosition = new Vector3(p.x, p.y, p.z + dz);
		}
		// 添加分数显示
		GameObject uiScore = Instantiate(instance);
		uiScore.transform.parent = root.transform;
		uiScore.transform.localPosition = offset + new Vector3(0, 0, 0);
		SparkHealthRestore spark = uiScore.GetComponent<SparkHealthRestore>();
		spark.context = "+" + i_score;
	}

	public static void StageEnd()
	{
		Destroy(root);
	}
}
