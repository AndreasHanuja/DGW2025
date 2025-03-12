using DG.Tweening;
using Stateless;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;


public class UILevelCompleteWindow : UICanvasGeneric
{
	//Settings
	[SerializeField]
	private float openWindowDelay = 2.5f;
	[SerializeField]
	private AudioClip objectiveCompletedAudioClip;


	//Functions
	public void Start()
	{
		GameManager.Instance.OnTransitioned += OnGameManagerTransition;
	}


	private void OnDestroy()
	{
		GameManager.Instance.OnTransitioned -= OnGameManagerTransition;
	}


	public void ReplayLevel()
	{
		GameManager.Instance.FireTrigger(GameManager.Trigger.RestartLevel);
	}


	public void ExitToMenu()
	{
		GameManager.Instance.FireTrigger(GameManager.Trigger.ExitLevel);
	}


	private void OnGameManagerTransition(GameManagerTransition transition)
	{
		if (transition.Destination == GameManager.State.GameOver)
		{
			DOVirtual.DelayedCall(openWindowDelay, () =>
			{
				OwnUICanvas.ParentGroup.Active = true;
				OwnUICanvas.SetActive(true);
			}, false);
		}
		else
		{
			OwnUICanvas.ParentGroup.Active = false;
		}
	}
}