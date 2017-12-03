using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyerBrain : MonoBehaviour
{
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
		if (ChildMesh == null)
			Debug.LogError("Animator wasn't bound to BuyerBrain, check prefab!");
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
