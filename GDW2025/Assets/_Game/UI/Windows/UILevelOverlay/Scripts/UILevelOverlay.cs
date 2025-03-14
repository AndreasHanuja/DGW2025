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


	void Start()
	{
		PointsManager.Instance.PointsChanged += PointsChangedHandler;
		PointsChangedHandler(PointsManager.Instance.Points);
		CardStackManager.Instance.CardStackSizeChanged += CardStackSizeChangedHandler;
		CardStackSizeChangedHandler(CardStackManager.Instance.CardStackSize);
		CardStackManager.Instance.CurrentCardChanged += CurrentCardChangedHandler;
		CurrentCardChangedHandler(CardStackManager.Instance.CurrentCard);
	}

	private void PointsChangedHandler(int points)
	{
		pointsText.text = $"{points}";
	}

	private void CardStackSizeChangedHandler(int cardStackSize)
	{
		drawPileSize.text = $"{cardStackSize}";
	}

	private void CurrentCardChangedHandler(byte card)
	{
		cardImage.sprite = images[card];
	}
}
