using TMPro;
using UnityEngine;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text drawPileSize;
    [SerializeField] private TMP_Text currentCardType;

    void Start()
    {
        CardStackManager.Instance.CardStackSizeChanged += CardStackSizeChangedHandler;
        CardStackManager.Instance.CurrentCardChanged += CurrentCardChangedHandler;
    }

    private void CardStackSizeChangedHandler(int cardStackSize)
    {
        drawPileSize.text = $"+{cardStackSize}";
	}

    private void CurrentCardChangedHandler(byte card)
	{
		currentCardType.text = card.ToString();
	}
}
