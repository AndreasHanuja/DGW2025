using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;

public class UILevelOverlay : UICanvasGeneric
{
	[SerializeField] private TMP_Text drawPileSize;
	[SerializeField] private TMP_Text pointsText;
	[SerializeField] private Image cardImage;
	[SerializeField] private Sprite[] images;
	[SerializeField] private int cardStackSizeWarningThreshold = 3;
	[SerializeField] private Color cardStackSizeWarningColor = Color.yellow;
	[SerializeField] private Color cardStackSizeLoosColor = Color.red;

	private Color cardStackSizeDefaultColor;

	private void Awake()
	{
		cardStackSizeDefaultColor = drawPileSize.color;
	}

	void Start()
	{
		GameManager.Instance.OnTransitioned += OnGameManagerTransition;
		PointsManager.Instance.PointsChanged += PointsChangedHandler;
		PointsChangedHandler(PointsManager.Instance.Points);
		CardStackManager.Instance.CardStackSizeChanged += CardStackSizeChangedHandler;
		CardStackSizeChangedHandler(CardStackManager.Instance.CardStackSize);
		CardStackManager.Instance.CurrentCardChanged += CurrentCardChangedHandler;
		CurrentCardChangedHandler(CardStackManager.Instance.CurrentCard);
	}

	private void OnGameManagerTransition(GameManagerTransition transition)
	{
		if (transition.Destination == GameManager.State.GameOver)
		{
			drawPileSize.color = cardStackSizeLoosColor;
		}
		else if (transition.Destination == GameManager.State.Starting)
		{
			drawPileSize.color = cardStackSizeDefaultColor;
		}
	}

	private void PointsChangedHandler(int points)
	{
		pointsText.text = $"{points}";
	}

	private void CardStackSizeChangedHandler(int cardStackSize)
	{
		drawPileSize.text = $"{cardStackSize}";

		if (cardStackSize <= cardStackSizeWarningThreshold)
		{
			drawPileSize.color = cardStackSizeWarningColor;
		}
		else
		{
			drawPileSize.color = cardStackSizeDefaultColor;
		}
	}

	private void CurrentCardChangedHandler(byte card)
	{
		cardImage.sprite = images[card];
	}
}
