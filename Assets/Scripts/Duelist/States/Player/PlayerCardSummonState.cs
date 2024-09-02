using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSummonState : PlayerState {
    private readonly Card cardToSummon;

    // Tribute summon
    private Dictionary<EntitySO, HashSet<Card>> suppliedCards;
    private CardGraph cardGraph;

    public PlayerCardSummonState(Card cardToSummon) {
        this.cardToSummon = cardToSummon;
    }

    protected override void Enter() {
        switch (cardToSummon.SummonType) {
            case CardSummonType.Tribute: {
                suppliedCards = new Dictionary<EntitySO, HashSet<Card>>();
                foreach (var nonResourceCost in cardToSummon.NonResourceCosts) {
                    suppliedCards[nonResourceCost.entity] = new HashSet<Card>();
                }

                cardGraph = new CardGraph(new() {
                    playerDuelist.Field.Cards
                }, null);

                // Set cursor position
                playerDuelist.ShowCursor();
                playerDuelist.MoveCursorToCard(cardGraph.CurrCard, true);
                break;
            }
        }
    }

    protected override void Exit() {
        if (suppliedCards != null) {
            foreach (var cardSet in suppliedCards.Values) {
                foreach (var card in cardSet) {
                    card.ResetColor();
                }
            }
        }
        playerDuelist.HideCursor();
    }

    protected override IEnumerator Handle() {

        while (nextState == null) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                nextState = new PlayerCardSelectedState(cardToSummon);
                break;
            }

            switch (cardToSummon.SummonType) {
                case CardSummonType.Regular:
                    nextState = HandleRegularSummon();
                    break;

                case CardSummonType.Tribute:
                    nextState = HandleTributeSummon();
                    break;
            }

            yield return null;
        }
    }

    DuelistState HandleRegularSummon() {
        // Logic goes here for regular summon stuff
        duelist.PlaySelectedCard(cardToSummon);
        cardToSummon.ResetColor();

        return new PlayerBaseState();
    }

    DuelistState HandleTributeSummon() {
        if (IsTributeSummonAllowed()) {
            playerDuelist.HideCursor();

            if (Input.GetKeyDown(KeyCode.Space)) {
                duelist.PlaySelectedCard(cardToSummon, suppliedCards.Values);
                cardToSummon.ResetColor();
                return new PlayerBaseState();
            }

            return null;
        }

        Card currCard = cardGraph.CurrCard;

        if (suppliedCards.ContainsKey(currCard.Entity) && Input.GetKeyDown(KeyCode.Space)) {
            HashSet<Card> cardSet = suppliedCards[currCard.Entity];

            if (cardSet.Contains(currCard)) {
                // Remove the card
                cardSet.Remove(currCard);
                currCard.ResetColor();
            } else {
                // It is a cost we can cadd
                cardSet.Add(currCard);
                currCard.SetSelectedColor();
            }
        }

        Vector2Int move = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            move = Vector2Int.left;
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            move = Vector2Int.right;
        }

        if (!move.Equals(Vector2Int.zero)) {
            // Card fromCard = cardGraph.CurrCard;
            cardGraph.MoveNode(move);
            Card toCard = cardGraph.CurrCard;

            // TODO: How to run coroutine in this context??
            // yield return playerDuelist.MoveCursorToCard(toCard);
            playerDuelist.MoveCursorToCard(toCard);
        }

        return null;
    }

    bool IsTributeSummonAllowed() {
        foreach (var nonResourceCost in cardToSummon.NonResourceCosts) {
            if (suppliedCards[nonResourceCost.entity].Count != nonResourceCost.cost) {
                return false;
            }
        }

        Debug.Log("Allowed!");

        return true;
    }
}
