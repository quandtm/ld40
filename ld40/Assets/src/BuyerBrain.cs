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

    void Start()
    {
		isScaredFlag = Animator.StringToHash("IsScared");
		beginTrigger = Animator.StringToHash("Begin");
		var randIdx = (int)(UnityEngine.Random.value * buyerPrefabs.Count);
        randIdx = Math.Min(randIdx, buyerPrefabs.Count - 1);
		var childGameObject = GameObject.Instantiate(buyerPrefabs[randIdx], transform.position, Quaternion.identity);
        childGameObject.transform.parent = transform;
		ChildMesh = childGameObject.GetComponent<Animator>();
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
            // TODO: Remove and replace with proper animation tracking
            tmpAnimTimer -= Time.deltaTime;
			if (tmpAnimTimer <= 0f)
			{
				GameDirector.Instance.ReportBuyingDone();
			}
		}
    }
}
