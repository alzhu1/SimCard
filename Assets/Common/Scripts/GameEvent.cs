using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    public interface GameEventAction<T> where T : EventArgs {
        public void Raise(T args);
    }

    public class GameEvent<T> : GameEventAction<T> where T : EventArgs {
        public event Action<T> Event = delegate { };
        public void Raise(T args) => Event?.Invoke(args);
    }

    public class SubSceneInitGameEvent<T> : GameEventAction<T> where T : EventArgs {
        private GameEventAction<EventArgs> SubscribeSuccessAction;

        public SubSceneInitGameEvent(GameEventAction<EventArgs> SubscribeSuccessAction) => this.SubscribeSuccessAction = SubscribeSuccessAction;

        public Action<T> _event = delegate { };
        public event Action<T> Event {
            add {
                _event += value;
                SubscribeSuccessAction.Raise(new());
            }

            remove {
                _event -= value;
            }
        }
        public void Raise(T args) => _event?.Invoke(args);
    }
}
