using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {
	public AudioSource nomalAudio;
	public AudioSource panicAudio;
	public float maxVolume = 0.5f;
	public float volumeSmooth = 5f;

	Dictionary<Transform, bool> d_AlertState;
	bool isAlert;  // 是否有敌人处于Alert状态


	void Awake()
	{
		d_AlertState = new Dictionary<Transform, bool>();
		isAlert = false;
	}

	void Start ()
	{
		nomalAudio.volume = maxVolume;
		panicAudio.volume = 0;
		nomalAudio.Play();
		panicAudio.Play();
	}

	void OnEnable()
	{
		BehaviorTreeUpdate.EnemyAlertStateEvent += new BehaviorTreeUpdate.EnemyAlertStateEventHandler(EnemyAlertStateEventFunc);
	}

	void OnDisable()
	{
		BehaviorTreeUpdate.EnemyAlertStateEvent -= EnemyAlertStateEventFunc;
	}

	void Update()
	{
		if (isAlert) {
			nomalAudio.volume = Mathf.Lerp(nomalAudio.volume, 0, volumeSmooth * Time.deltaTime);
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, maxVolume, volumeSmooth * Time.deltaTime);
		}
		else {
			nomalAudio.volume = Mathf.Lerp(nomalAudio.volume, maxVolume, volumeSmooth * Time.deltaTime);
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0, volumeSmooth * Time.deltaTime);
		}
	}

	void EnemyAlertStateEventFunc(Transform enemy, bool isAlert)
	{
		if (d_AlertState.ContainsKey(enemy)) {
			d_AlertState[enemy] = isAlert;
		}
		else {
			d_AlertState.Add(enemy, isAlert);
		}
		CheckAlertState();
	}

	void CheckAlertState()
	{
		bool curAlertState = d_AlertState.ContainsValue(true);
		if (curAlertState != isAlert) {
			isAlert = curAlertState;
		}
	}
}
