using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    [ExecuteInEditMode]
    public class SpriteSorter : MonoBehaviour {
        [SerializeField] private float centerToBottomDistance = 1f;

        private SpriteRenderer sr;

        void Awake() {
            sr = GetComponent<SpriteRenderer>();
        }

        void Update() {
            sr.sortingOrder = Mathf.RoundToInt((transform.position.y - centerToBottomDistance) * 100f) * -1;
        }
    }
}
