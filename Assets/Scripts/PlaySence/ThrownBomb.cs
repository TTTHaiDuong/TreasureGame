using GameItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrownBomb : MonoBehaviour
{
    [SerializeField] private Explosion Explosion;
    [SerializeField] private Image Image;
    [SerializeField] private ExplosionThrownBomb Parent;

    private void Update()
    {
        Image.transform.forward = Camera.main.transform.forward;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PartOfBody") && !Parent.Owner.Body.IsThisChild(other.gameObject)) OnDestroy();
    }

    public void OnDestroy()
    {
        Explosion.Play(Explosion.Duration / 2);
        Parent.InfluencePlayers();
    }
}
