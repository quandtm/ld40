using UnityEngine;

public class GhostSpawnBrain : MonoBehaviour
{
    public Vector3 Destination;
    public float Speed;
    bool alive = true;

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
        var toDst = Destination - transform.position;
        transform.position = transform.position + (toDst.normalized * Speed * Time.deltaTime);
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
