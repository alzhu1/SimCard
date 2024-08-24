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

    void Awake() {
        Init();
    }

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

    protected void Init() {
        InitCardsFromChildren();
        InitDuelist();
    }

    public void TransferTo(CardHolder ch, Card card, bool isCardActive) {
        Cards.Remove(card);
        ch.Cards.Add(card);
        card.transform.SetParent(ch.transform);
        card.gameObject.SetActive(isCardActive);
    }
}
