using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hauntable : MonoBehaviour
{
	public bool IsPossessed;
	public bool PreviouslyInteracted;

	private int shakeAnimHash;

	void Start()
	{
		IsPossessed = false;
        PreviouslyInteracted = false;

		shakeAnimHash = Animator.StringToHash("Shake");

		GameDirector.Instance.RegisterHauntable(this);
		GameDirector.Instance.HintEvent += new Action(OnHint);
	}

	private void OnHint()
	{
	}
}
