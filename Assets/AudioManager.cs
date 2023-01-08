using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    AudioClip nightClip;
    [SerializeField]
    AudioClip dayClip;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayNight()
    {
        audioSource.Stop();
        audioSource.clip = nightClip;
        audioSource.Play();
    }
    public void PlayDay()
    {
        audioSource.Stop();
        audioSource.clip = dayClip;
        audioSource.Play();
    }
    public void StopPlaying()
    {
        audioSource.Stop();
    }
}
