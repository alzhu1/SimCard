using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType {
    Resource,
    Unit
}

[System.Serializable]
public struct Cost {
    public EntitySO entity;
    public int cost;
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card")]
public class CardSO : ScriptableObject {
    public string cardName;
    public int power;
    public Sprite sprite;
    public string flavorText;
    public string effectText;

    // Entity could represent global resources (e.g. food, fabrics)
    // Or it could represent unique units (so a unit has a CardSO + EntitySO)
    public EntitySO entity;

    // TODO: This is just an idea but could change in future
    // May want to do some kind of "level" summon system like YGO
    public List<Cost> costs;
}
