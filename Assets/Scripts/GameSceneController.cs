using System;
using TMPro;
using UnityEngine;

public enum CustomBoneId
{
    // XRHand_IndexTip = OVRSkeleton.BoneId.XRHand_IndexTip,
    customIndexTip = OVRPlugin.BoneId.XRHand_IndexTip,
}

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private GameObject _tempIndexTipVis;
    [SerializeField] private TextMeshProUGUI _debugText;

    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _handSkeleton;
    private OVRBone _bone;
    
    public void WriteAStroke() {}
    
    public void ConfirmWriting() {}
    
    public void RedoWriting() {}

    private void Update()
    {
        if (_hand.IsTracked)
        {
            DisplayIndexFingerTip();
        }
    }

    private void DisplayIndexFingerTip()
    {
        var indexTip = _handSkeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexTip];
        _tempIndexTipVis.transform.position = indexTip.Transform.position;
    }
}
