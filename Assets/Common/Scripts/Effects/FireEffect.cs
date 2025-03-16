using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class FireEffect : Effect {
        public override void Apply(Card source, Card target) {
            target.GetCurrentHolder().HoldingDuelist.FireCard(target);
        }
    }
}
