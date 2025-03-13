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
            cardStack.Enqueue((byte)(1 + (i % 8)));
        }
    }

    private Queue<byte> cardStack = new();

    public bool TryPeek(out byte value)
    {
        return cardStack.TryPeek(out value);
    }
    public void Push(byte value) //@Gandi hier bitte Karten in den Stack adden <3
    {
        cardStack.Enqueue(value);
        OnStackChanged?.Invoke();
    }
    public bool TryPop(out byte value)
    {
        bool result = cardStack.TryDequeue(out value);
        if(result)
        {
            OnStackChanged?.Invoke();
        }
        return result;
    }

    public int GetSize()
    {
        return cardStack.Count;
    }
}
