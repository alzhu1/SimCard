using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType {
    Normal,
    Resource
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Entity")]
public class EntitySO : ScriptableObject {
    public string entityName;

    public virtual EntityType entityType => EntityType.Normal;

    public bool IsResource() {
        return entityType == EntityType.Resource;
    }
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/ResourceEntity")]
public class ResourceEntitySO : EntitySO {
    public override EntityType entityType => EntityType.Resource;
}
