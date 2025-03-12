using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;

public class UILevelMenu : UICanvasGeneric
{
	//Functions
	public void Start()
	{
		GameManager.Instance.OnTransitioned += OnGameManagerTransitioned;
	}

	private void OnDestroy()
	{
		GameManager.Instance.OnTransitioned -= OnGameManagerTransitioned;
	}

	public void ResumePlaying()
	{
		//GameManager.Instance.FireTrigger(GameManager.Trigger.Resume);
	}

	public void ExitToMenu()
	{
		GameManager.Instance.FireTrigger(GameManager.Trigger.ExitLevel);
	}

	private void OnGameManagerTransitioned(GameManagerTransition transition)
	{
		/*
		if (transition.Trigger == GameManager.Trigger.Pause)
		{
			OwnUICanvas.SetActiveRecoursively();
			SetLevelName(GameManager.Instance.CurrentLevel.LevelName);
		}
		else if (transition.Trigger == GameManager.Trigger.Resume)
		{
			OwnUICanvas.ParentGroup.SetActive(false);
		}
		*/
	}
}