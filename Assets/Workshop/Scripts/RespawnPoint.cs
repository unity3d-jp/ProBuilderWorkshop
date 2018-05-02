using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RespawnPoint : MonoBehaviour
{
    [SerializeField] ParticleSystem emitter;
    bool isActive = false;

    private void Start()
    {
        emitter.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isActive) return;

        if (other.CompareTag("Player"))
        {
            RespawnManager manager = FindObjectOfType<RespawnManager>();
            manager.currentPoint = this;
            SetActive();
        }
    }

    private void SetActive()
    {
        isActive = true;
        emitter.gameObject.SetActive(true);

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();

        foreach (var p in particles)
        {
            p.Stop();
            p.Play();
            p.startColor = new Color(1, 0.8f, 0, p.startColor.a + 0.6f);
        }
    }
}
