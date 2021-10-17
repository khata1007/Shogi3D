using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Choose
{
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] AudioClip[] audioClips = new AudioClip[3];
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void play(int id)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = audioClips[id];
            audioSource.Play();
        }

        public void stop()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.Stop();
        }
    }

}