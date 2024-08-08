using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    public List<int> cards;
    public Sprite sprite;

    private List<GameObject> cardObjects;
    private Duelist duelist;

    void Awake() {
        duelist = GetComponentInParent<Duelist>();

        // TODO: Object pool?
        cardObjects = new List<GameObject>();
        foreach (var card in cards) {
            GameObject cardObject = new GameObject();
            cardObject.transform.SetParent(transform);

            cardObject.name = $"Card v = {card}";
            
            SpriteRenderer sr = cardObject.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(-6, 6);
            pos.y = Random.Range(-5, 5);
            cardObject.transform.localPosition = pos;

            cardObjects.Add(cardObject);
        }
    }

    public void AddCard(int card) {
        cards.Add(card);

        GameObject cardObject = new GameObject();
        cardObject.transform.SetParent(transform);

        cardObject.name = $"Card v = {card}";
        
        SpriteRenderer sr = cardObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(-10, 10);
        pos.y = Random.Range(-8, 8);
        cardObject.transform.localPosition = pos;

        cardObjects.Add(cardObject);
    }

    public void RemoveCard(int index) {
        cards.RemoveAt(index);
        Destroy(cardObjects[index]);
        cardObjects.RemoveAt(index);
    }

    void Start() {
        
    }

    void Update() {
        
    }
}
