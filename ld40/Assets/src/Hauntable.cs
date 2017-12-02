using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hauntable : MonoBehaviour
{
	public bool IsPossessed;
	public bool PreviouslyInteracted;

	private int shakeAnimHash;
	private Animator anim;

	void Start()
	{
		IsPossessed = false;
        PreviouslyInteracted = false;

		shakeAnimHash = Animator.StringToHash("Shake");

		GameDirector.Instance.RegisterHauntable(this);
		GameDirector.Instance.HintEvent += new Action(OnHint);

		anim = GetComponent<Animator>();
	}

	private void OnHint()
	{
		if (IsPossessed)
			anim.SetTrigger(shakeAnimHash);
	}
}
