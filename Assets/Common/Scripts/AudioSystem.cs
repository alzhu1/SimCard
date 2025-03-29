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

        Sound GetSound(string name) {
            Sound s = System.Array.Find<Sound>(sounds, sound => sound.name.Equals(name));
            if (s == null || s.source == null) {
                Debug.LogWarning($"Sound {name} does not exist!");
                return null;
            }

            return s;
        }

        public Sound Play(string name) {
            Debug.Log($"About to play {name}");

            Sound s = GetSound(name);
            s?.source.Play();
            return s;
        }

        IEnumerator Fade(string name, float fadeTime, bool fadeOut) {
            Sound s = GetSound(name);
            if (s == null) {
                Debug.Log("No source found or is not playing");
                yield break;
            }

            Debug.Log($"Fade for {name}, fadeOut: {fadeOut}");

            if (!fadeOut) {
                s.source.Play();
            }

            float startVolume = fadeOut ? s.volume : 0;
            float endVolume = fadeOut ? 0 : s.volume;

            float t = 0;
            while (t < fadeTime) {
                s.source.volume = Mathf.Lerp(startVolume, endVolume, t / fadeTime);
                yield return null;
                t += Time.deltaTime;
            }

            s.source.volume = endVolume;

            if (fadeOut) {
                s.source.Stop();
            }
        }

        public IEnumerator FadeOut(string name, float fadeTime) => Fade(name, fadeTime, true);
        public IEnumerator FadeIn(string name, float fadeTime) => Fade(name, fadeTime, false);
    }
}
