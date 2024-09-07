using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    // TODO: Maybe revisit this idea of a class containing the event + raise method
    // Main challenge is figuring out how to pass args
    // i.e. is there a simple way to pass in no/1/2/etc args

    // public abstract class CardGameEventArgs { }
    // public class EmptyArgs : CardGameEventArgs { }
    // public class DuelistArgs: CardGameEventArgs {
    //     public Duelist duelist;
    //     public DuelistArgs(Duelist duelist) => this.duelist = duelist;
    // }

    // public class CardGameEvent {
    //     public event Action<CardGameEventArgs> Event = delegate { };

    //     public void Raise(CardGameEventArgs args) {
    //         Event?.Invoke(args);
    //     }
    // }

    public class CardGameEventBus : MonoBehaviour {
        private static CardGameEventBus instance = null;

        public event Action OnGameStart = delegate { };
        public event Action<Duelist> OnTurnStart = delegate { };
        public event Action OnGameEnd = delegate { };

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }
        }

        public void RaiseOnGameStart() {
            OnGameStart?.Invoke();
        }

        public void RaiseOnTurnStart(Duelist duelist) {
            OnTurnStart?.Invoke(duelist);
        }

        public void RaiseOnGameEnd() {
            OnGameEnd?.Invoke();
        }
    }
}
