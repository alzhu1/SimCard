using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class Card : MonoBehaviour {
        [SerializeField]
        private CardSO cardSO;
        public CardSO CardSO => cardSO;

        public int Cost => CardSO.cost;
        public int Income => CardSO.income;
        public int TurnLimit => CardSO.turnLimit;

        public int ActiveTurns { get; private set; }
        public void IncrementActiveTurns() => ActiveTurns++;
        public bool ReachedTurnLimit() => ActiveTurns >= TurnLimit;

        public List<Effect> CardActiveEffects { get; private set; }
        public List<Effect> CardPassiveEffects { get; private set; }

        private SpriteRenderer sr;

        void Awake() {
            if (cardSO == null) {
                return;
            }

            sr = GetComponent<SpriteRenderer>();
            sr.sprite = cardSO.sprite;
            gameObject.SetActive(false);

            CardActiveEffects = new();
            CardPassiveEffects = new();
            foreach (Effect effect in CardSO.effects) {
                List<Effect> effectList = effect.active ? CardActiveEffects : CardPassiveEffects;
                effectList.Add(effect);
            }
        }

        public void InitCardSO(CardSO cardSO) {
            this.cardSO = cardSO;
            Awake();
        }

        public CardHolder GetCurrentHolder() {
            // TODO: Can probably optimize this a bit
            return GetComponentInParent<CardHolder>();
        }

        // TODO: Next is figuring out how to apply the card effects
        // Start with self-card effects; should be pretty simple.

        // First, card effects that trigger on the draw phase of the card owner.
        // This should be the simplest; basically treat Draw as an Upkeep step (we can rename it to UpkeepState or BeginState)
        // In this phase, we can loop through all cards on the field and apply the effect
        // Use this time to also figure out effects that change over time. Maybe need to copy the card effect to a new struct?

        // Or a special "timer struct" that takes the effect
        // Idea for the above: ApplyEffect could optionally take in a Timer object
        // And the effect behavior changes depending on the timer. This timer could be instantiated by something else
        // Or perhaps it's a default property of the Effect that can be changed in inspector if you want different behavior

        // Second, card effects that trigger on being played
        // This requires a new Play method on this Card class that will call ApplyEffect

        // We can ignore other instances of when card effects could occur (e.g. other phases, opponent phases)

        // After that, we should look at applied card effects.
        // Since we target other cards, this will likely require a new PlayerState to select the card that will be affected
        // For the effect application, maybe the Effect class could be kept in a list?
        // That could be used to keep track of the effects that a card currently has applied.
        // We can then refactor the first section to use this list; self-card effects can also utilize this too

        // Likely we would need a List<Effect> to keep track of continued effects (e.g. every upkeep phase)
        // For one-time effects, like upon playing a card, it's probably not necessary to keep track
    }
}
