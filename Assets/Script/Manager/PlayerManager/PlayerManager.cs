
/*-------------------------------------------
 * Player的核心管理类。
 * 分管PlayerMessenger,PlayerDataManager等等。
 *------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerManager : Manager {

	protected PlayerMessenger PlayerMSG;

	void Start () {
		PlayerMSG = transform.GetComponent<PlayerMessenger>();
	}
	
	void Update () {
		
	}

	void SelfHurtedFunc(Transform attacker, Transform suffer, float damage)
	{

	}

	public void HurtDeclaration(Transform owner, List<Transform> suffers, List<float> damages)
	{
		PlayerMSG.HurtDeclaration(owner, suffers, damages);
	}
	public void HurtDeclaration(Transform owner, List<Transform> suffers, float damage)
	{
		PlayerMSG.HurtDeclaration(owner, suffers, damage);
	}
	public void HurtDeclaration(Transform owner, Transform suffer, float damage)
	{
		PlayerMSG.HurtDeclaration(owner, suffer, damage);
	}

}
