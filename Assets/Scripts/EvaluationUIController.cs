using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _evaluationUIs;

    private int _currentUIIndex = 0;

    public void ShowEvaluationUI()
    {
        _evaluationUIs[_currentUIIndex].SetActive(true);
        _currentUIIndex++;
        if (_currentUIIndex >= _evaluationUIs.Count) _currentUIIndex = 0;
    }

    public void HideAllEvaluationUIs()
    {
        foreach (var ui in _evaluationUIs)
        {
            ui.SetActive(false);
        }
    }
    
    private void Start()
    {
        HideAllEvaluationUIs();
    }
}
