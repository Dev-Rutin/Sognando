using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySceneManager : Singleton<StorySceneManager>
{
    [SerializeField] private GameObject _fadePenal;
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _NPC;
    private string[] _dialogStrings;

    private bool _isStoryStart;
    // Start is called before the first frame update
    void Start()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
