using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MeleeWeaponManager
{
	private string[] cutSounds = { "Sounds/sndCut1", "Sounds/sndCut2" };
	private string unCutSound = "Sounds/sndKnife";
	private int cutCount = 0;

	new void Awake()
	{
		base.Awake();
	}

	new void Start()
	{
		base.Start();
		weaponName = (int)Constant.WEAPON_NAME.Knife;
	}

	protected override void PlayAttackShoundEventFunc()
	{
		AudioClip clip;
		if (HasEnemyInRange()) {
			clip = Resources.Load(cutSounds[cutCount], typeof(AudioClip)) as AudioClip;
			cutCount++;
			cutCount = cutCount >= cutSounds.Length ? 0 : cutCount;
		}
		else {
			clip = Resources.Load(unCutSound, typeof(AudioClip)) as AudioClip;
		}
		attackAudioSource.clip = clip;
		attackAudioSource.Play();
	}
}
