using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurrentSampleChineseCharacter : MonoBehaviour
{
    [SerializeField] private List<Material> _sampleChineseCharacters;
    
    private MeshRenderer _meshRenderer;

    public void UpdateCurrentSampleCharacter(string pronounciation)
    {
        Material currentCharacter = _sampleChineseCharacters.Where(i => i.name.Contains(pronounciation)).FirstOrDefault();
        _meshRenderer.material = currentCharacter;
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
}
