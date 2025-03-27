using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    [System.Serializable]
    public class Sound {
        public string name;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;

        public bool loop;

        public float pitch;

        [HideInInspector]
        public AudioSource source;
    }

    public class AudioSystem : MonoBehaviour {
        [SerializeField] private Sound[] sounds;

        void Awake() {
            foreach (Sound s in sounds) {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.loop = s.loop;
                s.source.pitch = s.pitch;

                s.source.playOnAwake = false;
                s.source.bypassEffects = true;
                s.source.bypassListenerEffects = true;
                s.source.bypassReverbZones = true;
            }
        }

        public Sound Play(string name) {
            Debug.Log($"About to play {name}");

            Sound s = System.Array.Find<Sound>(sounds, sound => sound.name.Equals(name));
            if (s == null || s.source == null) {
                Debug.LogWarning($"Sound {name} does not exist!");
            } else {
                s?.source.Play();
            }
            return s;
        }

        // void ReceiveLevelCompleteEvent() {
        //     Play("LevelComplete");
        // }

        // void ReceivePlayerMoveEvent() {
        //     Play("Move");
        // }

        // void ReceivePlayerActionEvent(bool hit) {
        //     Play(hit ? "Hit" : "Action");
        // }

        // void ReceivePlayerSwitchEvent() {
        //     Play("Switch");
        // }
    }
}
