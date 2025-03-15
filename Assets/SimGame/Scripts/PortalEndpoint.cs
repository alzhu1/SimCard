using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class PortalEndpoint : MonoBehaviour {
        [SerializeField] private PortalEndpoint otherEndpoint;

        private SimGameManager simGameManager;

        private bool endpointActive;

        void Awake() {
            simGameManager = GetComponentInParent<SimGameManager>();
            endpointActive = true;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (endpointActive && collider.TryGetComponent(out Player player)) {
                // Deactivate the other endpoint to prevent infinite looping
                otherEndpoint.endpointActive = false;
                simGameManager.EventBus.OnPlayerTeleport.Raise(new(otherEndpoint.transform.position));
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            endpointActive = true;
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, otherEndpoint.transform.position);
        }
    }
}
