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
            for (int i = 0; i < cardPoolSize; i++) {
                GameObject card = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, transform);
                card.SetActive(false);
                cards.Add(card.GetComponent<Card>());
            }
        }

        public Card GetPooledCard() {
            return cards.Last();
        }
    }
}
