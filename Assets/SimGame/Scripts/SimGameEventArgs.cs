using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class Args<T> : EventArgs {
        public T argument;

        public Args(T argument) => this.argument = argument;
    }
}
