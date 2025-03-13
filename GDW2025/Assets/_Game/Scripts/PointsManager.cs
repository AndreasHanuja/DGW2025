using UnityEngine;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;
using static Game.Map.MapInteractionManager;
using System.Collections.Generic;
using System.Linq;
using Game.Map.Models;
using System;

public class PointsManager : SingeltonMonoBehaviour<PointsManager>
{
	private int points = 0;
	public int Points { get => points; private set { points = value; PointsChanged?.Invoke(points); } }
	public event Action<int> PointsChanged;


	public void Start()
	{
		GameManager.Instance.OnTransitioned += OnGameManagerTransition;
	}

	private void OnDestroy()
	{
		GameManager.Instance.OnTransitioned -= OnGameManagerTransition;
	}

	private void OnGameManagerTransition(GameManagerTransition transition)
	{
		if (transition.Destination == GameManager.State.PlacingBuilding)
		{
			OnPlaciedBuilding((List<WFCResolvedChange>)transition.Parameters[0]);
		}
	}

	private void OnPlaciedBuilding(List<WFCResolvedChange> changes)
	{
		int lostPoints = GetPoints(changes.Select(change => change.oldValue).ToList());
		int gainedPoints = GetPoints(changes.Select(change => change.newValue).ToList());

		int pointDelta = gainedPoints - lostPoints;
		Points += pointDelta;
	}

	private int GetPoints(List<PlyModelSetup> buildings)
	{
		int points = 0;

		foreach (PlyModelSetup building in buildings)
		{
			if(building == null) continue;
			Points += building.PointValue;
		}

		return points;
	}
}
