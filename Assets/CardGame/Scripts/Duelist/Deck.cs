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

        public void InitFromCardMetadata(List<CardMetadata> initData, CardGameManager cardGameManager) {
            ClearDeck();

            foreach (CardMetadata cardMetadata in initData) {
                CardSOV2 cardSO = cardMetadata.cardSOV2;
                int count = cardMetadata.count;

                for (int i = 0; i < count; i++) {
                    Card cardFromPool = cardGameManager.CardPool.GetPooledCard();
                    cardFromPool.InitCardSO(cardSO);
                    cardGameManager.CardPool.TransferTo(this, cardFromPool, false);
                }
            }
        }
    }
}
