using System.Collections.Generic;
using UnityEngine;
using static Game.Map.MapInteractionManager;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;

public class CardStackManager : SingeltonMonoBehaviour<CardStackManager>
{
	[SerializeField]
	private int startCardDeckSize = 5;

	private byte currentCard = 0;
	public byte CurrentCard { get => currentCard; private set { currentCard = value; CurrentCardChanged?.Invoke(currentCard); } }
	private int cardStackSize = 0;
	public int CardStackSize { get => cardStackSize; private set { cardStackSize = value; CardStackSizeChanged?.Invoke(cardStackSize); } }
	private int nextCardPointTpreshold = 0;
	private int totalAddedCards = 0;

	public bool CanDrawBuilding => CardStackSize > 0;
	public event System.Action<int> CardStackSizeChanged;
	public event System.Action<byte> CurrentCardChanged;

	private void Start()
	{
		GameManager.Instance.OnTransitioned += OnGameManagerTransition;
		PointsManager.Instance.PointsChanged += AddCardWhenThresholdReched;
	}

	private void OnDestroy()
	{
		GameManager.Instance.OnTransitioned -= OnGameManagerTransition;
		PointsManager.Instance.PointsChanged -= AddCardWhenThresholdReched;
	}

	private void OnGameManagerTransition(GameManagerTransition transition)
	{
		if (transition.Destination == GameManager.State.Starting)
		{
			CurrentCard = 0;
			CardStackSize = startCardDeckSize;
			totalAddedCards = 0;
			nextCardPointTpreshold = CardThresholdFunction(totalAddedCards);
		}

		if (transition.Destination == GameManager.State.DrawingBuilding)
		{
			DrawCard();
			GameManager.Instance.FireTrigger(GameManager.Trigger.DrawBuildingCompleted);
		}
	}

	private void DrawCard()
	{
		if (CardStackSize <= 0)
		{
			throw new System.InvalidOperationException("User hat ne Karte gezogen, soll er aber nicht wenns keine gibt!");
		}

		CardStackSize--;
		CurrentCard = (byte)Random.Range(1, 9);
	}

	public bool TryPeek(out byte value)
    {
		value = CurrentCard;
        return CurrentCard > 0;
    }

    public bool TryPop(out byte value)
    {
		value = CurrentCard;
		CurrentCard = 0;
		return CurrentCard > 0;
	}

	private void AddCardWhenThresholdReched(int points)
	{
		if (points < nextCardPointTpreshold)
		{
			return;
		}

		CardStackSize++;
		totalAddedCards++;
		nextCardPointTpreshold = CardThresholdFunction(totalAddedCards);
	}

	private int CardThresholdFunction(int i)
	{
		return (i * i) + 10;
	}
}
