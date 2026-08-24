using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RandomAudio : MonoBehaviour
{
    [SerializeField] List<AudioResource> audioSources;
    [SerializeField] float pitchMin;
    [SerializeField] float pitchMax;

    AudioSource audioSource;

    public static Dictionary<string, RandomAudio> singletons;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (singletons == null)
        {
            singletons = new Dictionary<string, RandomAudio>();
        }

        singletons.Add(name, this);
    }

    public void Play()
    {
        audioSource.resource = audioSources[Random.Range(0, audioSources.Count)];
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.Play();
    }
}
