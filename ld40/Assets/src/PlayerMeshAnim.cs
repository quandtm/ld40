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

	public GameObject ghostPrefab;
	private GameObject ghost;
	private GhostSpawnBrain ghostBrain;

	void Start()
	{
		moveBlendHash = Animator.StringToHash("MoveSpeed");
		scareAnimHash = Animator.StringToHash("DoScare");
		stairClimbHash = Animator.StringToHash("DoStairClimb");
		stairDescendHash = Animator.StringToHash("DoStairDescend");
		anim = GetComponent<Animator>();

		ghost = GameObject.Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
		ghostBrain = ghost.GetComponent<GhostSpawnBrain>();
		ghostBrain.type = GhostType.Scared;
		ghost.name = "ScaredGhost";
		ghost.transform.parent = transform;
		ghost.transform.localPosition = new Vector3(0, .25f, 1);
		ghost.transform.localRotation = Quaternion.Euler(0, 165, 0);
		ghost.SetActive(false);
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
		IsInAnim = true;
	}

	public void ScareFinished()
	{
		IsInAnim = false;
	}

	public void OnFloorChange()
	{
        if (DoFloorChange != null)
            DoFloorChange();
	}

	public void OnStairLeaveEnd()
	{
        IsInAnim = false;
	}

	public void ShowScaredGhost()
	{
		ghost.SetActive(true);
		ghostBrain.PlayScared();
	}
}
