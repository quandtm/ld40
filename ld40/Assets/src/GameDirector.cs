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

public class GameDirector : MonoBehaviour
{
    public GameConfig BootstrapConfig;
    public static GameConfig NextConfig = null;
    public static GameDirector Instance { get; private set; }

    public event Action HintEvent;

    public Phase CurrentPhase;

    public int RemainingHaunts;
    public float SecondsRemaining;

    private List<Hauntable> haunts = new List<Hauntable>();
    private float sinceLastHint;
    private bool hintsEnabled;

    void Start()
    {
        if (Instance == null)
            Instance = this;

        if (NextConfig == null)
            NextConfig = BootstrapConfig;

        CurrentPhase = Phase.PreHaunt;
    }

    void Update()
    {
        switch (CurrentPhase)
        {
            case Phase.PreHaunt:
                BeginHaunt();
                break;

            case Phase.Haunt:
                // TODO: Should wait until the haunt is over
                CurrentPhase = Phase.Play;
                break;

            case Phase.Play:
                CheckGameConditions();
                break;
        }
    }

    void Destroy()
    {
        Instance = null;
    }

    public void RegisterHauntable(Hauntable haunt)
    {
        haunts.Add(haunt);
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
        }
        else if (SecondsRemaining <= 0f)
        {
            // Loss (or is it?)
            CurrentPhase = Phase.Buy;
        }
        else if (hintsEnabled)
        {
            if (SecondsRemaining <= NextConfig.LastChanceHintTime)
            {
                if (HintEvent != null)
                    HintEvent();
                hintsEnabled = false; // No more hints allowed after the last chance hint
            }
            else
            {
                sinceLastHint += Time.deltaTime;
                if (sinceLastHint >= NextConfig.SecondsBetweenHints)
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
        CurrentPhase = Phase.Haunt;

        RemainingHaunts = NextConfig.NumGhosts;
        SecondsRemaining = NextConfig.TimeLimit;
        sinceLastHint = 0f;
        hintsEnabled = NextConfig.EnableHints;

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
