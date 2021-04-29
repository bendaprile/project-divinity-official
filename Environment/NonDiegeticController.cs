using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NonDiegeticController : MonoBehaviour
{
    [SerializeField] float HardSwitchDuration = 1f;
    [SerializeField] float SwitchDuration = 3f;

    [SerializeField] private AudioSource[] audioSources;

    private float audioLevel;

    [SerializeField] private List<AudioClip> ApocNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> ApocCombatMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> TundraNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> TundraCombatMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> StormNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> StormCombatMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> JungleNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> JungleCombatMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> CityNormalMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> CityCombatMusic = new List<AudioClip>();

    private (List<AudioClip>, List<AudioClip>) ApocMusic;
    private (List<AudioClip>, List<AudioClip>) TundraMusic;
    private (List<AudioClip>, List<AudioClip>) StormMusic;
    private (List<AudioClip>, List<AudioClip>) JungleMusic;
    private (List<AudioClip>, List<AudioClip>) CityMusic;

    private Stack<(List<AudioClip>, List<AudioClip>)> Playlist = new Stack<(List<AudioClip>, List<AudioClip>)>();
    private int playlistIter;

    private CombatChecker CC;

    private float kill_until;
    private bool audio_killed;


    private Zones current_zone = Zones.Apoc;


    void Start()
    {
        Assert.IsTrue(audioSources[0].volume == audioSources[1].volume);
        audioLevel = audioSources[0].volume;
        CC = GameObject.Find("Player").GetComponentInChildren<CombatChecker>();
        Playlist.Push((null, null));

        ApocMusic = (ApocNormalMusic, ApocCombatMusic);
        TundraMusic = (TundraNormalMusic, TundraCombatMusic);
        StormMusic = (StormNormalMusic, StormCombatMusic);
        JungleMusic = (JungleNormalMusic, JungleCombatMusic);
        CityMusic = (CityNormalMusic, CityCombatMusic);
    }


    public void ChangeAudioSpecific(List<AudioClip> data)
    {
        Playlist.Push((data, null));
        kill_until = Time.unscaledTime + HardSwitchDuration;
    }

    public void ChangeAudioGeneral() //TODO MAKE BASED ON LOCATION
    {
        Playlist.Pop();
        kill_until = Time.unscaledTime + HardSwitchDuration;
    }


    private void Update()
    {
        if(Time.unscaledTime < kill_until)
        {
            if(audioSources[0].volume > 0)
            {
                audioSources[0].volume -= audioLevel * Time.unscaledDeltaTime / HardSwitchDuration;
            }

            if(audioSources[1].volume > 0)
            {
                audioSources[1].volume -= audioLevel * Time.unscaledDeltaTime / HardSwitchDuration;
            }

            audio_killed = true;
            return;
        }


        if (Playlist.Count == 1) //Swaps music based on the zone
        {
            if (current_zone == Zones.Apoc && Playlist.Peek() != ApocMusic)
            {
                Playlist.Pop();
                Playlist.Push(ApocMusic);
            }
        }


        if(audio_killed) //Happens once
        {
            audio_killed = false;
            playlistIter = 0;
            audioSources[0].clip = Playlist.Peek().Item1[0];
            audioSources[0].time = 0;

            if(Playlist.Peek().Item2 != null)
            {
                audioSources[1].clip = Playlist.Peek().Item2[0];
                audioSources[1].time = 0;
            }

        }


        if (CC.enemies_nearby && (Playlist.Peek().Item2 != null))
        {
            audioSources[0].volume -= audioLevel * Time.unscaledDeltaTime / SwitchDuration;
            audioSources[1].volume += audioLevel * Time.unscaledDeltaTime / SwitchDuration;
        }
        else
        {
            audioSources[0].volume += audioLevel * Time.unscaledDeltaTime / SwitchDuration;
            audioSources[1].volume -= audioLevel * Time.unscaledDeltaTime / SwitchDuration;
        }





        if (!audioSources[0].isPlaying) //Next track
        {
            playlistIter = (playlistIter + 1) % Playlist.Peek().Item1.Count;

            audioSources[0].clip = Playlist.Peek().Item1[playlistIter];
            audioSources[0].time = 0;
            audioSources[0].Play();

            if (Playlist.Peek().Item2 != null)
            {
                audioSources[1].clip = Playlist.Peek().Item2[0];
                audioSources[1].time = 0;
                audioSources[1].Play();
            }
        }
    }

}
