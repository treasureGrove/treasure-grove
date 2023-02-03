using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Dialog Content Template", menuName = "GKC/Create Dialog Content Template", order = 51)]
public class dialogContentTemplate : ScriptableObject
{
	public string Name;
	public int ID = 0;

	[Space]

	public List<completeDialogInfoTemplate> completeDialogInfoTemplateList = new List<completeDialogInfoTemplate> ();
}