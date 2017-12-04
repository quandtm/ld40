using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Phase
{
    PreHaunt,
    Haunt,
    HauntToPlay,
    Play,
    PlayToBuy,
    Buy,
    BuyToResults
}

public class GameDirectorImpl
{
    private const float HauntDuration = 3f;
    private const float TransitionDuration = 1f;

    public GameObject BuyerPrefab;
    public GameObject GhostPrefab;
    public Vector3 GhostSpawnLocation;

    public event Action HintEvent;
    public event Action PhaseChangeEvent;
    private float transitionProgress = -1f;
    public float TransitionProgress
    {
        get { return Mathf.Clamp01(transitionProgress); }
    }
    public bool IsTransitioning { get { return transitionProgress >= 0f; } }


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

    private int numBuyAnimating = 0;

    private void BeginTransition()
    {
        transitionProgress = 0f;
    }

    private void EndTransition()
    {
        transitionProgress = -1f;
    }

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

    public void ReportGhostEliminated()
    {
        if (CurrentPhase == Phase.Play)
        {
            int numRemaining = 0;
            foreach (var haunt in haunts)
            {
                if (haunt.IsPossessed)
                    numRemaining++;
            }
            RemainingHaunts = numRemaining;

            if (numRemaining == 0)
            {
                CurrentPhase = Phase.PlayToBuy;
                BeginTransition();
                Debug.Log("Victory");
            }
        }
    }

    public void ReportBuying()
    {
        if (CurrentPhase == Phase.Buy)
            numBuyAnimating++;
    }

    public void ReportBuyingDone()
    {
        if (CurrentPhase == Phase.Buy)
        {
            numBuyAnimating--;
            if (numBuyAnimating <= 0)
            {
                CurrentPhase = Phase.BuyToResults;
                BeginTransition();
            }
        }
    }

    public void Update()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            return;
        }

        if (IsTransitioning)
        {
            transitionProgress += Time.deltaTime / TransitionDuration;
            if (transitionProgress > 1f)
                EndTransition();
        }

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
                    BeginTransition();
                    if (HintEvent != null)
                        HintEvent();
                }
                break;

            case Phase.HauntToPlay:
                if (transitionProgress >= .5f)
                    CurrentPhase = Phase.Play;
                break;

            case Phase.Play:
                CheckGameConditions();
                break;

            case Phase.PlayToBuy:
                if (transitionProgress >= .5f)
                    CurrentPhase = Phase.Buy;
                break;

            case Phase.BuyToResults:
                if (transitionProgress >= .5f)
                {
                    var results = ResultsStore.Instance;
                    results.BuyersRemaining = config.NumGhosts - RemainingHaunts;
                    results.TimeRemaining = SecondsRemaining;
                    results.MinBuyersForWin = config.MinBuyersForWin;
                    results.TotalPossibleBuyers = config.NumGhosts;

                    SceneManager.LoadScene("ResultsScreen", LoadSceneMode.Single);
                }
                break;

            default: break; // Buy isn't implemented in Update
        }
    }

    private void CheckGameConditions()
    {
        SecondsRemaining -= Time.deltaTime;

        if (SecondsRemaining <= 0f)
        {
            // Loss (or is it?)
            CurrentPhase = Phase.PlayToBuy;
            BeginTransition();
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
    public GameObject BuyerPrefab;

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
        inst.BuyerPrefab = BuyerPrefab;
    }

    void Update()
    {
        Instance.Update();
    }

    void OnDestroy()
    {
        Debug.Log("Destroying game director");
        inst = null;
    }
}
