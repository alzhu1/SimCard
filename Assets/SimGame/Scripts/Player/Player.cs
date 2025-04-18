using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class Player : MonoBehaviour {
        [SerializeField]
        private BoxCollider2D frontCheckCollider;
        public BoxCollider2D FrontCheckCollider => frontCheckCollider;
        public Transform FrontCheck => frontCheckCollider.transform;
        public Vector3 PlayerDirection => frontCheckCollider.transform.localPosition;

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
        private int currency = 0;
        public int Currency => currency;
        public void IncreaseCurrency(int currencyGained) => currency += currencyGained;
        public void ConsumeCurrency(int currencyUsed) => currency = Mathf.Max(0, currency - currencyUsed);

        public Rigidbody2D RB { get; private set; }
        public SpriteRenderer SR { get; private set; }
        public Animator Animator { get; private set; }

        public SimGameManager SimGameManager { get; private set; }
        private SimPlayerState playerState;

        void Awake() {
            SimGameManager = GetComponentInParent<SimGameManager>();

            RB = GetComponent<Rigidbody2D>();
            SR = GetComponent<SpriteRenderer>();
            Animator = GetComponent<Animator>();
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
            RB.velocity = playerState.RBVelocity;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            Debug.Log($"Collider: {collider}");

            if (collider.TryGetComponent(out Interactable interactable)) {
                interactable.HandlePopupVisibility(true);
                SimGameManager.EventBus.OnCanInteract.Raise(new(interactable));
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            Debug.Log($"Collider (exit): {collider}");

            if (collider.TryGetComponent(out Interactable interactable)) {
                interactable.HandlePopupVisibility(false);
                SimGameManager.EventBus.OnCanInteract.Raise(new(null));
            }
        }

        void OnEnable() {
            playerState?.Restart();
        }

        public void UpdateDeckAfterEdit(List<CardMetadata> deck, List<CardMetadata> availableCards) {
            this.deck = deck;
            this.availableCards = availableCards;
        }

        public void AddToAvailableCards(List<CardMetadata> cardsToAdd) {
            // MINOR: This is dumb but should work
            if (cardsToAdd != null) {
                Dictionary<CardSO, int> cardMap = availableCards.ToDictionary(x => x.cardSO, x => x.count);
                foreach (CardMetadata card in cardsToAdd) {
                    if (!cardMap.ContainsKey(card.cardSO)) {
                        cardMap.Add(card.cardSO, 0);
                    }
                    cardMap[card.cardSO] += card.count;
                }
                availableCards = cardMap.Select(x => new CardMetadata(x.Key, x.Value)).ToList();
            }
        }

        public int GetCardCount(CardSO card) {
            int deckCardCount = Deck.Find(cardMetadata => cardMetadata.cardSO.Equals(card))?.count ?? 0;
            int availableCardCount = AvailableCards.Find(cardMetadata => cardMetadata.cardSO.Equals(card))?.count ?? 0;
            return deckCardCount + availableCardCount;
        }
    }
}
