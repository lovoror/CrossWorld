using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M16 : DistantWeaponManager {

	protected new void Awake()
	{
		base.Awake();
	}

	protected new void Start()
	{
		base.Start();
		weaponName = (int) Constant.WEAPON_NAME.M16;
	}
}
