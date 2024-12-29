using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class Card : MonoBehaviour, CardGraphSelectable {
        [SerializeField]
        private CardSO cardSO;
        public CardSO CardSO => cardSO;

        public string CardName => CardSO.cardName;
        public string FlavorText => CardSO.flavorText;

        public int Cost => CardSO.cost;
        public int BaseIncome => CardSO.income;
        public int TurnLimit => CardSO.turnLimit;
        public List<Effect> Effects => CardSO.effects;

        public int ActiveTurns { get; private set; }
        public void IncrementActiveTurns() => ActiveTurns++;
        public bool ReachedTurnLimit() => ActiveTurns >= TurnLimit;

        // For applied effects per turn
        public List<ActiveEffectApplier> ActiveAppliedEffects { get; private set; }
        public void AddActiveAppliedEffect(Card source, Effect effect) => ActiveAppliedEffects.Add(new(source, effect));

        public List<Effect> SelfEffects { get; private set; }
        public List<Effect> NonSelfEffects { get; private set; }

        public int Income { get; private set; }
        public void UpdateIncome(int delta) => Income += delta;

        private SpriteRenderer sr;

        void Awake() {
            if (cardSO == null) {
                return;
            }

            sr = GetComponent<SpriteRenderer>();
            sr.sprite = cardSO.sprite;
            gameObject.SetActive(false);

            ActiveAppliedEffects = new();

            SelfEffects = Effects.Where(effect => effect.selfEffect).ToList();
            NonSelfEffects = Effects.Where(effect => !effect.selfEffect).ToList();

            Income = BaseIncome;
        }

        public void InitCardSO(CardSO cardSO) {
            this.cardSO = cardSO;
            Awake();
        }

        public CardHolder GetCurrentHolder() {
            // MINOR: Can probably optimize this a bit
            return GetComponentInParent<CardHolder>();
        }

        public void ApplyEffectTo(Card target, Effect effect) {
            if (effect.active) {
                target.AddActiveAppliedEffect(this, effect);
            } else {
                effect.Apply(this, target);
            }
        }

        public void ApplyActiveEffects() {
            int effectIndex = 0;
            while (effectIndex < ActiveAppliedEffects.Count) {
                // Apply current effect, increment index
                bool reachedEffectEnd = ActiveAppliedEffects[effectIndex++].ApplyEffect(this);

                // If ended, go to previous index (pre-decrement) and remove
                if (reachedEffectEnd) {
                    Debug.Log("Going to remove an effect now");
                    ActiveAppliedEffects.RemoveAt(--effectIndex);
                }
            }
        }
    }
}
