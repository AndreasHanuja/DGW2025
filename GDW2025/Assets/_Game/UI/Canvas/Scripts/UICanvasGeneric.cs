using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UICanvas))]
public class UICanvasGeneric : MonoBehaviour
{
    //Globals
	private UICanvas ownUICanvas;
	protected UICanvas OwnUICanvas
	{
		get
		{
			if (ownUICanvas == null)
			{
				ownUICanvas = GetComponent<UICanvas>();
			}
			return ownUICanvas;
		}
	}
	public event Action OnActivate { add => OwnUICanvas.OnActivate += value; remove => OwnUICanvas.OnActivate -= value; }
	public event Action OnDeactivate { add => OwnUICanvas.OnDeactivate += value; remove => OwnUICanvas.OnDeactivate -= value; }
}
