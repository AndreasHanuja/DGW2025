using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Game.Map.MapInteractionManager;

public class GameManager : SingeltonMonoBehaviour<GameManager>
{
	public enum State
	{
		MainMenu,
		Starting,
		DrawingBuilding,
		SelectingBuildingPlacement,
		PlacingBuilding,
		GameOver
	}

	public enum Trigger
	{
		EnterLevel,
		ExitLevel,
		StartingCompleted,
		DrawBuildingCompleted,
		PlaceBuilding,
		PlacingBuildingCompleted,
		RestartLevel
	}

	[SerializeField]
	private bool displayState = false;

	private StateMachine<State, Trigger> stateMachiene;
	private StateMachine<State, Trigger>.TriggerWithParameters<List<WFCResolvedChange>> placeBuildingTrigger;
	private bool CanDrawBuilding => CardStackManager.Instance.CanDrawBuilding;
	public event Action<StateMachine<State, Trigger>.Transition> OnTransitioned;

	protected override void Awake()
	{
		base.Awake();
		InitializeStateMachiene();
	}

	private void InitializeStateMachiene()
	{
		stateMachiene = new StateMachine<State, Trigger>(State.MainMenu);

		//create triggers with parameters
		placeBuildingTrigger = stateMachiene.SetTriggerParameters<List<WFCResolvedChange>>(Trigger.PlaceBuilding);

		//confugure states
		stateMachiene.Configure(State.MainMenu)
			.Permit(Trigger.EnterLevel, State.Starting);

		stateMachiene.Configure(State.Starting)
			.Permit(Trigger.StartingCompleted, State.DrawingBuilding)
			.OnEntry(() => FireTrigger(Trigger.StartingCompleted));

		stateMachiene.Configure(State.DrawingBuilding)
			.Permit(Trigger.DrawBuildingCompleted, State.SelectingBuildingPlacement);

		stateMachiene.Configure(State.SelectingBuildingPlacement)
			.Permit(Trigger.PlaceBuilding, State.PlacingBuilding);

		stateMachiene.Configure(State.PlacingBuilding)
			.PermitIf(Trigger.PlacingBuildingCompleted, State.DrawingBuilding, () => CanDrawBuilding)
			.PermitIf(Trigger.PlacingBuildingCompleted, State.GameOver, () => !CanDrawBuilding)
            .OnEntry(() => FireTrigger(Trigger.PlacingBuildingCompleted));

        stateMachiene.Configure(State.GameOver)
			.Permit(Trigger.RestartLevel, State.Starting);

		stateMachiene.OnTransitioned((transition) => OnTransitioned?.Invoke(transition));

		//Debug.Log(UmlDotGraph.Format(stateMachiene.GetInfo()));
	}


	private void OnGUI()
	{
		if (!displayState)
		{
			return;
		}

		GUIStyle guiStyle = new GUIStyle();
		guiStyle.fontSize = 20;
		guiStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(10, 10, 500, 30), $"GameManager: {stateMachiene.State}", guiStyle);
	}

	public void FireTrigger(Trigger trigger)
	{
		stateMachiene.Fire(trigger);
	}

	public void PlacedBuilding(List<WFCResolvedChange> changes)
	{
		stateMachiene.Fire(placeBuildingTrigger, changes);
	}

	public bool IsInState(State state)
	{
		return stateMachiene.IsInState(state);
	}
}
