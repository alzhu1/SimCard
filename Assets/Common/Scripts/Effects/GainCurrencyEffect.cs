using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class GainCurrencyEffect : Effect {
        [SerializeField] private int delta;

        public override void Apply(Card source, Card target) {
            Debug.Log($"Get money: {delta}");
            target.GetCurrentHolder().HoldingDuelist.AdjustCurrency(delta);
        }
    }
}
