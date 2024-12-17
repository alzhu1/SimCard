using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;
using ResourceEntitySO = SimCard.Common.ResourceEntitySO;

namespace SimCard.CardGame {
    public abstract class Duelist : MonoBehaviour {
        private DuelistState duelistState;

        public CardGameManager CardGameManager { get; private set; }

        public Hand Hand { get; private set; }
        public Deck Deck { get; private set; }
        public Field Field { get; private set; }
        public Graveyard Graveyard { get; private set; }

        public Dictionary<ResourceEntitySO, int> CurrentResources { get; private set; }

        public int Currency { get; private set; }

        public int TurnActions { get; private set; }

        void Awake() {
            // Should be provided by parent
            CardGameManager = GetComponentInParent<CardGameManager>();

            // Get different duelist aspects in children objects
            Hand = GetComponentInChildren<Hand>();
            Deck = GetComponentInChildren<Deck>();
            Field = GetComponentInChildren<Field>();
            Graveyard = GetComponentInChildren<Graveyard>();

            CurrentResources = new Dictionary<ResourceEntitySO, int>();

            Currency = 50;
        }

        void Start() {
            CardGameManager.EventBus.OnGameStart.Event += InitForGame;
            CardGameManager.EventBus.OnTurnStart.Event += StartTurn;
        }

        void OnDestroy() {
            CardGameManager.EventBus.OnGameStart.Event -= InitForGame;
            CardGameManager.EventBus.OnTurnStart.Event -= StartTurn;
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

        protected abstract void InitForGame(EventArgs _args);
        protected abstract DuelistState StartState { get; }

        public bool AllowAction => TurnActions > 0;

        // TODO: Thinking that for these card operations, they should be co-routines so that other states can wait on them

        public void DrawCard() {
            if (Deck.TransferTo(Hand, Deck.NextCard, true)) {
                Deck.TryHideDeck();
            }

            OrganizeArea();
        }

        public void PlaySelectedCard(Card card) {
            TurnActions--;
            Currency -= card.Cost;

            Hand.TransferTo(Field, card, true);

            OrganizeArea();

            foreach (Effect passiveEffect in card.CardPassiveEffects) {
                if (passiveEffect.selfEffect) {
                    passiveEffect.ApplyEffect(card, card);
                } else {
                    // TODO: Fix this (need a better way to trigger "other" effects)
                }
            }
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

        public void AdjustCurrency(int delta) {
            Currency += delta;
        }
    }
}
