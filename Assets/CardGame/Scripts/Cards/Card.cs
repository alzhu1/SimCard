using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class Card : MonoBehaviour {
        [SerializeField]
        private CardSO cardSO;
        public CardSO CardSO => cardSO;

        public int Cost => CardSO.cost;
        public int BaseIncome => CardSO.income;
        public int TurnLimit => CardSO.turnLimit;
        public List<Effect> Effects => CardSO.effects;

        public int ActiveTurns { get; private set; }
        public void IncrementActiveTurns() => ActiveTurns++;
        public bool ReachedTurnLimit() => ActiveTurns >= TurnLimit;

        // For applied effects per turn
        public List<EffectApplier> ActiveAppliedEffects { get; private set; }
        public void AddActiveAppliedEffect(Card source, Effect effect) => ActiveAppliedEffects.Add(new(source, effect));

        public List<Effect> SelfEffects { get; private set; }
        public List<Effect> NonSelfEffects { get; private set; }

        public int Income { get; private set; }

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
            // TODO: Can probably optimize this a bit
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
            foreach (EffectApplier effectApplier in ActiveAppliedEffects) {
                effectApplier.ApplyEffect(this);
            }
        }

        // TODO: Need a better way to update income
        public void UpdateIncome(int delta) {
            Income += delta;
        }

        // TODO: Next is figuring out how to apply the card effects

        // Need to figure out effects that change over time. Maybe need to copy the card effect to a new struct?
        // Or a special "timer struct" that takes the effect
        // Idea for the above: Apply could optionally take in a Timer object
        // And the effect behavior changes depending on the timer. This timer could be instantiated by something else
        // Or perhaps it's a default property of the Effect that can be changed in inspector if you want different behavior

        // Notes on card effects applied to a non-self card:
        // Since we target other cards, this will likely require a new PlayerState to select the card that will be affected
        // When the effect is to be applied, pause the PlaySelectedCard coroutine (side note: make it a coroutine)
        // Also, transfer the state to be in card selection for effect
        // Will need a way to consistently go back to this state, to apply multiple non-self card effects
    }
}
