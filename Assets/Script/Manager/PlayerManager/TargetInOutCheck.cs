using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InOutType
{
	IN, OUT
}

public class TargetInOutCheck : MonoBehaviour {
	public List<string> targetTags;
	public Rect inRect;
	public Rect outRect;

	PlayerData I_PlayerData;
	PlayerMessenger I_PlayerMessenger;
	List<Transform> targets = new List<Transform>();
	List<Transform> inRangeTargets = new List<Transform>();  // 在可选中范围内的目标
	List<Transform> outRangeTargets = new List<Transform>(); // 在可选中范围外的目标
	bool hasTargetsIn = false;
	bool hasTargetsOut = false;
	float checkDeltaTime = 0.1f;
	float lastCheckTime = -1;
	void Awake()
	{
		I_PlayerData = PlayerData.Instance;
		I_PlayerMessenger = transform.GetComponentInParent<PlayerMessenger>();
		foreach (string tag in targetTags) {
			GameObject[] tagTargets = GameObject.FindGameObjectsWithTag(tag);
			for (int i = 0; i < tagTargets.Length; ++i) {
				targets.Add(tagTargets[i].transform);
			}
		}
		foreach (Transform target in targets) {
			outRangeTargets.Add(target);
		}
	}

	void OnEnable()
	{
		InitData.NewEnemyEvent += new InitData.NewEnemyEventHandler(NewEnemyEventFunc);
	}

	void OnDisable()
	{
		InitData.NewEnemyEvent -= NewEnemyEventFunc;
	}

	void Start ()
	{
		
	}

	void Update()
	{
		// 每checkDeltaTime进行一次检查
		if (Time.realtimeSinceStartup > lastCheckTime + checkDeltaTime) {
			lastCheckTime = Time.realtimeSinceStartup;
			bool hasDeads = RemoveDeads();
			CheckTargetsIn();
			CheckTargetsOut();
			if (hasDeads || hasTargetsIn || hasTargetsOut) {
				I_PlayerData.I_FocusTargets.SetTargets(inRangeTargets);
				I_PlayerMessenger.FocusTragetsChange();
			}
		}
	}

	// 是否有目标进入选中范围
	bool CheckTargetsIn()
	{
		hasTargetsIn = false;
		Vector2 o = new Vector2(transform.position.x, transform.position.z);
		List<Transform> removeTemp = new List<Transform>();
		foreach (Transform target in outRangeTargets) {
			Vector2 t = new Vector2(target.position.x, target.position.z);
			Vector2 offset = new Vector2(t.x - o.x, o.y - t.y);  // Rect的坐标系Y轴与Unity坐标系相反
			if (inRect.Contains(offset)) {
				if (!inRangeTargets.Contains(target)) {
					if (!Utils.IsDead(target)) {
						inRangeTargets.Add(target);
					}
				}
				removeTemp.Add(target);
				hasTargetsIn = true;
			}
		}
		foreach (Transform target in removeTemp) {
			outRangeTargets.Remove(target);
		}
		return hasTargetsIn;
	}

	// 是否有目标脱离选中范围
	bool CheckTargetsOut()
	{
		hasTargetsOut = false;
		Vector2 o = new Vector2(transform.position.x, transform.position.z);
		List<Transform> removeTemp = new List<Transform>();
		foreach (Transform target in inRangeTargets) {
			Vector2 t = new Vector2(target.position.x, target.position.z);
			Vector2 offset = new Vector2(t.x - o.x, o.y - t.y);  // Rect的坐标系Y轴与Unity坐标系相反
			if (!outRect.Contains(offset)) {
				if (!outRangeTargets.Contains(target)) {
					outRangeTargets.Add(target);
				}
				removeTemp.Add(target);
				hasTargetsOut = true;
			}
		}
		foreach (Transform target in removeTemp) {
			inRangeTargets.Remove(target);
		}
		return hasTargetsOut;
	}

	// 删除死亡的角色
	bool RemoveDeads()
	{
		bool hasDeads = false;
		List<Transform> removeTemp = new List<Transform>();
		foreach (Transform target in inRangeTargets) {
			bool isDead = Utils.IsDead(target);
			if (isDead) {
				removeTemp.Add(target);
				hasDeads = true;
			}
		}
		foreach (Transform target in removeTemp) {
			inRangeTargets.Remove(target);
		}
		removeTemp.Clear();
		foreach (Transform target in outRangeTargets) {
			bool isDead = Utils.IsDead(target);
			if (isDead) {
				removeTemp.Add(target);
				hasDeads = true;
			}
		}
		foreach (Transform target in removeTemp) {
			outRangeTargets.Remove(target);
		}
		return hasDeads;
	}

	void NewEnemyEventFunc(Transform enemy)
	{
		if (!outRangeTargets.Contains(enemy)) {
			outRangeTargets.Add(enemy);
		}
	}
}
