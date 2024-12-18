using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class GainEffect : Effect {
        [SerializeField] private int gainAmount;

        public override void Apply(Card source, Card target) {
            Debug.Log($"Gain effect, amount: {gainAmount}");
            target.UpdateIncome(gainAmount);
        }
    }
}
