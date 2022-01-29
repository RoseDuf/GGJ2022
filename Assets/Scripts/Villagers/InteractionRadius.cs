using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(SphereCollider))]
public class InteractionRadius : MonoBehaviour
{
    public List<IDamageable> Damageables = new List<IDamageable>();
    public List<IGrabable> Grabables = new List<IGrabable>();
    [SerializeField]
    private int _damage = 1;
    [SerializeField]
    private float _attackDelay = 0.5f;
    public delegate void AttackEvent(IDamageable target);
    public AttackEvent OnAttack;
    public delegate bool GiveEvent(IDamageable target);
    public GiveEvent OnGive;
    public delegate void GrabEvent(IGrabable target);
    public GrabEvent OnGrab;

    private void OnTriggerStay(Collider other)
    {
        if ((transform.tag == "Target" && other.tag == "Player") || transform.tag != "Target")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && !Damageables.Contains(damageable))
            {
                Damageables.Add(damageable);
            }
        }
        

        IGrabable grabable = other.GetComponent<IGrabable>();
        if (grabable != null && !Grabables.Contains(grabable))
        {
            Grabables.Add(grabable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((transform.tag == "Target" && other.tag == "Player") || transform.tag != "Target")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damageables.Remove(damageable);
            }
        }

        IGrabable grabable = other.GetComponent<IGrabable>();
        if (grabable != null)
        {
            Grabables.Remove(grabable);
        }
    }

    public void GrabItem()
    {
        IGrabable closestGrabable = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < Grabables.Count; i++)
        {
            Transform grabableTransform = Grabables[i].GetTransform();
            float distance = Vector3.Distance(transform.position, grabableTransform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGrabable = Grabables[i];
            }
        }

        if (closestGrabable != null)
        {
            OnGrab?.Invoke(closestGrabable);
        }

        closestGrabable = null;
        closestDistance = float.MaxValue;
    }

    public void GiveItem()
    {
        IDamageable closestDamageable = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < Damageables.Count; i++)
        {
            Transform damageableTransform = Damageables[i].GetTransform();
            float distance = Vector3.Distance(transform.position, damageableTransform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestDamageable = Damageables[i];
            }
        }

        if (closestDamageable != null && OnGive != null)
        {
            bool success = OnGive.Invoke(closestDamageable);

            if (success)
            {
                closestDamageable.ReceiveItem();
            }
        }

        closestDamageable = null;
        closestDistance = float.MaxValue;
    }
    
    public IEnumerator Attack(AttackStyle style, float delay)
    {
        WaitForSeconds wait = new WaitForSeconds(delay);

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

            if (style == AttackStyle.Singular)
            {
                break;
            }
        }
    }

    private bool DisabledDamageables(IDamageable damageable)
    {
        return damageable != null && !damageable.GetTransform().gameObject.activeSelf;
    }

    public enum AttackStyle
    {
        Repeat,
        Singular
    }
}
