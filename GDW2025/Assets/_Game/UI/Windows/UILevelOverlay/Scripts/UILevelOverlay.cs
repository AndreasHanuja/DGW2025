using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameManagerTransition = Stateless.StateMachine<GameManager.State, GameManager.Trigger>.Transition;

public class UILevelOverlay : UICanvasGeneric
{
	[SerializeField] private TMP_Text drawPileSize;
	[SerializeField] private TMP_Text pointsText;
	[SerializeField] private Image cardImage;
	[SerializeField] private Image cardImageBack;
	[SerializeField] private Image cardStackBarFillImage;
	[SerializeField] private GameObject gameOverPannel;
	[SerializeField] private Sprite[] images;
	[SerializeField] private Sprite imagesBackMedival;
	[SerializeField] private Sprite imagesBackSolar;
	[SerializeField] private Sprite imagesBackFantasy;
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
		gameOverPannel.SetActive(false);
		GameManager.Instance.OnTransitioned += OnGameManagerTransition;
		PointsManager.Instance.PointsChanged += PointsChangedHandler;
		PointsChangedHandler(PointsManager.Instance.Points);
		CardStackManager.Instance.CardStackSizeChanged += CardStackSizeChangedHandler;
		CardStackSizeChangedHandler(CardStackManager.Instance.CardStackSize);
		CardStackManager.Instance.CurrentCardChanged += CurrentCardChangedHandler;
		CurrentCardChangedHandler(CardStackManager.Instance.CurrentCard);
		CardStackManager.Instance.AddedCardToStack += PunchCardStackSizeText;
	}

	public void Restert()
	{
		SceneManager.LoadScene(0);
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

		gameOverPannel.SetActive(transition.Destination == GameManager.State.GameOver);
	}

	private void PointsChangedHandler(int points)
	{
		pointsText.text = $"{points}";
		UpdateBar();
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
		if (card == 6)
		{
			cardImageBack.sprite = imagesBackSolar;
		}
		else if (card == 7)
		{
			cardImageBack.sprite = imagesBackFantasy;
		}
		else
		{
			cardImageBack.sprite = imagesBackMedival;
		}
		UpdateBar();
	}

	private void PunchCardStackSizeText()
	{
		drawPileSize.transform.DOKill(true);
		drawPileSize.transform.DOPunchScale(Vector3.one * 1.0f, 1.0f, 0, 0.0f);
	}

	private void UpdateBar()
	{
		float fill = CardStackManager.Instance.NextCardPercentage;
		cardStackBarFillImage.transform.localScale = new Vector3(fill, 1.0f, 1.0f);
	}
}
