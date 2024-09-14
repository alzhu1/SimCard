using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    public enum EntityType {
        Normal,
        Resource,
    }

    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Entity")]
    public class EntitySO : ScriptableObject {
        public string entityName;

        public virtual EntityType entityType => EntityType.Normal;

        public bool IsResource() {
            return entityType == EntityType.Resource;
        }
    }
}
