using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyerBrain : MonoBehaviour
{
    public List<GameObject> buyerPrefabs;

    public bool WasStillPossessed;
    public Animator ChildMesh;
    private bool hasAnimated = false;

    private int isScaredFlag;
    private int beginTrigger;

    private float tmpAnimTimer = 3f;

    public GameObject ghostPrefab;
    private GameObject ghost;
    private GhostSpawnBrain ghostBrain;
    private int approveAnim;
    private int scaredAnim;
    private int inspectAnim;

    void Start()
    {
        isScaredFlag = Animator.StringToHash("IsScared");
        beginTrigger = Animator.StringToHash("Begin");
        approveAnim = Animator.StringToHash("Happy");
        scaredAnim = Animator.StringToHash("Scared");
        inspectAnim = Animator.StringToHash("buyer_inspect");

        var randIdx = (int)(UnityEngine.Random.value * buyerPrefabs.Count);
        randIdx = Math.Min(randIdx, buyerPrefabs.Count - 1);
        var childGameObject = GameObject.Instantiate(buyerPrefabs[randIdx], Vector3.zero, Quaternion.identity);
        childGameObject.transform.parent = transform;
        childGameObject.transform.localPosition = Vector3.zero;
        ChildMesh = childGameObject.GetComponent<Animator>();

        ghost = GameObject.Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
        ghostBrain = ghost.GetComponent<GhostSpawnBrain>();
        ghostBrain.type = GhostType.Spooker;
        ghost.name = "SpookGhost";
        ghost.transform.parent = transform;
        ghost.transform.localPosition = new Vector3(0, 0, 1);
        ghost.transform.localRotation = Quaternion.Euler(0, 165, 0);
        ghost.SetActive(false);
    }

    void Update()
    {
        if (!hasAnimated)
        {
            hasAnimated = true;
            ChildMesh.SetBool(isScaredFlag, WasStillPossessed);
            ChildMesh.SetTrigger(beginTrigger);
            GameDirector.Instance.ReportBuying();
        }
        else
        {
            var state = ChildMesh.GetCurrentAnimatorStateInfo(0);
            if (state.shortNameHash == scaredAnim || state.shortNameHash == approveAnim)
            {
                if (state.normalizedTime > 0.6f)
                    GameDirector.Instance.ReportBuyingDone();
            }
            else if (state.shortNameHash == inspectAnim && WasStillPossessed)
            {
                if (state.normalizedTime > 0.4f)
                {
                    ghost.SetActive(true);
                    ghostBrain.PlaySpook();
                }
            }
        }
    }
}
