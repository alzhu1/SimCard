using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public class Deck : CardHolder {
        private SpriteRenderer deckSr;

        public Card NextCard => cards.Count > 0 ? cards[0] : null;

        protected override void Awake() {
            base.Awake();
            deckSr = GetComponent<SpriteRenderer>();
        }

        public override void Spread() { }

        public void TryHideDeck() {
            if (cards.Count == 0) {
                deckSr.enabled = false;
            }
        }

        public void ClearDeck() {
            foreach (Card card in cards) {
                Destroy(card.gameObject);
            }
            cards.Clear();
        }

        public void InitFromCardMetadata(List<CardMetadata> initData, GameObject cardPrefab) {
            ClearDeck();

            foreach (CardMetadata cardMetadata in initData) {
                CardSO cardSO = cardMetadata.cardSO;
                int count = cardMetadata.count;

                for (int i = 0; i < count; i++) {
                    GameObject cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, transform);
                    Card card = cardObject.GetComponent<Card>();
                    card.InitCardSO(cardSO);
                    cards.Add(card);
                }
            }
        }
    }
}
