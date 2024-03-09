using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField] private Player Player;

    public void PutUpFlag(object obj)
    {
        foreach (Transform transform in Player.transform)
            if (transform.CompareTag("Player")) transform.gameObject.SetActive(false);

        Vector3 pos = transform.position;
        pos.y = 2;
        transform.position = pos;
        gameObject.SetActive(true);
        transform.rotation = Quaternion.Euler(-110, -47, -3);

        Player.TargetPosition = transform.position;
    }

    public void Revival(object obj)
    {
        foreach (Transform transform in Player.transform)
        {
            if (transform.CompareTag("Player")) transform.gameObject.SetActive(true);
            if (transform.CompareTag("Respawn")) Destroy(transform.gameObject);
        }
        gameObject.SetActive(false);
    }
}
