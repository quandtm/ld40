using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerMeshAnim : MonoBehaviour
{
	private int scareAnimHash;
	private int moveBlendHash;
	private Animator anim;

	public bool IsInAnim = false;

	void Start()
	{
		moveBlendHash = Animator.StringToHash("MoveSpeed");
		scareAnimHash = Animator.StringToHash("DoScare");
		anim = GetComponent<Animator>();
	}
    public void SetSpeed(float speed)
    {
		if (!IsInAnim)
			anim.SetFloat(moveBlendHash, speed);
    }

    public void PlayScare()
    {
		SetSpeed(0f);
		anim.SetTrigger(scareAnimHash);
        IsInAnim = true;
    }

	public void ScareFinished()
	{
		IsInAnim = false;
	}
}
