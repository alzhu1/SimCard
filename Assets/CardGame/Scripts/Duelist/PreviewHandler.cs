using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class PreviewHandler : PreviewUIListener {
        public CardGraphSelectable PreviewedItem {
            get {
                if (selectableStack.TryPeek(out CardGraphSelectable item)) {
                    return item;
                }
                return null;
            }
        }
        public int Index { get; private set; }

        private Stack<CardGraphSelectable> selectableStack;
        private int indexLength;

        public PreviewHandler(CardGraphSelectable previewedItem, int indexLength) {
            selectableStack = new Stack<CardGraphSelectable>();
            selectableStack.Push(previewedItem);
            this.indexLength = Mathf.Max(1, indexLength);
        }

        public void UpdateIndex(int delta) {
            // MINOR: If in the future we need to allow looping, need to account for that
            // Index = (Index + delta + Index) % indexLength;
            Index = Mathf.Clamp(Index + delta, 0, indexLength - 1);
        }

        public void HandleSelection() {
            switch (PreviewedItem) {
                // Only handle for graveyard
                case Graveyard graveyard: {
                    Card subCard = graveyard.Cards[Index];
                    selectableStack.Push(subCard);
                    break;
                }
            }
        }

        public bool HandleEscape() {
            selectableStack.Pop();
            return selectableStack.Count == 0;
        }
    }
}
