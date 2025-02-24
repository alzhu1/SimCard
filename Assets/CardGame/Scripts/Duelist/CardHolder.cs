using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    // CardHolder should be owned by a duelist
    public class CardHolder : MonoBehaviour {
        [SerializeField] protected float spreadOffset = 1.5f;
        [SerializeField] protected bool hidden;

        protected List<Card> cards;
        public List<Card> Cards {
            get { return cards; }
        }

        protected virtual void Awake() {
            // Initialize any cards that should be children of the card holder
            Card[] childrenCards = GetComponentsInChildren<Card>(true);
            cards = new List<Card>(childrenCards ?? new Card[] { });
        }

        public virtual void Organize() {
            // Default spread: space out cards horizontally
            float leftEdgeX = (1 - cards.Count) * spreadOffset / 2;
            for (int i = 0; i < cards.Count; i++) {
                Card card = cards[i];
                Vector3 pos = Vector3.zero;

                pos.x = leftEdgeX + (spreadOffset * i);
                card.transform.localPosition = pos;
            }
        }

        public bool TransferTo(CardHolder ch, Card card, bool isCardActive) {
            if (card == null) {
                return false;
            }

            // Move card locations
            Cards.Remove(card);
            ch.Cards.Add(card);
            card.transform.SetParent(ch.transform);

            // Update activity/hidden status based on target holder
            card.Hidden = ch.hidden;
            card.gameObject.SetActive(isCardActive);
            return true;
        }
    }
}
