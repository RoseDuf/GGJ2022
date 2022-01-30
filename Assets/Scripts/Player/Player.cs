using System.Collections;
using System.Linq;
using Game;
using UnityEngine;
using StarterAssets;

public class Player : DayNightSensibleMonoBehaviour, IDamageable
{
    [SerializeField]
    private UIPlayerCanvas _uiPlayerCanvas;
    [SerializeField] private float _health = 10f;
    private float _maxHealth = 10f;
    [SerializeField] private InteractionRadius _interactionRadius;
    [SerializeField] private ParticleSystem _bloodFX;
    [SerializeField] private float _attackDelay;
    private ThirdPersonController _controller;
    private Animator _animator;

    [SerializeField]
    private int _foodCapacity;
    public int FoodCapacity { get { return _foodCapacity; } }
    private UIInventory _uiInventory;
    private Inventory _inventory;

    [SerializeField] private GameObject daydoggo;
    [SerializeField] private GameObject nightdoggo;

    public Animator DayAnimator { get { return daydoggo.GetComponent<Animator>(); } }
    public Animator NightAnimator { get { return nightdoggo.GetComponent<Animator>(); } }

    private StarterAssetsInputs _input;
    private Coroutine LookCoroutine;
    private Coroutine AttackCoroutine;

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
        _interactionRadius.Damage = 10;
        _input = GetComponent<StarterAssetsInputs>();
        _inventory = new Inventory();
        _animator = daydoggo.GetComponent<Animator>();

        _controller = GetComponent<ThirdPersonController>();

        _maxHealth = _health;
        _uiManager.UpdatePlayerHealth(_health);
        _uiManager.UIinventory.SetInventory(_inventory);

        DaytimeManager.Instance.OnTimeOfDayChanged += OnTimeOfDayChanged;
    }

    protected void OnDestroy()
    {
        if (DaytimeManager.HasInstance)
        {
            DaytimeManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;
        }
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
            }

            if (_interactionRadius.Damageables.Count > 0 && _input.action)
            {
                _interactionRadius.GiveItem();
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

            if (AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
                AttackCoroutine = null;
            }

            if (_input.dash)
            {
                _input.dash = false;
            }
        }
        else if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            if (_interactionRadius.Damageables.Count > 0 && AttackCoroutine == null && _input.action)
            {
                AttackCoroutine =
                    StartCoroutine(_interactionRadius.Attack(InteractionRadius.AttackStyle.Singular, _attackDelay));
            }
            
            if (_interactionRadius.Damageables.Count == 0 && AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
                AttackCoroutine = null;
            }

            if (_inventory.Food.Count > 0)
            {
                _inventory.EmptyInventory();
                _uiManager.UIinventory.UpdateInventory();
            }
        }

        if (_input.action)
        {
            _input.action = false;
        }
    }

    private void OnTimeOfDayChanged(DaytimeManager.TimeOfDay timeOfDay)
    {
        if (timeOfDay == DaytimeManager.TimeOfDay.Day)
        {
            _animator.SetBool("IsDayNightTransition", true);
            _animator = DayAnimator;
        }
        if (timeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            _animator.SetBool("IsDayNightTransition", true);
            _animator = NightAnimator;
        }
    }

    private void OnGrab(IGrabable target)
    {
        if (_inventory.Food.Count < _foodCapacity)
        {
            Food item = target.GetTransform().gameObject.GetComponent<Food>();
            
            SoundSystem.Instance.PlayPickupFoodSound(gameObject);

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
                        // SoundSystem.Instance.PlayGiveFoodSound(gameObject); Overwhelming?
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
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        int randAnimation = Random.Range(0, 3);
        
        SoundSystem.Instance.PlayWolfAttackSound(gameObject);

        switch(randAnimation)
        {
            case 0:
                _animator.SetTrigger("AttackRight");
                _uiPlayerCanvas.ShowScratch(1);
                break;
            case 1:
                _animator.SetTrigger("AttackLeft");
                _uiPlayerCanvas.ShowScratch(3);
                break;
            case 2:
            default:
                _animator.SetTrigger("AttackJump");
                _uiPlayerCanvas.ShowScratch(2);
                break;
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
        if (damage > 0)
        {
            _animator.SetTrigger("Hurt");
        }

        _health -= damage;
        SoundSystem.Instance.PlayWolfHurtSound(gameObject);

        _uiManager.UpdatePlayerHealth(_health / _maxHealth);
        if (_health <= 0)
        {
            if (DaytimeManager.HasInstance)
                DaytimeManager.Instance.Stop();

            if (_bloodFX)
            {
                _bloodFX.transform.parent = null;
                _bloodFX.Play();
            }
            gameObject.SetActive(false);
            _animator.SetTrigger("Die");
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