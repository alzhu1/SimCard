using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class LossEffect : Effect {
        [SerializeField] private int lossAmount;

        public override void Apply(Card source, Card target) {
            Debug.Log($"Loss effect, amount: {lossAmount}");
            target.UpdateIncome(-lossAmount);
        }
    }
}
