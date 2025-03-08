using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class BuildingDoorTrigger : MonoBehaviour {
        [SerializeField] private bool doorEnabled = true;
        [SerializeField] private Collider2D openDoorParentCollider;
        [SerializeField] private Collider2D closedDoorParentCollider;

        [SerializeField] private Sprite[] buildingSprites;
        [SerializeField] private float frameTime = 0.25f;

        private SpriteRenderer buildingSr;

        void Awake() {
            buildingSr = GetComponentInParent<SpriteRenderer>();
            openDoorParentCollider.enabled = doorEnabled;
            closedDoorParentCollider.enabled = !doorEnabled;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (doorEnabled && collider.TryGetComponent(out Player player)) {
                StartCoroutine(ChangeDoorState(true));
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (doorEnabled && collider.TryGetComponent(out Player player)) {
                StartCoroutine(ChangeDoorState(false));
            }
        }

        IEnumerator ChangeDoorState(bool toOpen) {
            int index = toOpen ? 0 : buildingSprites.Length - 1;
            int delta = toOpen ? 1 : -1;

            while (index >= 0 && index < buildingSprites.Length) {
                buildingSr.sprite = buildingSprites[index];
                yield return new WaitForSeconds(frameTime);
                index += delta;
            }
        }
    }
}
