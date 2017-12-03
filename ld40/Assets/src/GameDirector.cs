using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public enum Phase
{
    PreHaunt,
    Haunt,
    HauntToPlay,
    Play,
    PlayToBuy,
    Buy
}

public class GameDirectorImpl
{
    private const float HauntDuration = 3f;

    public GameObject GhostPrefab;
    public Vector3 GhostSpawnLocation;

    public event Action HintEvent;
    public event Action PhaseChangeEvent;

    private Phase curPhase;
    public Phase CurrentPhase
    {
        get { return curPhase; }
        set
        {
            if (curPhase != value)
            {
                curPhase = value;
                if (PhaseChangeEvent != null)
                    PhaseChangeEvent();
            }
        }
    }

    public int RemainingHaunts;
    public float SecondsRemaining;

    private List<Hauntable> haunts = new List<Hauntable>();
    private float sinceLastHint;
    private bool hintsEnabled;

    private GameConfig config;
    public bool IsSetup { get { return config != null; } }

    private float hauntTimeRemaining;

    public void Setup(GameConfig cfg)
    {
        if (config != null)
        {
            Debug.LogError("Config already exists!");
            return;
        }
        config = cfg;
    }

    public void RegisterHauntable(Hauntable haunt)
    {
        Debug.LogFormat("{0} registered as haunt", haunt.name);
        haunts.Add(haunt);
    }

    public void Update()
    {
        switch (CurrentPhase)
        {
            case Phase.PreHaunt:
                BeginHaunt();
                break;

            case Phase.Haunt:
                hauntTimeRemaining -= Time.deltaTime;
                if (hauntTimeRemaining <= 0f)
                {
                    CurrentPhase = Phase.HauntToPlay;
                    if (HintEvent != null)
                        HintEvent();
                }
                break;

            case Phase.HauntToPlay:
                CurrentPhase = Phase.Play;
                Debug.Log("Begin play");
                break;

            case Phase.Play:
                CheckGameConditions();
                break;
        }
    }

    private void CheckGameConditions()
    {
        int numRemaining = 0;
        foreach (var haunt in haunts)
        {
            if (haunt.IsPossessed)
                numRemaining++;
        }
        RemainingHaunts = numRemaining;

        SecondsRemaining -= Time.deltaTime;

        if (RemainingHaunts == 0)
        {
            // Victory
            CurrentPhase = Phase.Buy;
            Debug.Log("Victory");
        }
        else if (SecondsRemaining <= 0f)
        {
            // Loss (or is it?)
            CurrentPhase = Phase.Buy;
            Debug.Log("Time out");
        }
        else if (hintsEnabled)
        {
            if (SecondsRemaining <= config.LastChanceHintTime)
            {
                if (HintEvent != null)
                    HintEvent();
                hintsEnabled = false; // No more hints allowed after the last chance hint
            }
            else
            {
                sinceLastHint += Time.deltaTime;
                if (sinceLastHint >= config.SecondsBetweenHints)
                {
                    if (HintEvent != null)
                        HintEvent();
                    sinceLastHint = 0;
                }
            }
        }
    }

    private void BeginHaunt()
    {
        if (haunts.Count == 0)
        {
            Debug.Log("Nothing to haunt, waiting...");
            return; // Keep spinning until we have some haunts
        }
        Debug.Log("begin haunt");
        CurrentPhase = Phase.Haunt;

        RemainingHaunts = config.NumGhosts;
        SecondsRemaining = config.TimeLimit;
        sinceLastHint = 0f;
        hintsEnabled = config.EnableHints;

        int toRemove = haunts.Count - RemainingHaunts;
        if (toRemove > 0)
        {
            // Remove some random haunt locations until we have enough
            for (int i = 0; i < toRemove; i++)
            {
                int idx = (int)(UnityEngine.Random.Range(0f, (float)(haunts.Count - 1)));
                haunts.RemoveAt(idx);
            }
        }

        foreach (var h in haunts)
        {
            h.IsPossessed = true;
            Vector3 dest = h.gameObject.transform.position;
            float yOff = UnityEngine.Random.Range(-2f, 2f);
            float xOff = UnityEngine.Random.Range(-2f, 2f);
            var spawnPos = GhostSpawnLocation;
            spawnPos.x += xOff;
            spawnPos.y += yOff;
            GameObject ghost = GameObject.Instantiate(GhostPrefab, spawnPos, Quaternion.identity);
            GhostSpawnBrain brain = ghost.GetComponent<GhostSpawnBrain>();
            if (brain == null)
            {
                Debug.LogError("Invalid prefab, ghost needs brain!");
                GameObject.Destroy(ghost);
            }
            brain.Destination = dest;
            brain.Speed = (dest - spawnPos).magnitude / HauntDuration;
        }

        // TODO: Turn off lights for certain duration so ghosts can fly to objects, then kill ghosts and re-enable lights
        hauntTimeRemaining = HauntDuration;
    }
}

public class GameDirector : MonoBehaviour
{
    public GameConfig BootstrapConfig;
    public Transform GhostSpawnLocation;
    public GameObject GhostPrefab; 

    private static GameDirectorImpl inst;
    public static GameDirectorImpl Instance
    {
        get
        {
            if (inst == null)
                inst = new GameDirectorImpl();
            return inst;
        }
    }

    void Start()
    {
        if (!Instance.IsSetup)
        {
            Debug.Log("Using bootstrap");
            inst.Setup(BootstrapConfig);
        }

        inst.GhostSpawnLocation = GhostSpawnLocation.position;
        inst.GhostPrefab = GhostPrefab;
    }

    void Update()
    {
        Instance.Update();
    }

    void Destroy()
    {
        inst = null;
    }
}
