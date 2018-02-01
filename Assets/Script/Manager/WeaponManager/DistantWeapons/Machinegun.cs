using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machinegun : DistantWeaponManager
{
	protected new void Awake()
	{
		base.Awake();
		weaponType = WeaponType.autoDistant;
		weaponName = WeaponNameType.Machinegun;
	}

	protected new void Start()
	{
		base.Start();
	}
}
