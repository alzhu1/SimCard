using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using SimCard.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SimCard.SimGame {
    public class SimGameManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static SimGameManager instance = null;

        public SimGameEventBus EventBus { get; private set; }

        [SerializeField] private GameObject environment;
        [SerializeField] private GameObject canvasUI;
        [SerializeField] private CinemachineVirtualCamera vCam;
        [SerializeField] private bool showCardGameTutorial;

        private Camera simGameCamera;
        private CinemachineFramingTransposer transposer;
        private CinemachineConfiner2D confiner;
        private int boundsLayer;

        private bool loadingSubScene;
        private bool isSceneChanging;

        private Player player;
        private InteractUI interactUI;
        private FadeUI fadeUI;

        private AudioSystem simGameAudioSystem;

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            EventBus = GetComponent<SimGameEventBus>();

            simGameCamera = Camera.main;
            transposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            confiner = vCam.GetComponent<CinemachineConfiner2D>();
            boundsLayer = LayerMask.NameToLayer("Bounds");

            player = GetComponentInChildren<Player>();
            interactUI = canvasUI.GetComponentInChildren<InteractUI>();
            fadeUI = canvasUI.GetComponentInChildren<FadeUI>();

            simGameAudioSystem = GetComponentInChildren<AudioSystem>();
        }

        void Start() {
            EventBus.OnInteractionEvent.Event += HandleInteractionEvent;
            EventBus.OnPlayerTeleport.Event += HandleTeleport;
            EventBus.OnCardGameEnd.Event += HandleCardGameEndEvent;
            EventBus.OnDeckBuilderEnd.Event += HandleDeckBuilderEndEvent;
            EventBus.OnSubSceneLoaded.Event += HandleSubSceneLoadedEvent;

            simGameAudioSystem.Play("BGM");
        }

        void OnDestroy() {
            EventBus.OnInteractionEvent.Event -= HandleInteractionEvent;
            EventBus.OnPlayerTeleport.Event -= HandleTeleport;
            EventBus.OnCardGameEnd.Event -= HandleCardGameEndEvent;
            EventBus.OnDeckBuilderEnd.Event -= HandleDeckBuilderEndEvent;
            EventBus.OnSubSceneLoaded.Event -= HandleSubSceneLoadedEvent;
        }

        void HandleInteractionEvent(EventArgs<string, Interactable, int> args) {
            (string eventName, Interactable interactable, int index) = args;

            Debug.Log($"Received event: {eventName}");

            switch (eventName) {
                case "StartCardGame": {
                    StartCoroutine(StartCardGame(interactable));
                    break;
                }

                case "StartDeckBuild": {
                    StartCoroutine(StartDeckBuilder());
                    break;
                }

                case "Buy": {
                    Debug.Log($"Buying, the arg index is {index}, card in deck is {interactable.Deck[index].cardSO}");

                    CardMetadata cardMetadataToBuy = interactable.Deck[index];
                    CardSO cardToBuy = cardMetadataToBuy.cardSO;

                    // Put it in the player's available cards, not deck
                    CardMetadata playerCardMetadata = player.AvailableCards.Find(cardMetadata => cardMetadata.cardSO.Equals(cardToBuy));
                    if (playerCardMetadata != null) {
                        playerCardMetadata.count++;
                    } else {
                        player.AvailableCards.Add(new CardMetadata(cardToBuy, 1));
                    }

                    player.ConsumeCurrency(cardMetadataToBuy.count);
                    break;
                }

                default:
                    break;
            }
        }

        void HandleTeleport(EventArgs<Vector3> args) {
            PlayTeleportSound();
            StartCoroutine(Teleport(args.argument));
        }

        void HandleCardGameEndEvent(EventArgs<int, List<CardMetadata>> args) {
            (int goldWon, List<CardMetadata> pack) = args;
            player.IncreaseCurrency(goldWon);
            player.AddToAvailableCards(pack);

            StartCoroutine(EndCardGame());
        }

        void HandleDeckBuilderEndEvent(EventArgs<List<CardMetadata>, List<CardMetadata>> args) {
            Debug.Log("Returned from deck edit");
            (List<CardMetadata> deck, List<CardMetadata> availableCards) = args;
            player.UpdateDeckAfterEdit(deck, availableCards);
            StartCoroutine(EndDeckBuilder());
        }

        void HandleSubSceneLoadedEvent(EventArgs _) => loadingSubScene = false;

        // Interaction mediator methods
        public Coroutine StartInteractionCoroutine(InteractionParser parser) {
            return StartCoroutine(StartInteraction(parser));
        }

        public Coroutine EndInteractionCoroutine(InteractionParser parser) {
            return StartCoroutine(EndInteraction(parser));
        }

        IEnumerator StartInteraction(InteractionParser parser) {
            // Animate UI up + init parser for listening
            yield return interactUI.StartInteraction();
            interactUI.Parser = parser;
        }

        IEnumerator EndInteraction(InteractionParser parser) {
            // Don't reference parser anymore + animate UI down
            interactUI.Parser = null;
            yield return interactUI.EndInteraction();
            parser.EndInteraction();
        }

        IEnumerator Teleport(Vector3 destination) {
            EventBus.OnPlayerPause.Raise(new(false));

            interactUI.Hide();
            yield return fadeUI.FadeIn(0.5f);

            // Save values
            Vector3 damping = new Vector3(
                transposer.m_XDamping,
                transposer.m_YDamping,
                transposer.m_ZDamping
            );
            Vector3 deadZone = new Vector3(
                transposer.m_DeadZoneWidth,
                transposer.m_DeadZoneHeight,
                transposer.m_DeadZoneDepth
            );

            // Set all these to 0
            transposer.m_XDamping = transposer.m_YDamping = transposer.m_ZDamping = 0;
            transposer.m_DeadZoneWidth = transposer.m_DeadZoneHeight = transposer.m_DeadZoneDepth = 0;

            // Unset confiner
            confiner.m_BoundingShape2D = null;

            // Directly disable vCam, then wait a frame to force an instant camera update (instead of smooth transition)
            vCam.enabled = false;
            yield return null;

            player.transform.position = destination;

            // Set the confiner to the enclosing bound (this assumes no overlapping bounds)
            confiner.m_BoundingShape2D = Physics2D.OverlapCircle(destination, 0.01f, 1 << boundsLayer);
            vCam.enabled = true;

            yield return fadeUI.FadeOut(0.5f);

            // Restore vCam values
            transposer.m_XDamping = damping.x;
            transposer.m_YDamping = damping.y;
            transposer.m_ZDamping = damping.z;

            transposer.m_DeadZoneWidth = deadZone.x;
            transposer.m_DeadZoneHeight = deadZone.y;
            transposer.m_DeadZoneDepth = deadZone.z;

            EventBus.OnPlayerUnpause.Raise(new());
        }

        IEnumerator StartCardGame(Interactable interactable) {
            yield return StartCoroutine(LoadSubScene(1, () => {
                EventBus.OnCardGameInit.Raise(new(player.Deck, interactable.Deck, showCardGameTutorial));
                showCardGameTutorial = false;
            }, true));
        }

        IEnumerator EndCardGame() {
            yield return StartCoroutine(UnloadSubScene(1, true));
        }

        IEnumerator StartDeckBuilder() {
            yield return StartCoroutine(LoadSubScene(2, () => EventBus.OnDeckBuilderInit.Raise(new(player.Deck, player.AvailableCards))));
        }

        IEnumerator EndDeckBuilder() {
            yield return StartCoroutine(UnloadSubScene(2));
        }

        // Scene loading helpers
        IEnumerator LoadSubScene(int sceneIndex, Action initAction, bool fadeBGM = false) {
            yield return new WaitUntil(() => !isSceneChanging);
            isSceneChanging = true;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

            // Hide the interact UI when loading
            interactUI.Hide();

            asyncLoad.allowSceneActivation = false;
            // Disable player movement beforehand, keep sprite (will disable later)
            EventBus.OnPlayerPause.Raise(new(false));
            if (fadeBGM) {
                StartCoroutine(simGameAudioSystem.FadeOut("BGM", 1f));
            }

            yield return fadeUI.FadeIn();

            loadingSubScene = true;

            while (asyncLoad.progress < 0.9f) {
                yield return null;
            }

            // Disable environmental objects in sim game
            EventBus.OnPlayerPause.Raise(new(true));
            environment.SetActive(false);
            asyncLoad.allowSceneActivation = true;

            // Wait for sub scene to have loaded, then init
            yield return new WaitUntil(() => loadingSubScene);
            initAction.Invoke();
            yield return null;

            yield return fadeUI.FadeOut();
            canvasUI.SetActive(false);
            // simGameCamera.gameObject.SetActive(false);
            simGameCamera.enabled = false;

            isSceneChanging = false;
        }

        IEnumerator UnloadSubScene(int sceneIndex, bool fadeBGM = false) {
            yield return new WaitUntil(() => !isSceneChanging);
            isSceneChanging = true;

            canvasUI.SetActive(true);
            // simGameCamera.gameObject.SetActive(true);
            simGameCamera.enabled = true;

            yield return fadeUI.FadeIn();

            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneIndex);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f) {
                yield return null;
            }

            // Disable environmental objects in sim game
            EventBus.OnPlayerUnpause.Raise(new());
            environment.SetActive(true);
            asyncLoad.allowSceneActivation = true;

            if (fadeBGM) {
                StartCoroutine(simGameAudioSystem.FadeIn("BGM", 1f));
            }

            yield return fadeUI.FadeOut();

            isSceneChanging = false;
        }

        // Sounds
        public void PlayOptionMoveSound() => simGameAudioSystem.Play("OptionMove");
        public void PlayAdvanceSound() => simGameAudioSystem.Play("Advance");
        public void PlayBackActionSound() => simGameAudioSystem.Play("BackAction");
        public void PlayDoorOpenSound() => simGameAudioSystem.Play("DoorOpen");
        public void PlayDoorCloseSound() => simGameAudioSystem.Play("DoorClose");
        void PlayTeleportSound() => simGameAudioSystem.Play("Teleport");
    }
}
