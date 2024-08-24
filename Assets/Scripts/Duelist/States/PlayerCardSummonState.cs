using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSummonState : DuelistState {
    private Card cardToSummon;

    public PlayerCardSummonState(Card cardToSummon) {
        this.cardToSummon = cardToSummon;
    }

    public override void EnterState() {
        // throw new System.NotImplementedException();
        // For now, use Hand as holder
        // highlightedHolder = duelist.Hand;
        // this.HighlightedCard = highlightedHolder.Cards[0];

    }

    public override DuelistState HandleState() {
        Debug.Log("In player card summon state");

        // TODO: Handle tribute summons too
        return HandleRegularSummon();
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();
    }

    DuelistState HandleRegularSummon() {
        // Logic goes here for regular summon stuff
        duelist.PlaySelectedCard(cardToSummon);
        cardToSummon.ResetColor();

        return new EndState();
    }

    DuelistState HandleTributeSummon() {
        return null;
    }
}
