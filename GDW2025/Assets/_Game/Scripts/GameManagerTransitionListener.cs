using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;

public class GameManagerTransitionListener : MonoBehaviour
{
	[SerializeField]
	private GameManager.State[] sourceFilters;
	[SerializeField]
	private GameManager.State[] destinationFilters;
	[SerializeField]
	private GameManager.Trigger[] triggerFilters;

	[SerializeField]
	public UnityEvent OnTransitioned;

	public void OnEnable()
	{
		GameManager.Instance.OnTransitioned += OnGameManagerTransition;
	}


	private void OnDisable()
	{
		GameManager.Instance.OnTransitioned -= OnGameManagerTransition;
	}


	private void OnGameManagerTransition(GameManagerTransition transition)
	{
		if (sourceFilters.Length > 0 && !sourceFilters.Contains(transition.Source))
		{
			return;
		}
		if (destinationFilters.Length > 0 && !destinationFilters.Contains(transition.Destination))
		{
			return;
		}
		if (triggerFilters.Length > 0 && !triggerFilters.Contains(transition.Trigger))
		{
			return;
		}

		OnTransitioned.Invoke();
	}
}
