using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class DrawState<T> : DuelistState where T : DuelistState, new() {
        protected override void Enter() { }

        protected override void Exit() { }

        protected override IEnumerator Handle() {
            duelist.DrawCard();
            nextState = new T();

            foreach (Card card in duelist.Field.Cards) {
                int preCurrency = duelist.Currency;
                duelist.AdjustCurrency(card.Income);
                Debug.Log($"Currency ({preCurrency}) + income ({card.Income}) = {duelist.Currency}");
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
