using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EntityCost = Cost<EntitySO>;
using ResourceCost = Cost<ResourceEntitySO>;

public class Card : MonoBehaviour {
    [SerializeField] private CardSO cardSO;

    public EntitySO EntitySO => cardSO.entity;

    public int Power => cardSO.power;

    public List<EntityCost> Costs => cardSO.costs;

    private List<ResourceCost> resourceCosts;
    public List<ResourceCost> ResourceCosts => resourceCosts;

    private List<EntityCost> nonResourceCosts;
    public List<EntityCost> NonResourceCosts => nonResourceCosts;

    // TODO: Card likely needs to carry info about its own state
    // i.e. owned by player/opponent, whether it's on the field, etc

    private SpriteRenderer sr;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = cardSO.sprite;
        gameObject.SetActive(false);

        resourceCosts = new List<ResourceCost>();
        nonResourceCosts = new List<EntityCost>();

        foreach (var cost in Costs) {
            if (cost.entity.IsResource()) {
                resourceCosts.Add(cost.ToResourceCost());
            } else {
                nonResourceCosts.Add(cost);
            }
        }
    }

    void Start() {
        
    }

    void Update() {
        
    }

    public void PlayCard() {}
    public void SacrificeCard() {}

    public bool IsResourceCard() {
        return EntitySO?.IsResource() == true;
    }

    public ResourceEntitySO GetResource() {
        if (IsResourceCard()) {
            return this.EntitySO as ResourceEntitySO;
        }

        return null;
    }

    public bool HasCosts() {
        return Costs.Count > 0;
    }

    public bool HasNonResourceCosts() {
        return NonResourceCosts.Count > 0;
    }

    public void SetHighlightedColor() {
        sr.color = Color.yellow;
    }

    public void SetSelectedColor() {
        sr.color = Color.green;
    }

    public void SetSummonColor() {
        sr.color = Color.red;
    }

    public void ResetColor() {
        Debug.Log("Resetting color");
        sr.color = Color.white;
    }
}
