using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.DeckBuilder {
    public interface DeckBuilderUIListener {
        public List<CardSO> SelectableCards { get; }
        public Dictionary<CardSO, (int, int)> CardToCount { get; }
        public int Index { get; }
    }

    public class DeckBuilderUI : MonoBehaviour {
        private DeckBuilderManager deckBuilderManager;
        private DeckBuilderUIListener deckBuilderUIListener;

        void Awake() {
            deckBuilderManager = GetComponentInParent<DeckBuilderManager>();
        }

        void Start() {
            deckBuilderManager.EventBus.OnDeckBuilderStart.Event += HandleDeckBuilderStart;
        }

        void OnDestroy() {
            deckBuilderManager.EventBus.OnDeckBuilderStart.Event -= HandleDeckBuilderStart;
        }

        void Update() {
            if (deckBuilderUIListener == null) {
                return;
            }

            // TODO: Handle stuff with UI listener here

            // TODO: Additionally, we want to give the player options on how to sort (e.g. by income, by name, by effects)
        }

        void HandleDeckBuilderStart(EventArgs<DeckBuilderUIListener> args) {
            deckBuilderUIListener = args.argument;
        }
    }
}
