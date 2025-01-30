using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GameSceneController : MonoBehaviour
{
    public bool isWritingActivated = false;
    
    [Header("Temporary")]
    [SerializeField] private TextMeshProUGUI _debugText;

    [Header("Hand Setup")]
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _handSkeleton;
    
    [Header("Sample Characters")]
    [SerializeField] private CurrentSampleChineseCharacter _currentSampleCharacter;

    [Header("Visual Feedback")]
    [SerializeField] private UserWritingVisualFeedback _userWritingVisualFeedback;

    [Header("User Writing Evaluation")]
    [SerializeField] private EvaluationUIController _evaluationUI;

    private bool _isWriting = false;
    private bool _wasWriting = false;
    private List<List<Vector3>> _userWrittenStrokes = new List<List<Vector3>>();
    private List<Vector3> _currentUserWrittenStroke = new List<Vector3>();
    private int _maxCountUserWrittenStrokes;
    private int _countUserWrittenStrokes = 0;
    private List<string> _sampleCharacters = new List<string> { "ai", "zhong", "wen" };
    private int _sampleCharacterIndex = 0;

    public void ActivateWriting()
    {
        isWritingActivated = true;
        _currentSampleCharacter.gameObject.SetActive(true);
    }

    public void DeactivateWriting()
    {
        isWritingActivated = false;
        _currentSampleCharacter.gameObject.SetActive(false);
    }
    
    public void StartWriting()
    {
        _isWriting = true;
    }

    public void EndWriting()
    {
        _isWriting = false;
        _currentUserWrittenStroke.Clear();
    }

    public void ConfirmWriting()
    {
        if (!isWritingActivated) return;

        StartCoroutine(AsyncConfirmWriting());

        // TODO: randomize writing colour
    }

    public void RedoWriting()
    {
        EraseUserWrittenStrokes();
    }

    private void Start()
    {
        _maxCountUserWrittenStrokes = _userWritingVisualFeedback.strokes.Count;
        _currentSampleCharacter.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_hand.IsTracked && _handSkeleton.IsInitialized && isWritingActivated)
        {
            if (_isWriting)
            {
                _currentUserWrittenStroke.Add(GetIndexFingerTipPosition());
                
                UpdateUserWrittenStrokes(_wasWriting);
                VisualizeCurrentUserWrittenStroke(_currentUserWrittenStroke.ToArray());
            }
        }
        _wasWriting = _isWriting;
    }
    
    private IEnumerator AsyncConfirmWriting()
    {
        EraseUserWrittenStrokes();
        
        // display evaluation UI
        if (_currentSampleCharacter.gameObject.activeSelf)
        {
            _currentSampleCharacter.gameObject.SetActive(false);
            
            _evaluationUI.ShowEvaluationUI();
            yield return new WaitForSeconds(3f);
            _evaluationUI.HideAllEvaluationUIs();
        }
        
        // display next sample Chinese character
        UpdateCurrentSampleCharacter();
        _currentSampleCharacter.gameObject.SetActive(true);
        yield return _currentSampleCharacter.gameObject.transform.DOShakePosition(0.5f, 0.5f, 15)
            .SetEase(Ease.InOutQuad).WaitForCompletion();
    }

    private void UpdateCurrentSampleCharacter()
    {
        _currentSampleCharacter.UpdateCurrentSampleCharacter(_sampleCharacters[_sampleCharacterIndex]);
        _sampleCharacterIndex++;
        if (_sampleCharacterIndex >= _sampleCharacters.Count) _sampleCharacterIndex = 0;
    }

    private Vector3 GetIndexFingerTipPosition()
    {
        var indexTip = _handSkeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexTip];
        return indexTip.Transform.position;
    }
    
    private void UpdateUserWrittenStrokes(bool wasWriting)
    {        
        if (wasWriting && _userWrittenStrokes.Count >= 1)
        {
            // if it's a continuous writing stoke 
            _userWrittenStrokes[_countUserWrittenStrokes - 1] = _currentUserWrittenStroke;
        }
        else
        {
            // if it's a new writing stroke
            _userWrittenStrokes.Add(_currentUserWrittenStroke);
            if (_countUserWrittenStrokes < _maxCountUserWrittenStrokes)
            {
                _countUserWrittenStrokes += 1;
            }
            else
            {
                _countUserWrittenStrokes = 0;
            }
        }
    }
    
    private void VisualizeCurrentUserWrittenStroke(Vector3[] currentUserWrittenStroke)
    {
        _userWritingVisualFeedback.strokes[_countUserWrittenStrokes].positionCount = currentUserWrittenStroke.Length;
        _userWritingVisualFeedback.strokes[_countUserWrittenStrokes].SetPositions(currentUserWrittenStroke);
    }

    private void EraseUserWrittenStrokes()
    {
        _userWrittenStrokes.Clear();
        _countUserWrittenStrokes = 0;

        foreach (var lineRenderer in _userWritingVisualFeedback.strokes)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
