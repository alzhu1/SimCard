using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.DeckBuilder {
    public class DeckBuilder : DeckBuilderUIListener {
        public List<CardSO> SelectableCards { get; private set; }
        public Dictionary<CardSO, (int, int)> CardToCount { get; private set; }
        public int Index { get; private set; }

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
        }

        public void UpdateIndex(int delta) {
            Index = Mathf.Max(0, Mathf.Clamp(Index + delta, 0, SelectableCards.Count - 1));
        }

        public void UpdateCardCount(int delta) {
            CardSO currCard = SelectableCards[Index];
            (int currValue, int totalValue) = CardToCount[currCard];
            CardToCount[currCard] = (Mathf.Clamp(currValue + delta, 0, totalValue), totalValue);
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
