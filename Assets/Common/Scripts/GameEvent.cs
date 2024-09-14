using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    public class GameEvent<T> where T : EventArgs {
        public event Action<T> Event = delegate { };
        public void Raise(T args) => Event?.Invoke(args);
    }
}
