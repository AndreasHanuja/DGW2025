using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text drawPileSize;
    [SerializeField] private Image cardImage;
    [SerializeField] private Sprite[] images;


    void Start()
    {
        CardStackManager.Instance.CardStackSizeChanged += CardStackSizeChangedHandler;
        CardStackManager.Instance.CurrentCardChanged += CurrentCardChangedHandler;
        CardStackManager.Instance.TryPeek(out byte value);
        CurrentCardChangedHandler(value);
    }

    private void CardStackSizeChangedHandler(int cardStackSize)
    {
        drawPileSize.text = $"+{cardStackSize}";
	}

    private void CurrentCardChangedHandler(byte card)
	{
        cardImage.sprite = images[card];
    }
}
