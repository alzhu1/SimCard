using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;
using UnityEngine.UI;

namespace SimCard.DeckBuilder {
    public interface DeckBuilderUIListener {
        public List<CardSO> SelectableCards { get; }
        public Dictionary<CardSO, (int, int)> CardToCount { get; }
        public int Index { get; }
    }

    public class DeckBuilderUI : MonoBehaviour {
        [SerializeField]
        private Image upArrow;

        [SerializeField]
        private DeckBuilderCardUI[] cardRows;

        [SerializeField]
        private Image downArrow;

        private DeckBuilderManager deckBuilderManager;
        private int topIndex;

        public DeckBuilderUIListener DeckBuilderUIListener { get; set; }

        void Awake() {
            deckBuilderManager = GetComponentInParent<DeckBuilderManager>();
            topIndex = 0;
        }

        void Update() {
            if (DeckBuilderUIListener == null) {
                return;
            }

            // TODO: Add more options on top of screen (e.g. selecting/sorting options)

            // Handle top index update
            topIndex = Mathf.Min(topIndex, DeckBuilderUIListener.Index);
            topIndex =
                Mathf.Max(topIndex + cardRows.Length - 1, DeckBuilderUIListener.Index)
                - (cardRows.Length - 1);

            for (int i = topIndex; i < topIndex + cardRows.Length; i++) {
                int cardRowIndex = i - topIndex;
                DeckBuilderCardUI cardRow = cardRows[cardRowIndex];

                if (i < DeckBuilderUIListener.SelectableCards.Count) {
                    cardRow.gameObject.SetActive(true);
                    CardSO card = DeckBuilderUIListener.SelectableCards[i];

                    // Set basic info
                    cardRow.CardNameText.text = card.cardName;
                    cardRow.CostText.text = card.cost.ToString();
                    cardRow.IncomeText.text = card.income.ToString();

                    // Update the deck count text
                    (int deckCount, int totalCount) = DeckBuilderUIListener.CardToCount[card];
                    cardRow.DeckCountText.text = $"{deckCount} / {totalCount}";

                    // Red text if it's the current index
                    cardRows[cardRowIndex]
                        .SetTextColor(i == DeckBuilderUIListener.Index ? Color.red : Color.white);
                } else {
                    cardRow.gameObject.SetActive(false);
                }
            }

            upArrow.enabled = topIndex != 0;
            downArrow.enabled = topIndex + cardRows.Length < DeckBuilderUIListener.SelectableCards.Count;
        }
    }
}
