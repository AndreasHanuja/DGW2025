using Stateless;
using Stateless.Graph;
using System;
using UnityEngine;

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

	private StateMachine<State, Trigger> stateMachiene;
	private bool CanDrawBuilding => true;
	public event Action<StateMachine<State, Trigger>.Transition> OnTransitioned;

	protected override void Awake()
	{
		base.Awake();
		InitializeStateMachiene();
	}

	private void InitializeStateMachiene()
	{
		stateMachiene = new StateMachine<State, Trigger>(State.MainMenu);

		stateMachiene.Configure(State.MainMenu)
			.Permit(Trigger.EnterLevel, State.Starting);

		stateMachiene.Configure(State.Starting)
			.Permit(Trigger.StartingCompleted, State.DrawingBuilding);

		stateMachiene.Configure(State.DrawingBuilding)
			.Permit(Trigger.DrawBuildingCompleted, State.SelectingBuildingPlacement);

		stateMachiene.Configure(State.SelectingBuildingPlacement)
			.Permit(Trigger.PlaceBuilding, State.PlacingBuilding);

		stateMachiene.Configure(State.PlacingBuilding)
			.PermitIf(Trigger.PlacingBuildingCompleted, State.DrawingBuilding, () => CanDrawBuilding)
			.PermitIf(Trigger.PlacingBuildingCompleted, State.GameOver, () => !CanDrawBuilding);

		stateMachiene.Configure(State.GameOver)
			.Permit(Trigger.RestartLevel, State.Starting);

		stateMachiene.OnTransitioned((transition) => OnTransitioned?.Invoke(transition));

		//Debug.Log(UmlDotGraph.Format(stateMachiene.GetInfo()));
	}

	public void FireTrigger(Trigger trigger)
	{
		stateMachiene.Fire(trigger);
	}

	public bool IsInState(State state)
	{
		return stateMachiene.IsInState(state);
	}
}
