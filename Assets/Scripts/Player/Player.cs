using System.Collections;
using System.Linq;
using Game;
using UnityEngine;
using StarterAssets;

public class Player : DayNightSensibleMonoBehaviour, IDamageable
{
    [SerializeField] private float _health = 10f;
    private float _maxHealth = 10f;
    [SerializeField] private InteractionRadius _interactionRadius;
    [SerializeField] private float _attackDelay;
    private Animator _animator;
    private ThirdPersonController _controller;

    [SerializeField]
    private int _foodCapacity;
    public int FoodCapacity { get { return _foodCapacity; } }
    private UIInventory _uiInventory;
    private Inventory _inventory;

    [SerializeField] private GameObject daydoggo;
    [SerializeField] private GameObject nightdoggo;

    private StarterAssetsInputs _input;
    private Coroutine LookCoroutine;
    private Coroutine AttackCoroutine;

    private const string k_Attack = "Attack";
    private const string k_Grab = "Grab";

    private bool _inputsActivated = true;
    private UIManager _uiManager;

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
        _uiManager = UIManager.Instance;
    }

    private void Start()
    {
        _interactionRadius.OnAttack += OnAttack;
        _interactionRadius.OnGrab += OnGrab;
        _interactionRadius.OnGive += OnGive;
        _input = GetComponent<StarterAssetsInputs>();
        _inventory = new Inventory();

        _controller = GetComponent<ThirdPersonController>();
        _animator = GetComponentInChildren<Animator>();

        _maxHealth = _health;
        _uiManager.UpdatePlayerHealth(_health);
        _uiManager.UIinventory.SetInventory(_inventory);
    }

    private void Update()
    {
        if (_uiManager != null && _uiManager.GameIsPaused) return;

        if (!_inputsActivated)
            return;

        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
        {
            if (_interactionRadius.Grabables.Count > 0 && _input.action)
            {
                _interactionRadius.GrabItem();
                _input.action = false;
            }

            if (_interactionRadius.Damageables.Count > 0 && _input.action)
            {
                _interactionRadius.GiveItem();
                _input.action = false;
            }

            if (_inventory.Food.Count > 0 && _input.dash)
            {
                Food foodPoolable = _inventory.Food.Last();
                foodPoolable.Agent.enabled = true;
                foodPoolable.transform.position = transform.position;
                foodPoolable.transform.gameObject.SetActive(true);
                _inventory.RemoveFood(_inventory.Food.Last());
                _uiManager.UIinventory.UpdateInventory();
            }
        }
        else if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            if (_interactionRadius.Damageables.Count > 0 && AttackCoroutine == null && _input.action)
            {
                AttackCoroutine =
                    StartCoroutine(_interactionRadius.Attack(InteractionRadius.AttackStyle.Singular, _attackDelay));
                _input.action = false;
            }
            
            if (_interactionRadius.Damageables.Count == 0 && AttackCoroutine != null)
            {
                AttackCoroutine = null;
            }
        }

        if (_input.action || _input.dash)
        {
            _input.action = false;
            _input.dash = false;
        }
    }

    private void OnGrab(IGrabable target)
    {
        if (_inventory.Food.Count < _foodCapacity)
        {
            _animator.SetTrigger(k_Grab);

            Food item = target.GetTransform().gameObject.GetComponent<Food>();

            _inventory.AddFood(item);
            _uiManager.UIinventory.UpdateInventory();
            _interactionRadius.Grabables.Remove(target);
            target.GetTransform().gameObject.SetActive(false);
        }
    }

    private bool OnGive(IDamageable target)
    {
        if (_inventory.Food.Count > 0)
        {
            Villager villager = target.GetTransform().GetComponent<Villager>();

            if (villager.Fatness < villager.MaxFatness)
            {
                foreach (Food food in _inventory.Food)
                {
                    if (food.FoodData.TypeOfFood.ToString() == villager.Type.ToString())
                    {
                        _inventory.RemoveFood(food);
                        _uiManager.UIinventory.UpdateInventory();
                        return true;
                    }
                }

                return false;
            }
        }

        return false;
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

        _uiManager.UpdatePlayerHealth(_health / _maxHealth);
        if (_health <= 0)
        {
            if (DaytimeManager.HasInstance)
                DaytimeManager.Instance.Stop();
            
            gameObject.SetActive(false);
            //_animator.SetTrigger("Die");
            GameManager.Instance.PlayerDied();
        }
        
    }
    
    public void Heal(int healValue)
    {
        _health += healValue;

        _uiManager.UpdatePlayerHealth(_health / _maxHealth);
        if (_health > _maxHealth)
            _health = _maxHealth;
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
        daydoggo.SetActive(true);
        nightdoggo.SetActive(false);
    }

    protected override void OnNight(int nightNumber)
    {
        daydoggo.SetActive(false);
        nightdoggo.SetActive(true);
    }

    protected override void OnDayNightTransitionStarted()
    {
        InputsActivated = false;
        _animator.SetBool("IsDayNightTransition", true);
    }

    protected override void OnDayNightTransitionFinished()
    {
        InputsActivated = true;
        _animator.SetBool("IsDayNightTransition", false);
    }
}