using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    // CardHolder should be owned by a duelist
    public class CardHolder : MonoBehaviour {
        protected List<Card> cards;
        public List<Card> Cards {
            get { return cards; }
        }

        protected virtual void Awake() {
            // Initialize any cards that should be children of the card holder
            Card[] childrenCards = GetComponentsInChildren<Card>(true);
            cards = new List<Card>(childrenCards ?? new Card[] { });
        }

        public virtual void Spread() {
            // Default spread: space out cards horizontally
            int offset = 2;
            int leftEdgeX = (1 - cards.Count) * offset / 2;
            for (int i = 0; i < cards.Count; i++) {
                Card card = cards[i];
                Vector3 pos = Vector3.zero;

                pos.x = leftEdgeX + (offset * i);
                card.transform.localPosition = pos;
            }
        }

        public bool TransferTo(CardHolder ch, Card card, bool isCardActive) {
            if (card == null) {
                return false;
            }

            Cards.Remove(card);
            ch.Cards.Add(card);
            card.transform.SetParent(ch.transform);
            card.gameObject.SetActive(isCardActive);
            return true;
        }
    }
}
