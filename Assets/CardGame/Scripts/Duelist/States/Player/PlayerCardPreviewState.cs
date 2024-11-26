using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class PlayerCardPreviewState : PlayerState {
        private readonly Card previewedCard;

        public PlayerCardPreviewState(Card previewedCard) {
            this.previewedCard = previewedCard;
        }

        protected override void Enter() {
            Debug.Log("Going to preview the card now");
            playerDuelist.CardGameManager.EventBus.OnPlayerCardPreview.Raise(new(previewedCard, new()));
        }

        protected override void Exit() {
            playerDuelist.CardGameManager.EventBus.OnPlayerCardPreview.Raise(new(null, new()));
        }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    // Revert to base state, keeping the original card
                    nextState = new PlayerCardSelectedState(previewedCard);
                    break;
                }

                yield return null;
            }
        }
    }
}
