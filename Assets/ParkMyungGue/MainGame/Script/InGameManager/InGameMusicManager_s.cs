using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public partial class InGameMusicManager_s : Singleton<InGameMusicManager_s>//data 
{
    [Header("fmod")]
    private EventInstance _curMusicEventInstance;
    private int _getPosition;
    [Header("data")]
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private int _bpm;
    public float secPerBeat { get; private set; }
    public float musicPosition { get; private set; }
    private float _musicPositionInBeats;
    public int completedLoops { get; private set; }
    private float _lastBeatCount;
    public float loopPositionInBeats { get; private set; }
    private float _pauseValue;
    private int _previousPausePosition;
    public bool isPause { get; private set; }
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        secPerBeat = 60f / _bpm;
        isPause = true;
    }
}
public partial class InGameMusicManager_s //main system
{
    public void InGameBind()
    {
        InGameFunBind_s.Instance.EgameStart += GameStart;
        InGameFunBind_s.Instance.EgamePlay += GamePlay;
        InGameFunBind_s.Instance.EgameEnd += GameEnd;
        InGameFunBind_s.Instance.Epause += AudioPause;
        InGameFunBind_s.Instance.EunPause += AudioUnPause;
    }
    public void GameStart()
    {
        musicPosition = 0;
        _musicPositionInBeats = 0;
        completedLoops = 0;
        _lastBeatCount = 0;
        loopPositionInBeats = 0;
        _pauseValue = 0;
        _previousPausePosition = 0;
    }
    public void GamePlay()
    {
        AudioPlay(_musicClip); //need last
    }
    public void GameEnd()
    {
        AudioStop(_musicClip);//need first   
    }
}
public partial class InGameMusicManager_s //game system
{
    private void Update()
    {
        if (!isPause)
        {
            _curMusicEventInstance.getTimelinePosition(out _getPosition);
            musicPosition = (_getPosition*0.001f) - _pauseValue;
            _musicPositionInBeats = musicPosition / secPerBeat;
            if (_musicPositionInBeats >= (completedLoops + 1))
            {
                completedLoops++;
                InGameBeatManager_s.Instance.NextBit();
                EnemyUI_s.Instance.PumpHP(InGameEnemy_s.Instance.curPumpTarget);
            }
            loopPositionInBeats = _musicPositionInBeats - completedLoops;
            if(loopPositionInBeats>InGameBeatManager_s.Instance.beatJudgeMax&&_lastBeatCount<completedLoops)
            {
                InGameManager_s.Instance.MoveNextBeat();
                _lastBeatCount = completedLoops;
            }
        }
    }
    public void AudioPlay(AudioClip clip)
    {
        GetComponent<StudioEventEmitter>().Play();
        isPause = false;
        _curMusicEventInstance = GetComponent<StudioEventEmitter>().EventInstance;
    }
    public void AudioStop(AudioClip clip)
    {
        GetComponent<StudioEventEmitter>().Stop();
    }
    public void AudioPause()
    {
        _previousPausePosition = _getPosition;
        GetComponent<StudioEventEmitter>().EventInstance.setPaused(true);
        isPause = true ;
        Debug.Log("bb");
    }
    public void AudioUnPause()
    {
        GetComponent<StudioEventEmitter>().EventInstance.setTimelinePosition((_previousPausePosition));
        GetComponent<StudioEventEmitter>().EventInstance.setPaused(false);
        isPause = false;
        Debug.Log("aa");

    }
    public void ChangeVolume(float volume) { }
}
