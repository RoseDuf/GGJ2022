using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractionRadius : MonoBehaviour
{
    private List<IDamageable> Damageables = new List<IDamageable>();
    [SerializeField]
    private int _damage = 1;
    [SerializeField]
    private float _attackDelay = 0.5f;
    public delegate void AttackEvent(IDamageable target);
    public AttackEvent OnAttack;
    private Coroutine AttackCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null)
        {
            Damageables.Add(damageable);

            if (AttackCoroutine == null)
            {
                AttackCoroutine = StartCoroutine(Attack()); //TODO: change this to use input
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            Damageables.Remove(damageable);
            if (Damageables.Count == 0)
            {
                StopCoroutine(AttackCoroutine);
                AttackCoroutine = null;
            }
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(_attackDelay);

        yield return wait;

        IDamageable closestDamageable = null;
        float closestDistance = float.MaxValue;

        while (Damageables.Count > 0)
        {
            for(int i = 0; i < Damageables.Count; i++)
            {
                Transform damageableTransform = Damageables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTransform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDamageable = Damageables[i];
                }
            }

            if (closestDamageable != null)
            {
                OnAttack?.Invoke(closestDamageable);
                closestDamageable.TakeDamage(_damage);
            }

            closestDamageable = null;
            closestDistance = float.MaxValue;

            yield return wait;

            Damageables.RemoveAll(DisabledDamageables);
        }

        AttackCoroutine = null;
    }

    private bool DisabledDamageables(IDamageable damageable)
    {
        return damageable != null && !damageable.GetTransform().gameObject.activeSelf;
    }
}
