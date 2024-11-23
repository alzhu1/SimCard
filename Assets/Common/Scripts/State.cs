using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    public abstract class State {
        private bool completed;
        protected bool Completed => completed;

        protected MonoBehaviour actor;

        protected State nextState;

        protected abstract void Enter();
        protected abstract IEnumerator Handle();
        protected abstract void Exit();

        private IEnumerator handle;

        protected void InitActor(MonoBehaviour actor) {
            this.actor = actor;
        }

        public void Begin() {
            // Seems like this is considered a bad practice?
            // Considering that these states should go with the duelist,
            // I feel like it should be fine

            // Alternative is to make the coroutine public, and call StartCoroutine in controller
            handle = HandleWithLifecycle();
            actor.StartCoroutine(handle);
        }

        IEnumerator HandleWithLifecycle() {
            Debug.Log($"Entering {this}");
            Enter();
            Debug.Log($"Starting to handle {this}");
            yield return actor.StartCoroutine(Handle());
            Debug.Log($"Exiting {this}");
            Exit();
            completed = true;
        }

        public void Restart() {
            if (handle != null) {
                actor.StopCoroutine(handle);
            }

            handle = HandleWithLifecycle();
            actor.StartCoroutine(handle);
        }
    }
}
