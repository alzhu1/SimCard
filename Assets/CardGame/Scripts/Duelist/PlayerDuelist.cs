using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public class PlayerDuelist : Duelist {
        // MINOR: Cursor as its own script?
        [SerializeField]
        private SpriteRenderer cursor;

        [SerializeField]
        private float cursorMoveTime = 1f;

        protected override DuelistState StartState => new DrawState<PlayerBaseState>();

        protected override void InitForGame(InitCardGameArgs args) {
            if (args != null) {
                Deck.InitFromCardMetadata(args.playerDeck, CardGameManager);
            }

            for (int i = 0; i < FirstDrawAmount; i++) {
                DrawCard();
            }
        }

        public void ShowCursor() {
            cursor.enabled = true;
        }

        public void HideCursor() {
            cursor.enabled = false;
        }

        public Coroutine MoveCursorTo(CardGraphSelectable selectable, bool instant = false) {
            return StartCoroutine(MoveCursor(selectable, instant));
        }

        IEnumerator MoveCursor(CardGraphSelectable selectable, bool instant) {
            Vector3 start = cursor.transform.position;
            Vector3 dest = selectable.transform.position;
            float t = 0;

            if (!instant) {
                while (t < cursorMoveTime) {
                    cursor.transform.position = Vector3.Lerp(start, dest, t / cursorMoveTime);
                    yield return null;
                    t += Time.deltaTime;
                }
            }

            cursor.transform.position = dest;
        }
    }
}
