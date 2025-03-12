using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;

public class GameManagerStateListener : MonoBehaviour
{
	[SerializeField]
	private GameManager.State state;

	[SerializeField]
	public UnityEvent OnStateEntered;
	[SerializeField]
	public UnityEvent OnStateExited;

	private bool isInState;

	public void Start()
	{
		isInState = GameManager.Instance.IsInState(state);
		if (isInState)
		{
			OnStateEntered.Invoke();
		}
		GameManager.Instance.OnTransitioned += OnGameManagerTransition;
	}


	private void OnDestroy()
	{
		GameManager.Instance.OnTransitioned -= OnGameManagerTransition;
	}


	private void OnGameManagerTransition(GameManagerTransition transition)
	{
		if (!isInState && GameManager.Instance.IsInState(state))
		{
			isInState = false;
			OnStateEntered.Invoke();
		}
		else if (isInState && !GameManager.Instance.IsInState(state))
		{
			isInState = false;
			OnStateExited.Invoke();
		}
	}
}
