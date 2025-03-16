using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimCard.CardGame {
    public class CardPool : CardHolder {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private int cardPoolSize = 100;

        protected override void Awake() {
            base.Awake();
            for (int i = 0; i < cards.Count - cardPoolSize; i++) {
                cards.Add(InitCard());
            }
        }

        public Card GetPooledCard() {
            if (cards.Count == 0) {
                Debug.LogWarning("Ran out of pooled cards, add a new one");
                cards.Add(InitCard());
            }
            return cards.Last();
        }

        Card InitCard() {
            GameObject card = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, transform);
            card.SetActive(false);
            return card.GetComponent<Card>();
        }
    }
}
