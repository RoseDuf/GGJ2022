using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(SphereCollider))]
public class InteractionRadius : MonoBehaviour
{
    public List<IDamageable> Damageables = new List<IDamageable>();
    public List<IGrabable> Grabables = new List<IGrabable>();
    public int Damage { get; set; }
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
                Villager villager;
                damageable.GetTransform().TryGetComponent<Villager>(out villager);
                if (villager == null || (villager != null && !villager.IsDead))
                {
                    Damageables.Add(damageable);
                }
            }
        }
        

        IGrabable grabable = other.GetComponent<IGrabable>();
        if (grabable != null && !Grabables.Contains(grabable))
        {
            Food food;
            grabable.GetTransform().TryGetComponent<Food>(out food);
            if (food != null)
            {
                food.GetUIFood.ShowArrow(true);
            }

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
            Food food;
            grabable.GetTransform().TryGetComponent<Food>(out food);
            if (food != null)
            {
                food.GetUIFood.ShowArrow(false);
            }

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

        if (style == AttackStyle.Singular)
        {
            if (closestDamageable != null)
            {
                OnAttack?.Invoke(closestDamageable);
                yield return wait;
                closestDamageable.TakeDamage(Damage);

                Villager villager;
                closestDamageable.GetTransform().TryGetComponent<Villager>(out villager);
                if (villager == null || (villager != null && villager.IsDead))
                {
                    Damageables.Remove(closestDamageable);
                }
            }
        }
        else if(style == AttackStyle.Repeat)
        {
            yield return wait;

            while (style == AttackStyle.Repeat)
            {
                if (closestDamageable != null)
                {
                    OnAttack?.Invoke(closestDamageable);
                    closestDamageable.TakeDamage(Damage);
                }

                yield return wait;

                Damageables.RemoveAll(DisabledDamageables);

                if (style == AttackStyle.Singular)
                {
                    break;
                }
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
