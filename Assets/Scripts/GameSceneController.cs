using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    // [Header("Temporary")]
    // [SerializeField] private GameObject _tempIndexTipVis;
    [SerializeField] private TextMeshProUGUI _debugText;

    [Header("Hand Setup")]
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _handSkeleton;

    [Header("Visual Feedback")]
    [SerializeField] private UserWritingVisualFeedback _userWritingVisualFeedback;

    private bool _isWriting = false;
    private bool _wasWriting = false;
    private List<List<Vector3>> _userWrittenStrokes = new List<List<Vector3>>();
    private List<Vector3> _currentUserWrittenStroke = new List<Vector3>();
    private int _maxCountUserWrittenStrokes;
    private int _countUserWrittenStrokes;

    public void StartWriting()
    {
        _isWriting = true;
        // _tempIndexTipVis.SetActive(true);
    }

    public void EndWriting()
    {
        _isWriting = false;
        _currentUserWrittenStroke.Clear();
        // _tempIndexTipVis.SetActive(false);
    }

    public void ConfirmWriting()
    {
        // TODO: randomize writing colour
    }

    public void RedoWriting()
    {
        
    }

    private void Start()
    {
        _maxCountUserWrittenStrokes = _userWritingVisualFeedback.strokes.Count;
    }

    private void Update()
    {
        if (_hand.IsTracked && _handSkeleton.IsInitialized)
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

    private Vector3 GetIndexFingerTipPosition()
    {
        var indexTip = _handSkeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexTip];
        // _tempIndexTipVis.transform.position = indexTip.Transform.position;
        return indexTip.Transform.position;
    }
    
    private void UpdateUserWrittenStrokes(bool wasWriting)
    {
        if (wasWriting && _userWrittenStrokes.Count >= 1)
        {
            // if it's a continuous writing stoke 
            _userWrittenStrokes[_countUserWrittenStrokes] = _currentUserWrittenStroke;
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
        _debugText.text = "_countUserWrittenStrokes: " + _countUserWrittenStrokes;
    }
    
    private void VisualizeCurrentUserWrittenStroke(Vector3[] currentUserWrittenStroke)
    {
        _userWritingVisualFeedback.strokes[_countUserWrittenStrokes].positionCount = currentUserWrittenStroke.Length;
        _userWritingVisualFeedback.strokes[_countUserWrittenStrokes].SetPositions(currentUserWrittenStroke);
    }
}
