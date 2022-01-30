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

    [SerializeField] private List<MeshRenderer> _hatMeshRenderers;
    [SerializeField] private List<MeshFilter> _hatMeshFilters;
    
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
    
    [SerializeField]
    private int _maxFatness;
    
    [SerializeField]
    private int _baseHealth;

    private Coroutine LookCoroutine;

    private SkinnedMeshRenderer _meshRenderer;

    private int _fatness = 1;
    
    public int Fatness
    {
        get => _fatness;
        set
        {
            _fatness = value;
            UpdateHats();
        } 
    }
    public int MaxFatness { get; set; }
    
    public int Aggressivity;
    public int Health;

    [SerializeField] private ParticleSystem BloodFX;
    [SerializeField] private ParticleSystem HeartFX;
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

        OriginalScale = _villagerModel.localScale;
    }

    public void UpdateHats()
    {
        var i = 0;
        for (; i < _hatMeshFilters.Count && i < Fatness; ++i)
        {
            _hatMeshFilters[i].gameObject.SetActive(true);
        }

        for (; i < _hatMeshFilters.Count; ++i)
        {
            _hatMeshFilters[i].gameObject.SetActive(false);
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
        MaxFatness = _maxFatness;
        Aggressivity = 1; 
        Health = _baseHealth;

        if (GameManager.HasInstance)
            UpdateStatsForDay(GameManager.Instance.CurrentDay);
    }

    public void InitializeVillager()
    {
        _villagerModel.localScale = Vector3.one;
        Fatness = 1;
        _meshRenderer.SetBlendShapeWeight(0, Fatness);
        IsDead = false;

        Material[] newMaterials = new Material[1];
        var villagerData = VillagerDatabase.Instance.VillagerData.Find(x => x.TypeOfFood.ToString() == _typeOfFood.ToString());
        newMaterials[0] = villagerData.Material;
        _meshRenderer.materials = newMaterials;
        
        var foodData = FoodDatabase.Instance.FoodData.Find(x => x.TypeOfFood.ToString() == _typeOfFood.ToString());
        for (var i = 0; i < _hatMeshRenderers.Count; i++)
        {
            _hatMeshRenderers[i].material = foodData.Material;
            
            var postition = _hatMeshRenderers[i].transform.localPosition;
            postition.y = _hatMeshRenderers[0].transform.localPosition.y + i * villagerData.DistanceBetweenHats;
            _hatMeshRenderers[i].transform.localPosition = postition;
        }
        foreach (var meshFilter in _hatMeshFilters)
        {
            meshFilter.mesh = foodData.Mesh;
            meshFilter.transform.eulerAngles = foodData.Rotation;
            meshFilter.transform.localScale = new Vector3(foodData.Scale, foodData.Scale, foodData.Scale) / 1000;
        }
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
            SoundSystem.Instance.PlayVillagerAttackSound(gameObject);

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
            if (IsDead) 
                return;
            
            IsDead = true;
            _animator.SetTrigger("Die");
            StartCoroutine(WaitForTimeBeforeDying(deathLength));

            foreach (var hat in _hatMeshRenderers)
            {
                hat.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator WaitForTimeBeforeDying(float time)
    {
        BloodFX.Play(true);
        yield return new WaitForSeconds(time);
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        SoundSystem.Instance.PlayVillagerDeathSound(gameObject);
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
            HeartFX.Play(true);
            _animator.SetTrigger("Happy");
            SoundSystem.Instance.PlayVillagerJoySound(gameObject);
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
