using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    [ExecuteInEditMode]
    public class SpriteSorter : MonoBehaviour {
        [UnityEngine.Serialization.FormerlySerializedAs("centerToBottomDistance")]
        [SerializeField]
        private float verticalAdjustment = 1f;

        private SpriteRenderer sr;

        void Awake() {
            sr = GetComponent<SpriteRenderer>();
        }

        void Update() {
            sr.sortingOrder = Mathf.RoundToInt((transform.position.y - verticalAdjustment) * 100f) * -1;
        }
    }
}
