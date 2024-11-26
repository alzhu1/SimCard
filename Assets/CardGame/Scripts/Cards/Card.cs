using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;
using EntityCost = SimCard.Common.Cost<SimCard.Common.EntitySO>;
using ResourceCost = SimCard.Common.Cost<SimCard.Common.ResourceEntitySO>;

namespace SimCard.CardGame {
    public enum CardSummonType {
        Regular,
        Tribute,
    }

    public class Card : MonoBehaviour {
        [SerializeField]
        private CardSO cardSO;
        public CardSO CardSO => cardSO;

        public EntitySO Entity => cardSO.entity;

        public int Power => cardSO.power;

        public List<EntityCost> Costs => cardSO.costs;

        public List<ResourceCost> ResourceCosts { get; private set; }
        public List<EntityCost> NonResourceCosts { get; private set; }

        public CardSummonType SummonType {
            get {
                if (HasNonResourceCosts()) {
                    return CardSummonType.Tribute;
                }

                return CardSummonType.Regular;
            }
        }

        // TODO: Card likely needs to carry info about its own state
        // i.e. owned by player/opponent, whether it's on the field, etc

        private SpriteRenderer sr;

        void Awake() {
            if (cardSO == null) {
                return;
            }

            sr = GetComponent<SpriteRenderer>();
            sr.sprite = cardSO.sprite;
            gameObject.SetActive(false);

            ResourceCosts = new List<ResourceCost>();
            NonResourceCosts = new List<EntityCost>();

            foreach (EntityCost cost in Costs) {
                if (cost.entity.IsResource()) {
                    ResourceCosts.Add(cost.ToResourceCost());
                } else {
                    NonResourceCosts.Add(cost);
                }
            }
        }

        public void InitCardSO(CardSO cardSO) {
            this.cardSO = cardSO;
            Awake();
        }

        public bool IsResourceCard() {
            return Entity.IsResource() == true;
        }

        public ResourceEntitySO GetResource() {
            if (IsResourceCard()) {
                return Entity as ResourceEntitySO;
            }

            return null;
        }

        public bool HasCosts() {
            return Costs.Count > 0;
        }

        public bool HasNonResourceCosts() {
            return NonResourceCosts.Count > 0;
        }

        public void SetSelectedColor() {
            sr.color = Color.green;
        }

        public void ResetColor() {
            Debug.Log("Resetting color");
            sr.color = Color.white;
        }
    }
}
