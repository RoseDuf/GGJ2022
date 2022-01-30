using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFood : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Camera cameraToLookAt;

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
    }

    public void ShowArrow(bool canShow)
    {
        _image.enabled = canShow;
    }

    public bool IsShowing { get { return _image.enabled; } }

    void FaceCamera()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
