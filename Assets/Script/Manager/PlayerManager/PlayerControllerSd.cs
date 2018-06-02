//#define KEYBOARD_CONTROL

using UnityEngine;
using System.Collections;

public class PlayerControllerSd : MonoBehaviour
{
	public Animator bodyAnim;


	void Awake()
	{

	}

	void Start()
	{
		
	}

	void OnEnable()
	{
		MoboControllerSd.PlayerAttackEvent += new MoboControllerSd.PlayerAttackEventHandler(PlayerAttackEventFunc);

	}

	void OnDisable()
	{
		MoboControllerSd.PlayerAttackEvent -= PlayerAttackEventFunc;

	}

	void Reset()
	{

	}

	void Update()
	{
;

	}

	void LateUpdate()
	{
		
	}

	void PlayerAttackEventFunc(AttackSdType attackType)
	{
		switch (attackType) {
			/*-------- 点击 --------*/
			case AttackSdType.A:  // 轻攻击
				ResetTriggers();
				bodyAnim.SetTrigger("A");
				break;
			case AttackSdType.AU:
				ResetTriggers();
				bodyAnim.SetTrigger("AU");
				break;
			case AttackSdType.AD:
				ResetTriggers();
				bodyAnim.SetTrigger("AD");
				break;
			case AttackSdType.AL:
				ResetTriggers();
				bodyAnim.SetTrigger("AL");
				break;
			case AttackSdType.AR:
				ResetTriggers();
				bodyAnim.SetTrigger("AR");
				break;
			/*-------- 点击释放 --------*/
			case AttackSdType.ARN:
				ResetTriggers();
				bodyAnim.SetTrigger("ARN");
				break;
			case AttackSdType.ARU:
				ResetTriggers();
				bodyAnim.SetTrigger("ARU");
				break;
			case AttackSdType.ARD:  // 闪避
				ResetTriggers();
				bodyAnim.SetTrigger("ARD");
				break;
			case AttackSdType.ARL:
				ResetTriggers();
				bodyAnim.SetTrigger("ARL");
				break;
			case AttackSdType.ARR:  // 重攻击
				ResetTriggers();
				bodyAnim.SetTrigger("ARR");
				break;
			/*-------- 长按 --------*/
			case AttackSdType.HA:   // 蓄力
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				break;
			case AttackSdType.HAU:
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				bodyAnim.SetBool("HAU", true);
				break;
			case AttackSdType.HAD:
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				bodyAnim.SetBool("HAD", true);
				break;
			case AttackSdType.HAL:
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				bodyAnim.SetBool("HAL", true);
				break;
			case AttackSdType.HAR:
				ResetTriggers();
				ResetHASWithoutHA();
				bodyAnim.SetBool("HA", true);
				bodyAnim.SetBool("HAR", true);
				break;
			/*-------- 长按释放 --------*/
			case AttackSdType.HARN:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARN");
				break;
			case AttackSdType.HARU:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARU");
				break;
			case AttackSdType.HARD:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARD");
				break;
			case AttackSdType.HARL:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARL");
				break;
			case AttackSdType.HARR:
				ResetTriggers();
				ResetHAS();
				bodyAnim.SetTrigger("HARR");
				break;
		}
	}

	public void ResetTriggers()
	{
		bodyAnim.ResetTrigger("A");
		bodyAnim.ResetTrigger("AU");
		bodyAnim.ResetTrigger("AD");
		bodyAnim.ResetTrigger("AL");
		bodyAnim.ResetTrigger("AR");
		bodyAnim.ResetTrigger("ARN");
		bodyAnim.ResetTrigger("ARU");
		bodyAnim.ResetTrigger("ARD");
		bodyAnim.ResetTrigger("ARL");
		bodyAnim.ResetTrigger("ARR");
		bodyAnim.ResetTrigger("HARN");
		bodyAnim.ResetTrigger("HARU");
		bodyAnim.ResetTrigger("HARD");
		bodyAnim.ResetTrigger("HARL");
		bodyAnim.ResetTrigger("HARR");
	}

	void ResetHAS()
	{
		bodyAnim.SetBool("HA", false);
		ResetHASWithoutHA();
	}

	void ResetHASWithoutHA()
	{
		bodyAnim.SetBool("HAU", false);
		bodyAnim.SetBool("HAD", false);
		bodyAnim.SetBool("HAL", false);
		bodyAnim.SetBool("HAR", false);
	}
}
