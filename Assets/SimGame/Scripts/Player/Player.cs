using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class Player : MonoBehaviour {
        [SerializeField]
        private Transform frontCheck;

        [SerializeField]
        private float moveSpeed = 1f;

        public SimGameManager SimGameManager { get; private set; }

        private Rigidbody2D rb;
        private bool paused;

        public Transform FrontCheck => frontCheck;
        public float MoveSpeed => moveSpeed;
        public Rigidbody2D RB => rb;
        public bool Paused => paused;

        private Vector3 move;
        private Vector3Int direction;

        private SimPlayerState playerState;

        void Awake() {
            SimGameManager = GetComponentInParent<SimGameManager>();

            rb = GetComponent<Rigidbody2D>();
            direction = Vector3Int.down;
        }

        void Start() {
            playerState = new RegularState();
            playerState.Init(this);
            playerState.Begin();
        }

        void Update() {
            if (playerState != null) {
                SimPlayerState nextState = playerState.NextState;

                if (nextState != null) {
                    playerState = nextState;
                    playerState.Init(this);
                    playerState.Begin();
                }
            }
        }

        void FixedUpdate() {
            rb.velocity = playerState.RBVelocity;
        }

        // TODO: Collisions in another class/script?
        void OnTriggerEnter2D(Collider2D collider) {
            Debug.Log($"Collider: {collider}");

            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable != null) {
                SimGameManager.EventBus.OnCanInteract.Raise(new(interactable));
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            // TODO: Figure out how to distinguish between interactable and non interactable

            Debug.Log($"Collider (exit): {collider}");
            SimGameManager.EventBus.OnCanInteract.Raise(new(null));
        }

        public void Pause() {
            paused = true;
        }

        public void Unpause() {
            paused = false;
        }
    }
}
