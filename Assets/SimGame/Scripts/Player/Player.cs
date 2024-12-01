using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class Player : MonoBehaviour {
        [SerializeField]
        private Transform frontCheck;

        [SerializeField]
        private float moveSpeed = 1f;

        [SerializeField]
        private List<CardMetadata> deck;

        public SimGameManager SimGameManager { get; private set; }

        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private int energy = 100;

        public List<CardMetadata> Deck => deck;
        public Transform FrontCheck => frontCheck;
        public float MoveSpeed => moveSpeed;
        public Rigidbody2D RB => rb;
        public SpriteRenderer SR => sr;
        public int Energy => energy;

        private SimPlayerState playerState;

        void Awake() {
            SimGameManager = GetComponentInParent<SimGameManager>();

            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
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

        // FIXME: Minor bug after sleep transition. The state goes back to RegularState, but this isn't triggered
        // So no interactable is set, but the UI is still there
        // Should do a check on startup to see if we need to set interactable again
        void OnTriggerExit2D(Collider2D collider) {
            // TODO: Figure out how to distinguish between interactable and non interactable

            Debug.Log($"Collider (exit): {collider}");
            SimGameManager.EventBus.OnCanInteract.Raise(new(null));
        }

        public void RefreshEnergy() {
            energy = 100;
        }

        public void ConsumeEnergy(int energyUsed) {
            energy = Mathf.Max(0, energy - energyUsed);
        }

        void OnEnable() {
            playerState?.Restart();
        }
    }
}
