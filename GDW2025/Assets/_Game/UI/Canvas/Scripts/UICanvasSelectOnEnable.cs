using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Selectable))]
public class UICanvasSelectOnEnable : MonoBehaviour
{
	//Globals
	private UICanvas parentCanvas;


	//Functions
	private void Awake()
	{
		GetParentCanvas();
		parentCanvas.OnActivate += Activate;
	}


	private void OnDestroy()
	{
		parentCanvas.OnActivate -= Activate;
	}


	private void Activate()
	{
		GetComponent<Selectable>().Select();
	}


	private void GetParentCanvas()
	{
		UICanvas parentCanvas = GetComponentInParent<UICanvas>(true);
		if (parentCanvas == null)
		{ return; }

		this.parentCanvas = parentCanvas;
	}
}
