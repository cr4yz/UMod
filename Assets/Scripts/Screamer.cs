using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Screamer : MonoBehaviour
{

    [SerializeField]
    private Rigidbody _rb;
    private AudioSource _audioSource;

    private float _volModifier = 1.0f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.pitch = Random.Range(.75f, 1.25f);
        _audioSource.Stop();
        _volModifier = Random.Range(.5f, 1f);

        Invoke("PlayAudioSource", Random.Range(0, 2f));
    }

    private void Update()
    {
        var maxVel = 20;
        var curVel = _rb.velocity.magnitude;
        var alpha = curVel / maxVel;
        _audioSource.volume = Mathf.Lerp(_audioSource.volume, alpha, .5f) * _volModifier;
    }

    private void PlayAudioSource()
    {
        _audioSource.Play();
    }

}
