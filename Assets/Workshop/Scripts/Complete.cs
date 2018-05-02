using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Complete : MonoBehaviour
{
    [SerializeField] Animator princess;
    [SerializeField] ParticleSystem thankyou;

    private void Start()
    {
        thankyou.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        princess.Play("GiveFlowers");
        thankyou.gameObject.SetActive(true);
    }
}
