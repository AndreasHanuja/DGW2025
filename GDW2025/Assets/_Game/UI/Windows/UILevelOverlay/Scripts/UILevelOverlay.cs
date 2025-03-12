using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;

public class UILevelOverlay : UICanvasGeneric
{
	//Settings
	[SerializeField]
	private Button menuButton;


	//Functions
	private void Start()
	{
		GameManager.Instance.OnTransitioned += OnLevelManagerTransition;
	}

	private void OnDestroy()
	{
		GameManager.Instance.OnTransitioned -= OnLevelManagerTransition;
	}

	private void OnLevelManagerTransition(GameManagerTransition transition)
	{
		
	}
}
