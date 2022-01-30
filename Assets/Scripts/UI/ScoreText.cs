using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;

    void Start()
    {
        _textMesh.text = $"Score: {ScoreManager.Instance.Score:n0}";
    }
}
