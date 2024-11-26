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
}
