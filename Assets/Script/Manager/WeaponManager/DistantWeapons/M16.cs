using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M16 : DistantWeaponManager {

	protected new void Awake()
	{
		base.Awake();
		weaponType = WeaponType.autoDistant;
		weaponName = WeaponNameType.M16;
	}

	protected new void Start()
	{
		base.Start();
	}
}
