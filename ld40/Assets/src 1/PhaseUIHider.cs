using UnityEngine;

public class PhaseUIHider : MonoBehaviour
{
    public Phase VisiblePhase;

	void Start()
	{
		GameDirector.Instance.PhaseChangeEvent += OnPhaseChange;
	}

	void OnDestroy()
	{
		GameDirector.Instance.PhaseChangeEvent -= OnPhaseChange;
	}

	void OnPhaseChange()
	{
		gameObject.SetActive(GameDirector.Instance.CurrentPhase == VisiblePhase);
	}
}
