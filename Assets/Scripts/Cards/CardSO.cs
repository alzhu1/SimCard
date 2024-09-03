using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EntityCost = SimCard.CardGame.Cost<SimCard.CardGame.EntitySO>;
using ResourceCost = SimCard.CardGame.Cost<SimCard.CardGame.ResourceEntitySO>;

namespace SimCard.CardGame {
    [System.Serializable]
    public struct Cost<T> where T : EntitySO {
        public T entity;
        public int cost;

        public ResourceCost ToResourceCost() {
            if (entity is ResourceEntitySO) {
                return new() {
                    entity = entity as ResourceEntitySO,
                    cost = cost
                };
            }
            return new() { entity = null, cost = -1 };
        }
    }

    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card")]
    public class CardSO : ScriptableObject {
        public string cardName;
        public int power;
        public Sprite sprite;
        public string flavorText;
        public string effectText;

        // Entity could represent global resources (e.g. food, fabrics)
        // Or it could represent unique units (so a unit has a CardSO + EntitySO)
        public EntitySO entity;

        // TODO: This is just an idea but could change in future
        // May want to do some kind of "level" summon system like YGO
        public List<EntityCost> costs;
    }
}
