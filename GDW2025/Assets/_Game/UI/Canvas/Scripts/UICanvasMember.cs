using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UICanvasMember : MonoBehaviour
{
	//Settings
	[SerializeField]
	public UnityEvent OnAwake;
	[SerializeField]
	public UnityEvent OnParentCanvasActivate;
	[SerializeField]
	public UnityEvent OnParentCanvasDeactivate;


	//Globals
	private UICanvas parentCanvas;


	//Functions
	private void Awake()
	{
		GetParentCanvas();
		parentCanvas.OnActivate += OnParentCanvasActivate.Invoke;
		parentCanvas.OnDeactivate += OnParentCanvasDeactivate.Invoke;

		OnAwake.Invoke();
	}


	private void OnDestroy()
	{
		parentCanvas.OnActivate -= OnParentCanvasActivate.Invoke;
		parentCanvas.OnDeactivate -= OnParentCanvasDeactivate.Invoke;
	}


	private void GetParentCanvas()
	{
		UICanvas parentCanvas = GetComponentInParent<UICanvas>(true);
		if (parentCanvas == null)
		{ return; }

		this.parentCanvas = parentCanvas;
	}
}
