using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CurrentSampleChineseCharacter_Jesus : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sampleChineseCharacters;
    [SerializeField] private Image imageObject;

    //private MeshRenderer _meshRenderer;

    public void UpdateCurrentSampleCharacter(string pronounciation)
    {
        imageObject.sprite = _sampleChineseCharacters.Where(i => i.name.Contains(pronounciation)).FirstOrDefault();
    }

    //private void Awake()
    //{
    //    _meshRenderer = GetComponent<MeshRenderer>();
    //}
}
