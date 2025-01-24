using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [Header("Temporary")]
    [SerializeField] private GameObject _tempIndexTipVis;
    [SerializeField] private TextMeshProUGUI _debugText;

    [Header("Hand Setup")]
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _handSkeleton;

    [Header("Visuals")]
    [SerializeField] private LineRenderer _strokeVisual;

    private bool _isWriting = false;
    private List<List<Vector3>> _userWrittenStrokes = new List<List<Vector3>>();
    private List<Vector3> _currentUserWrittenStroke = new List<Vector3>();

    public void StartWriting()
    {
        _isWriting = true;
        _tempIndexTipVis.SetActive(true);
    }

    public void EndWriting()
    {
        _isWriting = false;
        _currentUserWrittenStroke.Clear();
        _tempIndexTipVis.SetActive(false);
    }
    
    public void ConfirmWriting() {}
    
    public void RedoWriting() {}

    private void Update()
    {
        _debugText.text = "isWriting: " + _isWriting + " | strokeCount: " + _currentUserWrittenStroke.Count;
        if (_isWriting && _hand.IsTracked && _handSkeleton.IsInitialized)
        {
            _currentUserWrittenStroke.Add(GetIndexFingerTipPosition());
            VisualizeStroke(_currentUserWrittenStroke);
        }
    }

    private Vector3 GetIndexFingerTipPosition()
    {
        var indexTip = _handSkeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexTip];
        _tempIndexTipVis.transform.position = indexTip.Transform.position;
        return indexTip.Transform.position;
    }
    
    private void VisualizeStroke(List<Vector3> currentUserWrittenStroke)
    {
        _strokeVisual.positionCount = 0;
        _strokeVisual.SetPositions(currentUserWrittenStroke.ToArray());
        _strokeVisual.positionCount = currentUserWrittenStroke.Count;
    }
}
