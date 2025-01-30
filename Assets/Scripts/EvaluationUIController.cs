using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _evaluationUIs;

    private int _currentUIIndex = 0;

    public void ShowEvaluationUI()
    {
        StartCoroutine(ShowCurrentEvaluationUI());
    }

    private void HideAllEvaluationUIs()
    {
        foreach (var ui in _evaluationUIs)
        {
            ui.SetActive(false);
        }
    }

    private IEnumerator ShowCurrentEvaluationUI()
    {
        _evaluationUIs[_currentUIIndex].SetActive(true);
        yield return new WaitForSeconds(3.5f);
        
        _evaluationUIs[_currentUIIndex].SetActive(false);
        _currentUIIndex++;
        if (_currentUIIndex >= _evaluationUIs.Count) _currentUIIndex = 0;
        
        yield break;
    }

    private void Start()
    {
        HideAllEvaluationUIs();
    }
}
