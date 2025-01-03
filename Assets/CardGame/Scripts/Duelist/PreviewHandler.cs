using System.Collections.Generic;

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
            this.indexLength = indexLength;
        }

        public void UpdateIndex(int delta) {
            Index = (Index + delta + Index) % indexLength;
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
