using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _health = 10f;
    [SerializeField]
    private InteractionRadius _interactionRadius;
    [SerializeField]
    private Animator _animator;
    private Coroutine LookCoroutine;

    private const string k_Attack = "Attack";

    private void Awake()
    {
        _interactionRadius.OnAttack += OnAttack;
    }

    private void OnAttack(IDamageable target)
    {
        _animator.SetTrigger(k_Attack);

        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
    }

    private IEnumerator LookAt(Transform target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);

        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = lookRotation;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health < 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
