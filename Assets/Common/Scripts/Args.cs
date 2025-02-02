using System;
using System.Collections.Generic;

namespace SimCard.Common {
    public class EventArgs<T> : EventArgs {
        public T argument;

        public EventArgs(T argument) => this.argument = argument;
        public void Deconstruct(out T item) {
            item = argument;
        }
    }

    public class EventArgs<T, U> : EventArgs {
        public T arg1;
        public U arg2;

        public EventArgs(T arg1, U arg2) => (this.arg1, this.arg2) = (arg1, arg2);
        public void Deconstruct(out T item1, out U item2) {
            item1 = arg1;
            item2 = arg2;
        }
    }

    public class EventArgs<T, U, V> : EventArgs {
        public T arg1;
        public U arg2;
        public V arg3;

        public EventArgs(T arg1, U arg2, V arg3) => (this.arg1, this.arg2, this.arg3) = (arg1, arg2, arg3);
        public void Deconstruct(out T item1, out U item2, out V item3) {
            item1 = arg1;
            item2 = arg2;
            item3 = arg3;
        }
    }
}
