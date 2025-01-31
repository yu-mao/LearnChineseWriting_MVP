using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneController_Jesus : MonoBehaviour
{
    // [Header("Temporary")]
    [SerializeField] private TextMeshProUGUI _debugText;

    [Header("Hand Setup")]
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _handSkeleton;

    [Header("Sample Characters")]
    [SerializeField] private CurrentSampleChineseCharacter_Jesus _currentSampleCharacter;
    [Tooltip("Base chinese word image")] public Image image;
    [Tooltip("Drawing to compare")] public RawImage rawImage;

    [Header("Visual Feedback")]
    [SerializeField] private UserWritingVisualFeedback _userWritingVisualFeedback;

    private bool _isWriting = false;
    private bool _wasWriting = false;
    private List<List<Vector3>> _userWrittenStrokes = new List<List<Vector3>>();
    private List<Vector3> _currentUserWrittenStroke = new List<Vector3>();
    private int _maxCountUserWrittenStrokes;
    private int _countUserWrittenStrokes = 0;
    private List<string> _sampleCharacters = new List<string> { "ai", "zhong", "wen" };
    private int _sampleCharacterIndex = 0;

    private bool canCompare;

    public void StartWriting()
    {
        _isWriting = true;
        _debugText.text = "isWriting";
    }

    public void EndWriting()
    {
        _isWriting = false;
        _currentUserWrittenStroke.Clear();
        _debugText.text = "";
    }

    public void ConfirmWriting()
    {
        if(canCompare)
        {
            canCompare = false;
            EraseUserWrittenStrokes();

            ImageComparison.instance.SetImageToCompare(image);
            ImageComparison.instance.SetRawImageToCompare(rawImage);
            Invoke("ConfirmCalification", 1.5f);
        }

        // TODO: evaluate user writing
        // TODO: randomize writing colour
    }

    private void ConfirmCalification()
    {
        ImageComparison.instance.CompareImages();
        UpdateCurrentSampleCharacter();
        canCompare = true;
    }

    public void RedoWriting()
    {
        EraseUserWrittenStrokes();
        _debugText.text = "RedoWriting";
    }

    private void Start()
    {
        _maxCountUserWrittenStrokes = _userWritingVisualFeedback.strokes.Count;
        UpdateCurrentSampleCharacter();
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

    private void UpdateCurrentSampleCharacter()
    {
        if (_sampleCharacterIndex >= _sampleCharacters.Count) _sampleCharacterIndex = 0;

        _currentSampleCharacter.UpdateCurrentSampleCharacter(_sampleCharacters[_sampleCharacterIndex]);
        _sampleCharacterIndex++;
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
