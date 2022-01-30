using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;

public class UIPlayerCanvas : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Camera cameraToLookAt;

    [SerializeField]
    private TextMeshProUGUI _textMeshPro;

    [SerializeField]
    private Animator _animator;
    
    // Start is called before the first frame update
    void Awake()
    {
        _image.enabled = false;

        if (cameraToLookAt == null)
        {
            cameraToLookAt = Camera.main;
        }
    }

    private void Start()
    {
        ScoreManager.Instance.OnScoreAdded += OnScoreAdded;
    }

    private void OnDestroy()
    {
        if (ScoreManager.HasInstance)
            ScoreManager.Instance.OnScoreAdded -= OnScoreAdded;
    }

    // Update is called once per frame
    void Update()
    {
        FaceCamera();
    }

    public void ShowScratch(int sratchNumber)
    {
        _image.enabled = sratchNumber == 1 || sratchNumber == 2 || sratchNumber == 3;
        _animator.enabled = _image.enabled;

        switch (sratchNumber)
        {
            case 1:
                _animator.SetTrigger("Scratch1");
                break;
            case 2:
                _animator.SetTrigger("Scratch2");
                break;
            case 3:
            default:
                _animator.SetTrigger("Scratch3");
                break;
        }
    }

    private void OnScoreAdded()
    {
        GivePoints(ScoreManager.Instance.CurrentPoints);
    }

    public void GivePoints(float amount)
    {
        StartCoroutine(DisplayTextCoroutine(amount));
    }

    private IEnumerator DisplayTextCoroutine(float amount)
    {
        if (amount > 0)
        {
            _textMeshPro.text = amount.ToString();
        }
        yield return new WaitForSeconds(1f);
        _textMeshPro.text = "";
    }

    void FaceCamera()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
