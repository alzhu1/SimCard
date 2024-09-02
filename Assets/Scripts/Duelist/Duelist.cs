using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Duelist : MonoBehaviour {
    private DuelistState duelistState;

    private CardGameManager cardGameManager;

    public Hand Hand { get; private set; }
    public Deck Deck { get; private set; }
    public Field Field { get; private set; }
    public Graveyard Graveyard { get; private set; }

    public Dictionary<ResourceEntitySO, int> CurrentResources { get; private set; }

    public int TotalPower => Field?.Cards?.Sum(x => x.Power) ?? 0;

    private int turnActions;

    void Awake() {
        // Should be provided by parent
        cardGameManager = GetComponentInParent<CardGameManager>();

        // Get different duelist aspects in children objects
        Hand = GetComponentInChildren<Hand>();
        Deck = GetComponentInChildren<Deck>();
        Field = GetComponentInChildren<Field>();
        Graveyard = GetComponentInChildren<Graveyard>();

        CurrentResources = new Dictionary<ResourceEntitySO, int>();
    }

    void Start() {
        cardGameManager.OnGameStart += InitForGame;
    }

    void OnDestroy() {
        cardGameManager.OnGameStart -= InitForGame;
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

    public void StartTurn() {
        turnActions = 4;
        duelistState = StartState;
        duelistState.Init(this);
        duelistState.Begin();
    }

    public void EndTurn() {
        duelistState = null;
        cardGameManager.EndTurn();
    }

    protected abstract void InitForGame();
    protected abstract DuelistState StartState { get; }

    public void DrawCard() {
        if (Deck.TransferTo(Hand, Deck.NextCard, true)) {
            Deck.TryHideDeck();
        }

        OrganizeArea();
    }

    public void PlaySelectedCard(Card card, IEnumerable<HashSet<Card>> cardSacrifices = null) {
        // Remove from resources
        foreach (var resourceCost in card.ResourceCosts) {
            CurrentResources[resourceCost.entity] -= resourceCost.cost;
        }

        // Remove sacrificed cards
        if (cardSacrifices != null) {
            foreach (var cardList in cardSacrifices) {
                foreach (var cardSacrifice in cardList) {
                    Field.TransferTo(Graveyard, cardSacrifice, false);
                }
            }
        }

        // Play the card
        if (card.IsResourceCard()) {
            ResourceEntitySO resource = card.GetResource();
            CurrentResources[resource] =
                CurrentResources.GetValueOrDefault(resource, 0) + card.Power;
            Hand.TransferTo(Graveyard, card, false);
        } else {
            Hand.TransferTo(Field, card, true);
        }

        OrganizeArea();
    }

    void OrganizeArea() {
        // This is called whenever a card based operation occurs
        Field.Spread();
        Hand.Spread();
    }
}
