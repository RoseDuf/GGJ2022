using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerCanvas : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Camera cameraToLookAt;

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

    // Update is called once per frame
    void Update()
    {
        FaceCamera();

        //if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Scratch1") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Scratch2") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Scratch3"))
        //{
        //    _image.enabled = false;
        //    _animator.enabled = false;
        //}

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

    void FaceCamera()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
