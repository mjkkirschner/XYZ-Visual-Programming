using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using Nodeplay.Engine;

public class CustomNodeLibraryButton : LibraryButton
{

	public  CustomNodeInfo Info {get;set;}

	public virtual void initializeButtonFromNodeInfo(CustomNodeInfo info)
	{
		this.NameLabel.text = info.Name;
		Debug.Log("name is" + info.Name);
		this.LoadedType = null;
		this.Info = info;
	}

	
}
