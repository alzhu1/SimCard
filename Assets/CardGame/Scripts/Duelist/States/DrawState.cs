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
            Debug.Log($"Paying taxes of value {duelist.Taxes}");
            duelist.AdjustCurrency(-duelist.Taxes);

            List<Card> cardsToDiscard = new();

            foreach (Card card in duelist.Field.Cards) {
                // We can increment card active turns at upkeep
                card.DecrementActiveTurns();

                // Check for turn limit reached
                if (card.ReachedTurnLimit()) {
                    cardsToDiscard.Add(card);
                    continue;
                }

                // Apply active effects
                card.ApplyActiveEffects();

                // Check for currency
                int preCurrency = duelist.Currency;
                duelist.AdjustCurrency(card.Income);
                Debug.Log($"Currency ({preCurrency}) + income ({card.Income}) = {duelist.Currency}");
                yield return new WaitForSeconds(0.5f);
            }

            // Keep separate, can't modify the card holder itself during loop
            foreach (Card card in cardsToDiscard) {
                duelist.Discard(card);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
