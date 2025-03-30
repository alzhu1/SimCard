using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class PlayerCardPreviewState : PlayerState {
        private readonly CardGraphSelectable previewedItem;

        private PreviewHandler previewHandler;

        public PlayerCardPreviewState(CardGraphSelectable previewedItem) {
            this.previewedItem = previewedItem;
        }

        protected override void Enter() {
            previewHandler = new PreviewHandler(previewedItem, playerDuelist.Graveyard.Cards.Count);
            playerDuelist.CardGameManager.EventBus.OnPlayerCardPreview.Raise(new(previewHandler));
        }

        protected override void Exit() {
            playerDuelist.CardGameManager.EventBus.OnPlayerCardPreview.Raise(new(null));
        }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    playerDuelist.CardGameManager.PlayBackActionSound();
                    bool previewDone = previewHandler.HandleEscape();
                    nextState = previewDone ? new PlayerCardSelectedState(previewedItem) : null;
                }

                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    previewHandler.UpdateIndex(-1);
                } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    previewHandler.UpdateIndex(1);
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    playerDuelist.CardGameManager.PlayCursorSelectSound();
                    previewHandler.HandleSelection();
                }

                yield return null;
            }
        }
    }
}
