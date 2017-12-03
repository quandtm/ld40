using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyerBrain : MonoBehaviour
{
	public bool WasStillPossessed;
    public Animator ChildMesh;
    private bool hasAnimated = false;

	private int happyAnim;
	private int scaredAnim;

	private float tmpAnimTimer = 3f;

    void Start()
    {
		happyAnim = Animator.StringToHash("DoHappy");
		scaredAnim = Animator.StringToHash("DoScared");
		if (ChildMesh == null)
			Debug.LogError("Animator wasn't bound to BuyerBrain, check prefab!");
    }

    void Update()
    {
		if (!hasAnimated)
		{
            hasAnimated = true;
			if (WasStillPossessed)
				ChildMesh.SetTrigger(scaredAnim);
			else
				ChildMesh.SetTrigger(happyAnim);
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
