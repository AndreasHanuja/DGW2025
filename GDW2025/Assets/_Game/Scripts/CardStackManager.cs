using System;
using System.Collections.Generic;

public class CardStackManager
{
    public static CardStackManager Instance = new CardStackManager();
    public Action OnStackChanged;
    private CardStackManager()
    {
        for(int i = 0; i < 50; i++)
        {
            cardStack.Push(1 + (i % 8));
        }
    }

    private Stack<int> cardStack = new();

    public bool TryPeek(out int value)
    {
        return cardStack.TryPeek(out value);
    }
    public void Push(int value) //@Gandi hier bitte Karten in den Stack adden <3
    {
        cardStack.Push(value);
        OnStackChanged?.Invoke();
    }
    public bool TryPop(out int value)
    {
        bool result = cardStack.TryPop(out value);
        if(result)
        {
            OnStackChanged?.Invoke();
        }
        return result;
    }
}
