using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public RespawnPoint currentPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetPlayerPosition(other.transform);
        }
    }

    public void SetPlayerPosition(Transform player)
    {
        player.position = currentPoint.transform.position;
        player.rotation = currentPoint.transform.rotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
