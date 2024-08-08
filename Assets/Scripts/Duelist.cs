using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duelist : MonoBehaviour {
    public Hand hand;

    void Awake() {
        hand = GetComponentInChildren<Hand>();
    }

    public Hand GetHand() {
        return hand;
    }
}
