using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class Hauntable : MonoBehaviour
{
	public bool IsPossessed;
	public bool PreviouslyInteracted;

	private int shakeAnimHash;
	private Animator anim;
	private bool registered = false;

	void Start()
	{
        registered = false;
		IsPossessed = false;
        PreviouslyInteracted = false;

		anim = GetComponent<Animator>();
		shakeAnimHash = Animator.StringToHash("Shake");
	}

	void Destroy()
	{
		GameDirector.Instance.HintEvent -= OnHint;
	}

	void Update()
	{
		if (!registered)
		{
			GameDirector.Instance.RegisterHauntable(this);
			GameDirector.Instance.HintEvent += OnHint;
			registered = true;
		}
	}

	private void OnHint()
	{
		if (IsPossessed)
			anim.SetTrigger(shakeAnimHash);
    }
}
