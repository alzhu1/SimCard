using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class Player : MonoBehaviour {
        [SerializeField] private Transform frontCheck;
        [SerializeField] private float moveSpeed = 1f;

        public SimGameManager SimGameManager { get; private set; }

        private Rigidbody2D rb;

        public Transform FrontCheck => frontCheck;
        public float MoveSpeed => moveSpeed;
        public Rigidbody2D RB => rb;

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

            SimGameManager.EventBus.OnCanInteract.Raise(new InteractArgs(collider.gameObject));
        }

        void OnTriggerExit2D(Collider2D collider) {
            SimGameManager.EventBus.OnCanInteract.Raise(new InteractArgs(null));
        }
    }
}
