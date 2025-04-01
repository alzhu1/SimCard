using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    public abstract class State {
        private bool completed;
        protected bool Completed => completed;

        protected MonoBehaviour actor;
        protected List<Coroutine> subroutines;

        protected State nextState;

        protected abstract void Enter();
        protected abstract IEnumerator Handle();
        protected abstract void Exit();

        private Coroutine coroutine;

        protected void InitActor(MonoBehaviour actor) {
            this.actor = actor;
            subroutines = new List<Coroutine>();
        }

        public void Begin() {
            // Seems like this is considered a bad practice?
            // Considering that these states should go with the duelist,
            // I feel like it should be fine

            // Alternative is to make the coroutine public, and call StartCoroutine in controller
            coroutine = actor.StartCoroutine(HandleWithLifecycle());
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

        public void Stop() {
            Debug.Log($"Requesting handle stop for {this}");

            foreach (Coroutine subroutine in subroutines) {
                actor.StopCoroutine(subroutine);
            }

            if (coroutine != null) {
                actor.StopCoroutine(coroutine);
                Exit();
            }
        }

        public void Restart() {
            Stop();
            actor.StartCoroutine(HandleWithLifecycle());
        }

        // Wrapper around StartCoroutine that uses the actor to start the coroutine
        // This should add on to the subroutine list
        public Coroutine StartSubroutine(IEnumerator handle) {
            Coroutine subroutine = actor.StartCoroutine(handle);
            subroutines.Add(subroutine);
            return subroutine;
        }
    }
}
