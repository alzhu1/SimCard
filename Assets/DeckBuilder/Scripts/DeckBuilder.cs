using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.DeckBuilder {
    public class DeckBuilder : DeckBuilderUIListener {
        public enum SortOptions {
            NameDescending = 0,
            NameAscending = 1,
            CostAscending = 2,
            CostDescending = 3,
            IncomeAscending = 4,
            IncomeDescending = 5
        }

        public List<CardSO> SelectableCards { get; private set; }
        public Dictionary<CardSO, (int, int)> CardToCount { get; private set; }
        public CardSO SelectedCard { get; private set; }

        // Index at -1 indicates picking a sort/metadata option
        // Index at 0+ indicates a card is selected
        public int Index { get; private set; }

        public int SubIndex { get; private set; }

        public DeckBuilder(List<CardMetadata> deck, List<CardMetadata> availableCards) {
            CardToCount = new();

            foreach (CardMetadata deckCard in deck) {
                CardToCount.Add(deckCard.cardSO, (deckCard.count, deckCard.count));
            }

            foreach (CardMetadata availableCard in availableCards) {
                if (CardToCount.TryGetValue(availableCard.cardSO, out (int, int) value)) {
                    CardToCount[availableCard.cardSO] = (
                        value.Item1,
                        value.Item2 + availableCard.count
                    );
                } else {
                    CardToCount.Add(availableCard.cardSO, (0, availableCard.count));
                }
            }

            SelectableCards = CardToCount.Select(x => x.Key).ToList();
            Index = 0;
            SubIndex = 0;
        }

        public void UpdateIndex(int delta) {
            if (SelectedCard == null) {
                Index = Mathf.Clamp(Index + delta, -1, SelectableCards.Count - 1);
            }
        }

        public void UpdateAtIndex(int delta) {
            if (SelectedCard == null) {
                if (Index >= 0) {
                    CardSO currCard = SelectableCards[Index];
                    (int currValue, int totalValue) = CardToCount[currCard];
                    CardToCount[currCard] = (Mathf.Clamp(currValue + delta, 0, totalValue), totalValue);
                } else {
                    // TODO: Fix the SubIndex mod, IDK how many options to add or where to store the true result
                    SubIndex = (SubIndex + delta + 6) % 6;
                }
            }
        }

        public void SelectAtIndex() {
            if (Index >= 0) {
                Debug.Log($"Card at index: {SelectableCards[Index]}");
                SelectedCard = SelectableCards[Index];
            } else {
                SortOptions currSortOption = (SortOptions)SubIndex;
                Debug.Log($"Curr sort option: {currSortOption}, SubIndex: {SubIndex}");
                SelectableCards.Sort((a, b) => {
                    switch (currSortOption) {
                        case SortOptions.NameDescending:
                            return a.cardName.CompareTo(b.cardName);

                        case SortOptions.NameAscending:
                            return b.cardName.CompareTo(a.cardName);

                        case SortOptions.CostAscending:
                            return a.cost - b.cost;

                        case SortOptions.CostDescending:
                            return b.cost - a.cost;

                        case SortOptions.IncomeAscending:
                            return a.income - b.income;

                        case SortOptions.IncomeDescending:
                            return b.income - a.income;
                    }
                    return 1;
                });
            }
        }

        public void RevertSelection() {
            SelectedCard = null;
        }

        public (List<CardMetadata>, List<CardMetadata>) OutputDeckBuilder() {
            List<CardMetadata> finalDeck = CardToCount
                .Where(x => x.Value.Item1 > 0)
                .Select(x => new CardMetadata(x.Key, x.Value.Item1))
                .ToList();
            List<CardMetadata> finalAvailableCards = CardToCount
                .Where(x => x.Value.Item2 > x.Value.Item1)
                .Select(x => new CardMetadata(x.Key, x.Value.Item2 - x.Value.Item1))
                .ToList();

            return (finalDeck, finalAvailableCards);
        }
    }
}
