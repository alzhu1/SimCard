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
        private int spriteIndex;

        private SimGameManager simGameManager;

        private Coroutine doorAnimatorCoroutine;

        void Awake() {
            buildingSr = GetComponentInParent<SpriteRenderer>();
            spriteIndex = 0;

            simGameManager = GetComponentInParent<SimGameManager>();

            openDoorParentCollider.enabled = doorEnabled;
            closedDoorParentCollider.enabled = !doorEnabled;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (doorEnabled && collider.TryGetComponent(out Player player)) {
                StartDoorAnimation(true);
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (doorEnabled && collider.TryGetComponent(out Player player)) {
                StartDoorAnimation(false);
            }
        }

        void StartDoorAnimation(bool toOpen) {
            if (doorAnimatorCoroutine != null) {
                StopCoroutine(doorAnimatorCoroutine);
            }

            spriteIndex = Mathf.Clamp(spriteIndex, 0, buildingSprites.Length - 1);
            doorAnimatorCoroutine = StartCoroutine(ChangeDoorState(toOpen));
        }

        IEnumerator ChangeDoorState(bool toOpen) {
            if (toOpen) {
                simGameManager.PlayDoorOpenSound();
            } else {
                simGameManager.PlayDoorCloseSound();
            }

            int delta = toOpen ? 1 : -1;

            while (spriteIndex >= 0 && spriteIndex < buildingSprites.Length) {
                buildingSr.sprite = buildingSprites[spriteIndex];
                yield return new WaitForSeconds(frameTime);
                spriteIndex += delta;
            }
        }
    }
}
