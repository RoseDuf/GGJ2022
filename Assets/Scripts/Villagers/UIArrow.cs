using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArrow : MonoBehaviour
{
    [SerializeField]
    private RawImage _arrow;

    [SerializeField]
    private Camera cameraToLookAt;


    // Start is called before the first frame update
    void Awake()
    {
        _arrow.enabled = false;

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

    public void ShowArrow(bool show)
    {
        _arrow.enabled = show;
    }

    void FaceCamera()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
