using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : DistantWeaponManager
{
	protected new void Awake()
	{
		base.Awake();
		weaponType = WeaponType.singleLoader;
		weaponName = WeaponNameType.Sniper;
	}

	protected new void Start()
	{
		base.Start();
	}
}
