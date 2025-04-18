using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class Duelist : MonoBehaviour {
        public static readonly int MAX_FIELD_CARDS = 5;
        public static readonly int MAX_HAND_CARDS = 6;

        public Duelist Enemy { get; private set; }
        public void SetEnemy(Duelist enemy) => Enemy = enemy;

        private DuelistState duelistState;

        public CardGameManager CardGameManager { get; private set; }

        public Hand Hand { get; private set; }
        public Deck Deck { get; private set; }
        public Field Field { get; private set; }
        public Graveyard Graveyard { get; private set; }

        // For debugging
        [field:SerializeField]
        public int Currency { get; private set; }
        public bool AdjustCurrency(int delta) {
            bool gameEnded = false;
            int beforeCurrency = Currency;
            Currency += delta;

            // Handle win/loss
            if (Currency < 0) {
                // Lose
                CardGameManager.EventBus.OnGameEnd.Raise(new(Enemy, this, CardGameEndReason.CurrencyGoalReached));
                gameEnded = true;
            }

            if (Currency >= CardGameManager.WinCurrency) {
                // Win
                CardGameManager.EventBus.OnGameEnd.Raise(new(this, Enemy, CardGameEndReason.CurrencyGoalReached));
                gameEnded = true;
            }

            // Send corresponding event to update UI
            SendCurrencyUpdateEvent(beforeCurrency, Currency);
            return gameEnded;
        }

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
            CardGameManager.PlayBackActionSound();

            duelistState = null;
            CardGameManager.EndTurn();
        }

        void StopDuelist(EventArgs<Duelist, Duelist, CardGameEndReason> _) {
            Debug.Log("Stop the duelist");
            duelistState?.Stop();
            duelistState = null;
        }

        // These need to be overridden
        public abstract bool IsPlayer { get; }

        protected abstract DuelistState StartState { get; }
        protected abstract void InitForGame(EventArgs<List<CardMetadata>, List<CardMetadata>> args);
        protected abstract void SendCurrencyUpdateEvent(int beforeCurrency, int afterCurrency);

        // TODO: Thinking that for these card operations, they should be co-routines so that other states can wait on them

        public void DrawCard() {
            CardGameManager.PlayCardProcessSound();

            if (!Deck.TransferTo(Hand, Deck.NextCard, true)) {
                // Lose
                CardGameManager.EventBus.OnGameEnd.Raise(new(Enemy, this, CardGameEndReason.LastCardDrawn));
            }

            OrganizeArea();
        }

        public void PlaySelectedCard(Card card) {
            CardGameManager.PlayCardSummonSound();

            TurnActions--;
            AdjustCurrency(-card.Cost);

            Hand.TransferTo(Field, card, true);

            OrganizeArea();

            foreach (Effect effect in card.SelfEffects) {
                ApplyCardEffect(effect, card, card);
            }
        }

        public void FireCard(Card card) {
            TurnActions--;
            Discard(card);
        }

        public void ApplyCardEffect(Effect effect, Card source, Card target) {
            source.ApplyEffectTo(target, effect);
        }

        public void Discard(Card card) {
            CardGameManager.PlayCardFireSound();

            CardHolder currentCardHolder = card.GetCurrentHolder();
            currentCardHolder.TransferTo(Graveyard, card, true);
            card.ClearActiveTurns();
            OrganizeArea();
        }

        public bool IsCardSummonAllowed(Card selectedCard) {
            // Duelist must have enough actions
            if (TurnActions <= 0) {
                return false;
            }

            // The field can't be full
            if (Field.Cards.Count >= MAX_FIELD_CARDS) {
                return false;
            }

            // Also, the card must be in the hand
            if (Hand != selectedCard.GetCurrentHolder()) {
                return false;
            }

            return Currency >= selectedCard.Cost;
        }

        public bool IsCardFireAllowed(Card selectedCard) {
            // Duelist must have enough actions
            if (TurnActions <= 0) {
                return false;
            }

            // Net loss income is not fireable
            if (selectedCard.Income <= 0) {
                return false;
            }

            // Card must be on the field
            if (Field != selectedCard.GetCurrentHolder()) {
                return false;
            }

            return true;
        }

        public bool IsCardFireAllowed() {
            return TurnActions > 0;
        }

        void OrganizeArea() {
            // This is called whenever a card based operation occurs
            Field.Organize();
            Hand.Organize();
            Deck.Organize();
            Graveyard.Organize();
        }
    }
}
