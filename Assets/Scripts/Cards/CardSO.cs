using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType {
    Resource,
    Unit
}

[System.Serializable]
public struct ResourceCost {
    public ResourceSO resource;
    public int cost;
}

[System.Serializable]
public struct CardCost {
    public CardSO card;
    public int cost;
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card")]
public class CardSO : ScriptableObject {
    public string cardName;
    public int power;
    public Sprite sprite;
    public string flavorText;
    public string effectText;

    // TODO: This is just an idea but could change in future
    // May want to do some kind of "level" summon system like YGO
    public List<CardCost> cardCosts;
    public List<ResourceCost> resourceCosts;

    public virtual CardType CardType => CardType.Unit;
}
