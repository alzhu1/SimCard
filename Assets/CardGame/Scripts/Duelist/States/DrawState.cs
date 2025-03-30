using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class DrawState<T> : DuelistState where T : DuelistState, new() {
        protected override void Enter() { }

        protected override void Exit() { }

        protected override IEnumerator Handle() {
            yield return duelist.StartCoroutine(Upkeep());

            duelist.DrawCard();
            nextState = new T();
        }

        IEnumerator Upkeep() {
            List<Card> cardsToDiscard = new();

            foreach (Card card in duelist.Field.Cards) {
                duelist.CardGameManager.PlayCardProcessSound();

                card.SetHighlight();

                // Apply active effects
                card.ApplyActiveEffects();

                // Check for currency
                int preCurrency = duelist.Currency;
                Debug.Log($"Currency ({preCurrency}) + income ({card.Income}) = {duelist.Currency}");
                duelist.AdjustCurrency(card.Income);

                // We can update card active turns at upkeep
                card.DecrementActiveTurns();

                // Check for turn limit reached
                if (card.ReachedTurnLimit()) {
                    cardsToDiscard.Add(card);
                    Debug.Log($"Adding {card} to discard list");
                }

                yield return new WaitForSeconds(0.5f);
                card.UnsetHighlight();
            }

            // Keep separate, can't modify the card holder itself during loop
            foreach (Card card in cardsToDiscard) {
                duelist.Discard(card);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
