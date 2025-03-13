using TMPro;
using UnityEngine;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text drawPileSize;
    [SerializeField] private TMP_Text currentCardType;

    void Start()
    {
        CardStackManager.Instance.OnStackChanged += StackChangedHandler;
        StackChangedHandler();
    }

    private void StackChangedHandler()
    {
        drawPileSize.text = "+"+CardStackManager.Instance.GetSize();
        CardStackManager.Instance.TryPeek(out byte card);
        currentCardType.text = card.ToString();
    }
}
