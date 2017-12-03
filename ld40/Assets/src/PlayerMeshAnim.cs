using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerMeshAnim : MonoBehaviour
{
	public event Action DoFloorChange;
	private int scareAnimHash;
	private int moveBlendHash;
	private int stairClimbHash;
	private int stairDescendHash;
	private Animator anim;

	public bool IsInAnim = false;

	void Start()
	{
		moveBlendHash = Animator.StringToHash("MoveSpeed");
		scareAnimHash = Animator.StringToHash("DoScare");
		stairClimbHash = Animator.StringToHash("DoStairClimb");
		stairDescendHash = Animator.StringToHash("DoStairDescend");
		anim = GetComponent<Animator>();
	}
    public void SetSpeed(float speed)
    {
		if (!IsInAnim)
			anim.SetFloat(moveBlendHash, speed);
    }

	public void StopWalking()
	{
		anim.SetFloat(moveBlendHash, 0f);
	}

    public void PlayScare()
    {
		StopWalking();
		anim.SetTrigger(scareAnimHash);
        IsInAnim = true;
    }

	public void ClimbStairs()
	{
		StopWalking();
		anim.SetTrigger(stairClimbHash);
        IsInAnim = true;
	}

	public void DescendStairs()
	{
		StopWalking();
		anim.SetTrigger(stairDescendHash);
		// IsInAnim = true;
		// TODO: Uncomment when stair descend anim is in place
	}

	public void ScareFinished()
	{
		IsInAnim = false;
	}

	public void OnFloorChange()
	{
        if (DoFloorChange != null)
            DoFloorChange();
		Debug.Log("Floor change");
	}

	public void OnStairLeaveEnd()
	{
		Debug.Log("fin");
        IsInAnim = false;
	}
}
