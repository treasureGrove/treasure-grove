using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeWeaponAttackListInfoSocket : MonoBehaviour
{
	public meleeWeaponAttackInfo mainMeleeWeaponAttackInfo;
	public grabPhysicalObjectMeleeAttackSystem mainGrabPhysicalObjectMeleeAttackSystem;

	public void copyWeaponInfoToTemplate (bool settingInfoOnEditorTime)
	{
		if (mainGrabPhysicalObjectMeleeAttackSystem != null) {
			mainGrabPhysicalObjectMeleeAttackSystem.setNewMeleeWeaponAttackInfoTemplate (mainMeleeWeaponAttackInfo, settingInfoOnEditorTime);

			mainGrabPhysicalObjectMeleeAttackSystem.copyWeaponInfoToTemplate (settingInfoOnEditorTime);
		}
	}

	public void copyTemplateToWeaponAttackInfo (bool settingInfoOnEditorTime)
	{
		if (mainGrabPhysicalObjectMeleeAttackSystem != null) {
			mainGrabPhysicalObjectMeleeAttackSystem.setNewMeleeWeaponAttackInfoTemplate (mainMeleeWeaponAttackInfo, settingInfoOnEditorTime);

			mainGrabPhysicalObjectMeleeAttackSystem.copyTemplateToWeaponAttackInfo (settingInfoOnEditorTime);
		}
	}
}
