using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringToSpriteRenderer
{
	public string key;
	public SpriteRenderer spriteRender;
}

enum AlertState {
	no,
	alert, 
	danger,
}

public class UiAlertDirection : MonoBehaviour
{
	public List<StringToSpriteRenderer> triangles;
	public float fadeTime = 0.5f;
	public float alertDist = 105;
	public Color c;
	float alertDistSq;
	bool playerIsDead;

	Dictionary<string, SpriteRenderer> d_Triangles;
	Transform player;
	Dictionary<Transform, bool> enemysInfo;  // enemy 及其是否发现player
	List<Transform> alertEnemys;  // 需要预警的敌人
	Dictionary<string, AlertState> alertInfo;  // 八方向预警信息

	void Awake()
	{
		enemysInfo = new Dictionary<Transform, bool>();
	}

	void OnEnable()
	{
		EnemysData.EnemysDataInitEvent += new EnemysData.EnemysDataInitEventHandler(EnemysDataInitEventFunc);
		InitData.NewEnemyEvent += new InitData.NewEnemyEventHandler(NewEnemyEventFunc);
		AttackOB.DeadNotifyEvent += new AttackOB.DeadNotifyEventHandler(DeadNotifyEventFunc);
		BehaviorTreeUpdate.EnemyAlertStateEvent += new BehaviorTreeUpdate.EnemyAlertStateEventHandler(EnemyAlertStateEventFunc);
	}

	void OnDisable()
	{
		EnemysData.EnemysDataInitEvent -= EnemysDataInitEventFunc;
		InitData.NewEnemyEvent -= NewEnemyEventFunc;
		AttackOB.DeadNotifyEvent -= DeadNotifyEventFunc;
		BehaviorTreeUpdate.EnemyAlertStateEvent -= EnemyAlertStateEventFunc;
	}

	void Start ()
	{
		playerIsDead = false;
		alertDistSq = alertDist * alertDist;
		player = PlayerData.Instance.target;
		alertEnemys = new List<Transform>();
		alertInfo = new Dictionary<string, AlertState>();
		d_Triangles = new Dictionary<string, SpriteRenderer>();
		foreach (var tr in triangles) {
			if (!d_Triangles.ContainsKey(tr.key)) {
				d_Triangles.Add(tr.key, tr.spriteRender);
			}
			tr.spriteRender.color = new Color(c.r, c.g, c.b, 0);
		}
		foreach (var key in d_Triangles.Keys) {
			if (!alertInfo.ContainsKey(key)) {
				alertInfo.Add(key, AlertState.no);
			}
		}
	}

	float deltaUpdateTime = 0.1f;
	float last_update_time = 0;
	void Update ()
	{
		transform.position = player.position;
		if (Time.time - last_update_time >= deltaUpdateTime) {
			last_update_time = Time.time;
			CheckAlertEnemys();
		}
		ShowAlert();
	}

	void CheckAlertEnemys()
	{
		foreach (Transform enemy in enemysInfo.Keys) {
			if (!alertEnemys.Contains(enemy) && (enemy.position - player.position).sqrMagnitude <= alertDistSq) {
				alertEnemys.Add(enemy);
			}
			else if (alertEnemys.Contains(enemy) && (enemy.position - player.position).sqrMagnitude > alertDistSq) {
				alertEnemys.Remove(enemy);
			}
		}
	}

	float tarAlpha = 0;
	float curFadeTime = 0;
	void ShowAlert()
	{
		// 重置alertInfo
		foreach (var key in d_Triangles.Keys) {
			alertInfo[key] = AlertState.no;
		}
		// 计算alertInfo
		foreach (var enemy in alertEnemys) {
			Vector3 dir3D = enemy.position - player.position;
			Vector2 dir = new Vector2(dir3D.x, dir3D.z);
			DirectionType8 dirType = Utils.GetDirection8(dir);
			bool isAlert = enemysInfo[enemy];
			switch (dirType) {
				case DirectionType8.Up:
					DealAlertInfo(enemy, "Up");
					break;
				case DirectionType8.UpRight:
					DealAlertInfo(enemy, "UpRight");
					break;
				case DirectionType8.Right:
					DealAlertInfo(enemy, "Right");
					break;
				case DirectionType8.DownRight:
					DealAlertInfo(enemy, "DownRight");
					break;
				case DirectionType8.Down:
					DealAlertInfo(enemy, "Down");
					break;
				case DirectionType8.DownLeft:
					DealAlertInfo(enemy, "DownLeft");
					break;
				case DirectionType8.Left:
					DealAlertInfo(enemy, "Left");
					break;
				case DirectionType8.UpLeft:
					DealAlertInfo(enemy, "UpLeft");
					break;
				default:
					break;
			}
		}
		// 根据AlertInfo显示信息
		curFadeTime += Time.deltaTime;
		float ca;
		if (curFadeTime <= fadeTime) {
			ca = curFadeTime / fadeTime;
			if (tarAlpha == 0) {
				ca = 1 - ca;
			}
			//spriteRender.color = new Color(c.r, c.g, c.b, ca);
		}
		else {
			ca = tarAlpha;
			tarAlpha = 1 - tarAlpha;
			curFadeTime = 0;
		}
		foreach (var info in alertInfo) {
			if (info.Value == AlertState.no) {
				d_Triangles[info.Key].color = new Color(c.r, c.g, c.b, 0);
			}
			else if (info.Value == AlertState.alert) {
				d_Triangles[info.Key].color = new Color(c.r, c.g, c.b, 1);
			}
			else if (info.Value == AlertState.danger) {
				d_Triangles[info.Key].color = new Color(c.r, c.g, c.b, ca);
			}
		}
	}

	void DealAlertInfo(Transform enemy, string key)
	{
		AlertState state = AlertState.no;
		if (enemysInfo.ContainsKey(enemy)) {
			bool isAlert = enemysInfo[enemy];
			state = isAlert ? AlertState.danger : AlertState.alert;
		}
		if (state == AlertState.danger) {
			alertInfo[key] = state;
		}
		else if (state == AlertState.alert) {
			alertInfo[key] = alertInfo[key] == AlertState.danger ? AlertState.danger : AlertState.alert;
		}
	}

	void ShowTriangle(string key, bool show)
	{
		if (d_Triangles.ContainsKey(key)) {
			d_Triangles[key].gameObject.SetActive(show);
		}
	}

	void EnemysDataInitEventFunc(Transform[] enemys)
	{
		foreach (Transform enemy in enemys) {
			if (!this.enemysInfo.ContainsKey(enemy)) {
				this.enemysInfo.Add(enemy, false);
			}
		}
	}

	void NewEnemyEventFunc(Transform enemy)
	{
		if (!enemysInfo.ContainsKey(enemy)) {
			enemysInfo.Add(enemy, false);
		}
	}

	void DeadNotifyEventFunc(Transform killer, Transform dead, WeaponNameType weapon)
	{
		if (dead == player) {
			playerIsDead = true;
			transform.gameObject.SetActive(false);
		}
		if (enemysInfo.ContainsKey(dead)) {
			enemysInfo.Remove(dead);
		}
		if (alertEnemys.Contains(dead)) {
			alertEnemys.Remove(dead);
		}
	}

	void EnemyAlertStateEventFunc(Transform enemy, bool isAlert)
	{
		if (enemysInfo.ContainsKey(enemy)) {
			enemysInfo[enemy] = isAlert;
		}
	}
}
