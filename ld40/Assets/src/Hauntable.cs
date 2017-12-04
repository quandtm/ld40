using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
public class Hauntable : MonoBehaviour
{
	private bool isPossessed;
	public bool IsPossessed
    {
        get { return isPossessed; }
        set
        {
            if (isPossessed != value)
            {
                isPossessed = value;
                if (isPossessed == false)
                    GameDirector.Instance.ReportGhostEliminated();
                else
                    wasEverPossessed = true;
            }
        }
    }
	[HideInInspector]
	public bool PreviouslyInteracted;

	private int shakeAnimHash;
	private Animator anim;
	private bool registered = false;
	private bool wasEverPossessed;

	void Start()
	{
        registered = false;
		isPossessed = false;
        PreviouslyInteracted = false;

		anim = GetComponent<Animator>();
		shakeAnimHash = Animator.StringToHash("Shake");
	}

	void OnDestroy()
	{
		GameDirector.Instance.HintEvent -= OnHint;
		GameDirector.Instance.PhaseChangeEvent -= OnPhaseChange;
	}

	void Update()
	{
		if (!registered)
		{
			GameDirector.Instance.RegisterHauntable(this);
			GameDirector.Instance.HintEvent += OnHint;
			GameDirector.Instance.PhaseChangeEvent += OnPhaseChange;
			registered = true;
		}
	}

	private void OnPhaseChange()
	{
		if (GameDirector.Instance.CurrentPhase == Phase.Buy && wasEverPossessed)
		{
			var boxCenter = GetComponent<BoxCollider>().center;
			var prefab = GameDirector.Instance.BuyerPrefab;
            if (prefab != null)
			{
				var go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
				var xform = go.transform;
				xform.parent = gameObject.transform;
				var lp = boxCenter;
				lp.y = -(GetComponent<BoxCollider>().size.y / 2f);
				xform.localPosition = lp;
				var brain = go.GetComponent<BuyerBrain>();
				brain.WasStillPossessed = isPossessed;
			}
			else
				Debug.LogError("Buyer prefab must be set on game director");
		}
	}

	private void OnHint()
	{
		if (IsPossessed)
			anim.SetTrigger(shakeAnimHash);
    }
}
