using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase
{
    PreHaunt,
    Haunt,
    Play,
    Buy
}

public class GameDirectorImpl
{
    public event Action HintEvent;

    public Phase CurrentPhase;

    public int RemainingHaunts;
    public float SecondsRemaining;

    private List<Hauntable> haunts = new List<Hauntable>();
    private float sinceLastHint;
    private bool hintsEnabled;

    private GameConfig config;
    public bool IsSetup { get { return config != null; } }

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
                CurrentPhase = Phase.Play;
                Debug.Log("begin play");
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
            // TODO: Spawn ghosts and give them their object as a destination
        }

        // TODO: Turn off lights for certain duration so ghosts can fly to objects, then kill ghosts and re-enable lights

        if (HintEvent != null)
            HintEvent();
    }
}

public class GameDirector : MonoBehaviour
{
    public GameConfig BootstrapConfig;

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
