using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.AI;

public class Villager : PoolableObject, IDamageable
{
    public enum FoodType
    {
        Apple,
        Fish,
        Cake,
        Cheese
    }

    public VillagerMovement Movement;
    public NavMeshAgent Agent;

    public UIArrow UIArrow { get; set; }

    [SerializeField] private MeshRenderer _hatMeshRenderer;
    [SerializeField] private MeshFilter _hatMeshFilter;
    
    [SerializeField]
    private FoodType _typeOfFood;
    [SerializeField]
    private InteractionRadius _interactionRadius;
    [SerializeField]
    private float _attackDelay;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _villagerModel;
    private Vector3 _scale;

    private Coroutine LookCoroutine;

    private SkinnedMeshRenderer _meshRenderer;
    
    public int Fatness { get; set; }
    [SerializeField]
    private int _maxFatness;
    public int MaxFatness { get; set; }
    public int Aggressivity;

    [SerializeField]
    private int _baseHealth;
    public int Health;

    [SerializeField] private ParticleSystem BloodFX;
    public bool IsDead { get; set; }

    public FoodType Type { get { return _typeOfFood; } set { _typeOfFood = value; } }

    private const string k_Attack = "Attack";

    private Coroutine AttackCoroutine;
    private bool _isMoving = true;

    private float deathLength;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _interactionRadius.OnAttack += OnAttack;
        _interactionRadius.OnGive += OnGive;
        
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            if (clip.name == "Armature|Die")
            {
                deathLength = clip.length;
            }
        }
    }

    public void PlayHappyAnimation()
    {
        _animator.SetTrigger("Happy");
    }

    void Start()
    {
        InitializeVillager();
        
        UIArrow = GetComponentInChildren<UIArrow>();
        Fatness = 1;
        MaxFatness = _maxFatness;
        _meshRenderer.SetBlendShapeWeight(0, Fatness);
        Aggressivity = 1; 
        Health = _baseHealth;
        IsDead = false;

        if (GameManager.HasInstance)
            UpdateStatsForDay(GameManager.Instance.CurrentDay);
    }

    private void InitializeVillager()
    {
        Material[] newMaterials = new Material[1];
        newMaterials[0] = VillagerDatabase.Instance.VillagerData.Find(x => x.TypeOfFood.ToString() == _typeOfFood.ToString()).Material;
        _meshRenderer.materials = newMaterials;
        
        var foodData = FoodDatabase.Instance.FoodData.Find(x => x.TypeOfFood.ToString() == _typeOfFood.ToString());
        _hatMeshRenderer.material = foodData.Material;
        _hatMeshFilter.mesh = foodData.Mesh;
        _hatMeshFilter.transform.eulerAngles = foodData.Rotation;
        _hatMeshFilter.transform.localScale = new Vector3(foodData.Scale, foodData.Scale, foodData.Scale) / 1000;
    }

    private bool OnGive(IDamageable target)
    {
        LookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
        return true;
    }

    private bool attackRight = true;

    public void DebugAttack()
    {
        OnAttack(null);
    }
    
    private void OnAttack(IDamageable target)
    {
        if (Aggressivity > 1)
        {
            if (attackRight)
                _animator.SetTrigger("AttackRight");
            else
                _animator.SetTrigger("AttackLeft");

            attackRight = !attackRight;

            if (LookCoroutine != null)
            {
                StopCoroutine(LookCoroutine);
            }

            LookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
        }
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

    public override void OnDisable()
    {
        base.OnDisable();
        Agent.enabled = false;
    }

    private void Update()
    {
        if (!_isMoving)
            return;

        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day && AttackCoroutine != null)
        {
            StopCoroutine(AttackCoroutine);
            AttackCoroutine = null;
        }

        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            if (_interactionRadius.Damageables.Count > 0 && AttackCoroutine == null)
            {
                AttackCoroutine = StartCoroutine(_interactionRadius.Attack(InteractionRadius.AttackStyle.Repeat, _attackDelay));
            }
            if (_interactionRadius.Damageables.Count == 0 && AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
                AttackCoroutine = null;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0) //Dies
        {
            IsDead = true;
            _animator.SetTrigger("Die");
            StartCoroutine(WaitForTimeBeforeDying(deathLength));
        }
    }

    private IEnumerator WaitForTimeBeforeDying(float time)
    {
        BloodFX.Play(true);
        yield return new WaitForSeconds(time);
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        // TODO Play FX
        if (BloodFX != null)
        {
            StartCoroutine(WaitBeforeDisableVillager(BloodFX.GetComponent<ParticleSystem>().main.startLifetime.constantMax));
        }
        else
        {
            Debug.LogWarning("BloodFX not link with Villager Script");
        }
        
        AddScoreFromDeath();
    }

    private IEnumerator WaitBeforeDisableVillager(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    private void AddScoreFromDeath()
    {
        if (!ScoreManager.HasInstance)
            return;
        
        ScoreManager.Instance.AddScoreForKilling(Fatness);
    }

    public void ReceiveItem()
    {
        if (Fatness < MaxFatness)
        {
            _animator.SetTrigger("Happy");
            Fatness += 1;
            _villagerModel.localScale = new Vector3(_villagerModel.localScale.x + 0.2f, _villagerModel.localScale.y + 0.2f, _villagerModel.localScale.z + 0.2f);
            //_meshRenderer.SetBlendShapeWeight(0, Fatness * 50);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void UpdateStatsForDay(int dayNumber)
    {
        var stats = DayStatsSystem.Instance.GetForDay(dayNumber);
        Aggressivity = (int) stats.BaseAggressivity;
        _interactionRadius.Damage = Aggressivity - 1;
    }

    public void StopMoving()
    {
        Agent.SetDestination(transform.position);
        Movement.enabled = false;
        _isMoving = false;
        Agent.enabled = false;
    }

    public void ResumeMoving()
    {
        Movement.enabled = true;
        _isMoving = true;
        Agent.enabled = true;
    }
}
