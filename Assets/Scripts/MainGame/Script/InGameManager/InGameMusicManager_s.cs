using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public partial class InGameMusicManager_s : Singleton<InGameMusicManager_s>//data 
{
    [Header("fmod")]
    private int _masterSampleRate;
    private float _currentSample;
    private StudioEventEmitter _eventEmitter;
    private ulong _startDspClock;
    private ulong _dspClock;
    private ulong _parent;
    private FMOD.Channel _requestdChannel;
    private FMOD.ChannelGroup _channelsGroup;
    [Header("data")]
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private int _bpm;
    private WaitForEndOfFrame _waitUpdate;
    public float secPerBeat { get; private set; }
    public float musicPosition { get; private set; }
    private float _musicPositionInBeats;
    public int completedLoops { get; private set; }
    private float _lastBeatCount;
    public float loopPositionInBeats { get; private set; }
    public bool isPause { get; private set; }
    private void Start()
    {
        secPerBeat = 60f / _bpm;
        isPause = true;
        _eventEmitter = GetComponent<StudioEventEmitter>();
        _waitUpdate = new WaitForEndOfFrame();
        FMODUnity.RuntimeManager.CoreSystem.getSoftwareFormat(out _masterSampleRate, out FMOD.SPEAKERMODE speakerMode, out int numRawSpeakers);
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
    int positionTest;
    private void Update()
    {
        if (!isPause)
        {
            GetCurrentDSPClock(_channelsGroup, out _dspClock, out _parent);
            musicPosition = (_currentSample-_startDspClock) / _masterSampleRate;
            _musicPositionInBeats = musicPosition / secPerBeat;
            if (_musicPositionInBeats >= (completedLoops + 1))
            {
                completedLoops++;
                InGameBeatManager_s.Instance.NextBit();
                Debug.Log(musicPosition);
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
        _eventEmitter.Play();
        StartCoroutine(GetChannelAndStart());
    }
    IEnumerator GetChannelAndStart()
    {
        while (ReturnEventChannel(_eventEmitter, out _requestdChannel, out _channelsGroup)!=FMOD.RESULT.OK)
        {
            yield return _waitUpdate;
        }
        GetCurrentDSPClock(_channelsGroup, out _dspClock, out _parent);
        _startDspClock = _dspClock;
        isPause = false;
    }
    public void AudioStop(AudioClip clip)
    {
        _eventEmitter.Stop();
    }
    public void AudioPause()
    {
        _channelsGroup.setPaused(true);
        GetCurrentDSPClock(_channelsGroup, out _dspClock, out _parent);
        isPause = true;
    }
    public void AudioUnPause()
    {
        _requestdChannel.setPosition((uint)_dspClock, FMOD.TIMEUNIT.PCM);
        _channelsGroup.setPaused(false);
        isPause = false;
    }
    public void ChangeVolume(float volume) { }

    private FMOD.RESULT ReturnEventChannel(FMODUnity.StudioEventEmitter eventEmitter, out FMOD.Channel requestedChannel, out FMOD.ChannelGroup channelsGroup)
    {
        requestedChannel = new FMOD.Channel();
        channelsGroup = new FMOD.ChannelGroup();

        FMOD.RESULT result = eventEmitter.EventInstance.getChannelGroup(out FMOD.ChannelGroup newgroup);

        //Checking at every step if there is an error which will return a invalid result 
        if (result != FMOD.RESULT.OK)
        {
            Debug.Log("Failed to retrieeve parent channel group with error: " + result);
            return result;
        }

        result = newgroup.getGroup(0, out channelsGroup);

        if (result != FMOD.RESULT.OK)
        {
            Debug.Log("Failed to retrieve child channel group with error: " + result);
            return result;
        }

        result = channelsGroup.getChannel(0, out requestedChannel);

        if (result == FMOD.RESULT.OK)
        {
            Debug.Log("Retrieved the correct channel");
            return result;
        }
        else
        {
            Debug.Log("Failed to find the right channel with error: " + result);
            return result;
        }
    }
    private FMOD.RESULT GetCurrentDSPClock(FMOD.ChannelGroup cG, out ulong dspClock, out ulong parent)
    {
        FMOD.RESULT result = cG.getDSPClock(out dspClock, out parent);

        if (result != FMOD.RESULT.OK)
        {
            Debug.Log("Failed to retrieve DSP clock with error: " + result);
            dspClock = 0;
            parent = 0;
            return result;
        }
        _currentSample = _dspClock;
        return result;
    }
}
