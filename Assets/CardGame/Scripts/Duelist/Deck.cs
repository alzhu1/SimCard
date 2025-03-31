using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public class Deck : CardHolder, CardGraphSelectable {
        [SerializeField] private List<CardMetadata> baseDeck;

        public string PreviewName => "Deck";

        private SpriteRenderer deckSr;

        public Card NextCard => cards.Count > 0 ? cards[0] : null;

        protected override void Awake() {
            base.Awake();
            deckSr = GetComponent<SpriteRenderer>();
        }

        public override void Organize() {
            foreach (Card card in cards) {
                card.transform.position = transform.position;
                card.Hidden = hidden;
            }
        }

        public void ClearDeck() {
            foreach (Card card in cards) {
                Destroy(card.gameObject);
            }
            cards.Clear();
        }

        public void InitBaseDeck(CardGameManager cardGameManager) {
            InitFromCardMetadata(baseDeck, cardGameManager);
        }

        public void InitFromCardMetadata(List<CardMetadata> initData, CardGameManager cardGameManager) {
            ClearDeck();

            List<CardSO> cards = initData.SelectMany(cardMetadata => Enumerable.Repeat(cardMetadata.cardSO, cardMetadata.count)).ToList();
            while (cards.Count > 0) {
                // Shuffle the deck by randomly selecting a card
                int randomIndex = Random.Range(0, cards.Count);
                CardSO cardSO = cards[randomIndex];
                cards.RemoveAt(randomIndex);

                // Init pooled card
                Card cardFromPool = cardGameManager.CardPool.GetPooledCard();
                cardFromPool.InitCardSO(cardSO);
                cardGameManager.CardPool.TransferTo(this, cardFromPool, true);
            }
        }
    }
}
