using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CardHolder should be owned by a duelist
public class CardHolder : MonoBehaviour {
    protected List<Card> cards;
    public List<Card> Cards {
        get { return cards; }
    }

    protected Duelist duelist;

    protected void InitCardsFromChildren() {
        Card[] childrenCards = GetComponentsInChildren<Card>(true);
        if (childrenCards != null) {
            cards = new List<Card>(childrenCards);
        } else {
            cards = new List<Card>();
        }
    }

    protected void InitDuelist() {
        duelist = GetComponentInParent<Duelist>();
    }

    void Awake() {
        InitCardsFromChildren();
        InitDuelist();
    }
}
