using FMOD.Studio;
using FMODUnity;
using UnityEditor.Rendering;
using UnityEngine;

public partial class InGameMusicManager_s : Singleton<InGameMusicManager_s>//data 
{
    [Header("fmod")]
    private FMOD.ChannelGroup _channel;
    private ulong _dspClock, _parentClock;
    [Header("data")]
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private int _bpm;
    public float secPerBeat { get; private set; }
    public float musicPosition { get; private set; }
    private float _musicPositionInBeats;
    private float _dspMusicStartPosition;
    public int completedLoops { get; private set; }
    private float _lastBeatCount;
    public float loopPositionInBeats { get; private set; }
    private WaitForEndOfFrame _waitUpdate;
    //pause
    private float _pausePosition;
    private float _pauseStartDspPosition;
    private float _totalPauseValue;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        secPerBeat = 60f / _bpm;
        _waitUpdate = new WaitForEndOfFrame();
        //FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out _channel);
    }
}
public partial class InGameMusicManager_s //main system
{
    public void InGameBind()
    {
        InGameFunBind_s.Instance.EgameStart += GameStart;
        InGameFunBind_s.Instance.EgamePlay += GamePlay;
        InGameFunBind_s.Instance.EgameEnd += GameEnd;
    }
    public void GameStart()
    {
        musicPosition = 0;
        _musicPositionInBeats = 0;
        _dspMusicStartPosition = 0;
        completedLoops = 0;
        _lastBeatCount = 0;
        loopPositionInBeats = 0;
        _pausePosition = 0;
        _pauseStartDspPosition = 0;
        _totalPauseValue = 0;
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
        if (InGameManager_s.Instance.curGameStatus == EGameStatus.PLAYING)
        {
            //_channel.getDSPClock(out _dspClock, out _parentClock);
            Debug.Log(AudioSettings.dspTime);
            musicPosition = (float)(AudioSettings.dspTime - _dspMusicStartPosition - _totalPauseValue);
            Debug.Log(musicPosition);
            _musicPositionInBeats = musicPosition / secPerBeat;
            if (_musicPositionInBeats >= (completedLoops + 1))
            {
                //Debug.Log("music"+_scripts._inGameMusicManager_s.musicPosition);
                completedLoops++;
                InGameBeatManager_s.Instance.NextBit();
            }
            loopPositionInBeats = _musicPositionInBeats - completedLoops;
            if(loopPositionInBeats>InGameBeatManager_s.Instance.beatJudgeMax&&_lastBeatCount<completedLoops)
            {
                InGameManager_s.Instance.MoveNextBeat();
                _lastBeatCount = completedLoops;
            }
        }
    }

}
public partial class InGameMusicManager_s //music manage
{
    public void AudioPlay(AudioClip clip)
    {
        // _channel.getDSPClock(out _dspClock, out _parentClock);
        //_dspMusicStartPosition =  _dspClock;
        //GetComponent<StudioEventEmitter>().Play();
        _audioSource.clip = _musicClip;
        _audioSource.Play();
        _dspMusicStartPosition = (float)AudioSettings.dspTime;
    }
    public void AudioStop(AudioClip clip)
    {
        //GetComponent<StudioEventEmitter>().Stop();
        _audioSource.Stop();
    }
    public void AudioPause()
    {
       // GetComponent<StudioEventEmitter>().EventInstance.setPaused(true);
        _pauseStartDspPosition = musicPosition;
        _audioSource.Pause();
    }
    public void AudioUnPause()
    {
        //_channel.getDSPClock(out _dspClock, out _parentClock);
        _totalPauseValue += (float)(AudioSettings.dspTime - _dspMusicStartPosition - _totalPauseValue) - _pauseStartDspPosition;
        //GetComponent<StudioEventEmitter>().EventInstance.setPaused(false);
        _audioSource.UnPause();
    }
    public void ChangeVolume(float volume) { }
}
