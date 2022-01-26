using System.Collections;
using Game;
using UnityEngine;
using StarterAssets;

public class Player : DayNightSensibleMonoBehaviour, IDamageable
{
    [SerializeField]
    private float _health = 10f;
    [SerializeField]
    private InteractionRadius _interactionRadius;
    [SerializeField]
    private float _attackDelay;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private ThirdPersonController _controller;
    private Inventory _inventory;

    [SerializeField] private GameObject daydoggo;
    [SerializeField] private GameObject nightdoggo;

    private StarterAssetsInputs _input;
    private Coroutine LookCoroutine;
    private Coroutine AttackCoroutine;

    private const string k_Attack = "Attack";
    private const string k_Grab = "Grab";

    private bool _inputsActivated = true;

    private bool InputsActivated
    {
        get => _inputsActivated;
        set
        {
            _inputsActivated = value;
            if (_controller) _controller.enabled = value;
        }
    }

    private void Awake()
    {
        _interactionRadius.OnAttack += OnAttack;
        _interactionRadius.OnGrab += OnGrab;
        _input = GetComponent<StarterAssetsInputs>();
        _inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        if (!_inputsActivated)
            return;
        
        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
        {
            if (_interactionRadius.CanDoAction && _input.action)
            {
                _interactionRadius.GrabItem();
                _interactionRadius.GiveItem();
                _input.action = false;
            }
        }
        else if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            if (_interactionRadius.CanDoAction && AttackCoroutine == null && _input.action)
            {
                AttackCoroutine = StartCoroutine(_interactionRadius.Attack(InteractionRadius.AttackStyle.Singular, _attackDelay));
                _input.action = false;
            }

            if (!_interactionRadius.CanDoAction && AttackCoroutine != null)
            {
                AttackCoroutine = null;
            }
        }
    }

    private void OnGrab(IGrabable target)
    {
        _animator.SetTrigger(k_Grab);

        GameObject item = target.GetTransform().gameObject;

        _inventory.Food.Add(item);
        item.SetActive(false);
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
        var lookPos = target.position - transform.position;
        lookPos.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(lookPos);

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
        if (_health <= 0)
        {
            if (DaytimeManager.HasInstance)
                DaytimeManager.Instance.Stop();
            gameObject.SetActive(false);
        }
    }

    public void ReceiveItem()
    {
        //Nothing
    }

    public Transform GetTransform()
    {
        return transform;
    }

    protected override void OnDay(int dayNumber)
    {
        // TODO Switch visual to day visuals
    }

    protected override void OnNight(int nightNumber)
    {
        // TODO Switch visual to night visuals
    }

    protected override void OnDayNightTransitionStarted()
    {
        InputsActivated = false;
    }

    protected override void OnDayNightTransitionFinished()
    {
        InputsActivated = true;
    }
}
