using GameItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseToExplosion : MonoBehaviour
{
    [SerializeField] private ExplosionThrownBomb Parent;

    private void Awake()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.radius = Parent.ExplosionRadius;
    }

    public void OnTriggerStay(Collider other)
    {
        if (Parent.WasExploded && other.CompareTag("PartOfBody") && other.transform.parent.TryGetComponent(out PartsOfBody body))
        {
            body.CloseToExplosion(Parent.Bomb.Explode, Parent.Owner);
        }
    }
}
