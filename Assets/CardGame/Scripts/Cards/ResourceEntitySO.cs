using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/ResourceEntity")]
    public class ResourceEntitySO : EntitySO {
        public override EntityType entityType => EntityType.Resource;
    }
}
