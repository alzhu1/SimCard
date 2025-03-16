using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class AdjustIncomeEffect : Effect {
        [SerializeField] private int delta;

        public override void Apply(Card source, Card target) {
            Debug.Log($"Change income of card effect, amount: {delta}");
            target.UpdateIncome(delta);
        }
    }
}
