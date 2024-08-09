using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    public Sprite sprite;

    private Duelist duelist;
    private List<Card> cards;

    void Awake() {
        duelist = GetComponentInParent<Duelist>();
        cards = new List<Card>();
    }

    public void AddCard(Card card) {
        cards.Add(card);

        card.transform.SetParent(transform);

        card.gameObject.SetActive(true);
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(-6, 6);
        pos.y = Random.Range(-5, 5);
        card.transform.localPosition = pos;
    }

    public void RemoveCard(int index) {
        Destroy(cards[index].gameObject);
        cards.RemoveAt(index);
    }

    void Start() {
        
    }

    void Update() {
        
    }
}
