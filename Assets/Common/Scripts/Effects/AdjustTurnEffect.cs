using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class AdjustTurnEffect : Effect {
        [SerializeField] private int delta;

        public override void Apply(Card source, Card target) {
            Debug.Log($"Change active turns of card effect, amount: {delta}");
            target.UpdateActiveTurns(delta);
        }
    }
}
