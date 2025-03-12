using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;


[RequireComponent(typeof(UIControlsButtonButton))]
public class UIControlsButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
{
	//Settings
	[InputControl(layout = "Button")]
	[SerializeField]
	private string m_ControlPath;


	//Globals
	private UIControlsButtonButton ownButton;
	public bool Interactable { get => ownButton.interactable; set { ownButton.interactable = value; } }
	private bool pressed = false;



	//Functions
	private void Awake()
	{
		ownButton = GetComponent<UIControlsButtonButton>();
	}


	public void OnPointerUp(PointerEventData data)
	{
		if (Interactable || pressed)
		{
			SendValueToControl(0.0f);
			pressed = false;
		}
	}


	public void OnPointerDown(PointerEventData data)
	{
		if (Interactable)
		{
			SendValueToControl(1.0f);
			pressed = true;
		}
	}


	protected override string controlPathInternal
	{
		get => m_ControlPath;
		set => m_ControlPath = value;
	}
}
