using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M16 : DistantWeaponManager {

	new void Awake()
	{
		base.Awake();
	}

	new void Start()
	{
		base.Start();
		weaponName = "M16";
	}
}
