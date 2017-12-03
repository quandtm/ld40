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
    public static GameDirector Instance { get; private set; }

    public Phase CurrentPhase;

    private List<Hauntable> haunts = new List<Hauntable>();

    public event Action HintEvent;

    public int TotalHaunts = 1;

    public int RemainingHaunts;

    void Start()
    {
        if (Instance == null)
            Instance = this;

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
    }

    private void BeginHaunt()
    {
        RemainingHaunts = TotalHaunts;
        CurrentPhase = Phase.Haunt;

        int toRemove = haunts.Count - TotalHaunts;
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
		// TODO: Once this is done, switch to play phase, enable character

        if (HintEvent != null)
            HintEvent();
    }
}
