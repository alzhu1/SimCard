using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/ResoruceCard")]
public class ResourceCardSO : CardSO {
    public ResourceSO resource;

    public new CardType CardType => CardType.Resource;
}
