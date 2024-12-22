using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class Duelist : MonoBehaviour {
        // TODO: Would prefer not to serialize if possible...
        [SerializeField] private Duelist enemy;
        public Duelist Enemy => enemy;

        private DuelistState duelistState;

        public CardGameManager CardGameManager { get; private set; }

        public Hand Hand { get; private set; }
        public Deck Deck { get; private set; }
        public Field Field { get; private set; }
        public Graveyard Graveyard { get; private set; }

        public int Currency { get; private set; }
        public void AdjustCurrency(int delta) {
            Currency += delta;

            // Handle win/loss
            if (Currency < 0) {
                // Lose
                CardGameManager.EventBus.OnGameEnd.Raise(new(Enemy, this));
            }

            if (Currency > 200) {
                // Win
                CardGameManager.EventBus.OnGameEnd.Raise(new(this, Enemy));
            }
        }

        public int Taxes { get; private set; }
        public void AdjustTaxes(int delta) => Taxes += delta;

        public int TurnActions { get; private set; }
        public bool IsActionAllowed() => TurnActions > 0;

        protected int FirstDrawAmount => 4;

        void Awake() {
            // Should be provided by parent
            CardGameManager = GetComponentInParent<CardGameManager>();

            // Get different duelist aspects in children objects
            Hand = GetComponentInChildren<Hand>();
            Deck = GetComponentInChildren<Deck>();
            Field = GetComponentInChildren<Field>();
            Graveyard = GetComponentInChildren<Graveyard>();

            Currency = 50;
            Taxes = 0;
        }

        void Start() {
            CardGameManager.EventBus.OnGameStart.Event += InitForGame;
            CardGameManager.EventBus.OnTurnStart.Event += StartTurn;
            CardGameManager.EventBus.OnGameEnd.Event += StopDuelist;
        }

        void OnDestroy() {
            CardGameManager.EventBus.OnGameStart.Event -= InitForGame;
            CardGameManager.EventBus.OnTurnStart.Event -= StartTurn;
            CardGameManager.EventBus.OnGameEnd.Event -= StopDuelist;
        }

        void Update() {
            if (duelistState != null) {
                DuelistState nextState = duelistState.NextState;

                if (nextState != null) {
                    duelistState = nextState;
                    duelistState.Init(this);
                    duelistState.Begin();
                }
            }
        }

        void StartTurn(EventArgs<Duelist> args) {
            if (args.argument != this) {
                return;
            }

            TurnActions = 2;
            duelistState = StartState;
            duelistState.Init(this);
            duelistState.Begin();
        }

        public void EndTurn() {
            duelistState = null;
            CardGameManager.EndTurn();
        }

        void StopDuelist(EventArgs<Duelist, Duelist> _args) {
            Debug.Log("Stop the duelist");
            duelistState?.Stop();
            duelistState = null;
        }

        protected abstract void InitForGame(InitCardGameArgs args);
        protected abstract DuelistState StartState { get; }

        // TODO: Thinking that for these card operations, they should be co-routines so that other states can wait on them

        public void DrawCard() {
            if (Deck.TransferTo(Hand, Deck.NextCard, true)) {
                Deck.TryHideDeck();
            }

            OrganizeArea();
        }

        public void PlaySelectedCard(Card card) {
            TurnActions--;
            AdjustCurrency(-card.Cost);

            Hand.TransferTo(Field, card, true);

            OrganizeArea();

            foreach (Effect effect in card.SelfEffects) {
                ApplyCardEffect(effect, card, card);
            }
        }

        public void ApplyCardEffect(Effect effect, Card source, Card target) {
            source.ApplyEffectTo(target, effect);
        }

        public void Discard(Card card) {
            CardHolder currentCardHolder = card.GetCurrentHolder();
            currentCardHolder.TransferTo(Graveyard, card, false);
        }

        void OrganizeArea() {
            // This is called whenever a card based operation occurs
            Field.Spread();
            Hand.Spread();
        }
    }
}
