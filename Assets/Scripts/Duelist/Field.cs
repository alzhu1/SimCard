using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : CardHolder {
    public void AddCard(Card card) {
        cards.Add(card);
        card.transform.SetParent(transform);
        card.gameObject.SetActive(true);
    }

    public void RemoveCard(int index) {
        Destroy(cards[index].gameObject);
        cards.RemoveAt(index);
    }

    public void SpreadField() {
        int offset = 2;
        int leftEdgeX = (1 - cards.Count) * offset / 2;
        for (int i = 0; i < cards.Count; i++) {
            Card card = cards[i];
            Vector3 pos = Vector3.zero;

            pos.x = leftEdgeX + (offset * i);
            card.transform.localPosition = pos;
        }
    }
}
