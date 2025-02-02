using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class Player : MonoBehaviour {
        [SerializeField]
        private Collider2D frontCheckCollider;
        public Transform FrontCheck => frontCheckCollider.transform;
        public Collider2D FrontCheckCollider => frontCheckCollider;

        [SerializeField]
        private float moveSpeed = 1f;
        public float MoveSpeed => moveSpeed;

        [SerializeField]
        private List<CardMetadata> deck;
        public List<CardMetadata> Deck => deck;

        [SerializeField]
        private List<CardMetadata> availableCards;
        public List<CardMetadata> AvailableCards => availableCards;

        [SerializeField]
        private int energy = 100;
        public int Energy => energy;
        public void RefreshEnergy() => energy = 100;
        public void ConsumeEnergy(int energyUsed) => energy = Mathf.Max(0, energy - energyUsed);


        [SerializeField]
        private int currency = 0;
        public int Currency => currency;
        public void IncreaseCurrency(int currencyGained) => currency += currencyGained;
        public void ConsumeCurrency(int currencyUsed) => currency = Mathf.Max(0, currency - currencyUsed);

        private Rigidbody2D rb;
        public Rigidbody2D RB => rb;

        private SpriteRenderer sr;
        public SpriteRenderer SR => sr;

        public SimGameManager SimGameManager { get; private set; }
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

        void OnTriggerEnter2D(Collider2D collider) {
            Debug.Log($"Collider: {collider}");

            if (collider.TryGetComponent(out Interactable interactable)) {
                SimGameManager.EventBus.OnCanInteract.Raise(new(interactable));
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            Debug.Log($"Collider (exit): {collider}");

            if (collider.TryGetComponent(out Interactable _)) {
                SimGameManager.EventBus.OnCanInteract.Raise(new(null));
            }
        }

        public void UpdateDeckAfterEdit(List<CardMetadata> deck, List<CardMetadata> availableCards) {
            this.deck = deck;
            this.availableCards = availableCards;
        }

        void OnEnable() {
            playerState?.Restart();
        }
    }
}
