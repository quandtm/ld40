using UnityEngine;

public enum GhostType
{
    FloatIn,
    Scared,
    Spooker
}

public class GhostSpawnBrain : MonoBehaviour
{
    public GhostType type;
    public Vector3 Destination;
    public float Speed;
    bool alive = true;

    public bool IsAnimating = false;
    private Animator anim;
    private int fleeAnimTrig;
    private int spookAnimTrig;
    private int fleeAnimName;
    private int spookAnimName;

    void Start()
    {
        GameDirector.Instance.PhaseChangeEvent += OnPhaseChange;
    }

    void OnDestroy()
    {
        GameDirector.Instance.PhaseChangeEvent -= OnPhaseChange;
    }

    void Update()
    {
        switch (type)
        {
            case GhostType.FloatIn:
                var toDst = Destination - transform.position;
                transform.position = transform.position + (toDst.normalized * Speed * Time.deltaTime);
                break;

            case GhostType.Scared:
                HandleNonFloat();
                break;

            case GhostType.Spooker:
                HandleNonFloat();
                break;
        }
    }

    private void HandleNonFloat()
    {
        var animState = anim.GetCurrentAnimatorStateInfo(0);
        if (animState.shortNameHash == fleeAnimName)
        {
            IsAnimating = true;
            if (animState.normalizedTime > 0.5f)
            {
                IsAnimating = false;
                gameObject.SetActive(false);
            }
        }
        else if (animState.shortNameHash == spookAnimName)
        {
            IsAnimating = true;
            if (animState.normalizedTime > 0.5f)
            {
                IsAnimating = false;
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (IsAnimating)
            {
                IsAnimating = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void PlayScared()
    {
        CheckAnim();
        anim.SetTrigger(fleeAnimTrig);
    }

    public void PlaySpook()
    {
        CheckAnim();
        anim.SetTrigger(spookAnimTrig);
    }

    private void CheckAnim()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            fleeAnimTrig = Animator.StringToHash("DoFlee");
            spookAnimTrig = Animator.StringToHash("DoSpookBuyer");
            fleeAnimName = Animator.StringToHash("ghost_scareoff");
            spookAnimName = Animator.StringToHash("ghost_spook");
        }
    }

    void OnPhaseChange()
    {
        if (GameDirector.Instance.CurrentPhase != Phase.Haunt)
		{
			if (alive)
			{
				GameObject.Destroy(gameObject);
				alive = false;
			}
		}
    }
}
