using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public AnimationClip clip;
    Animator anim;
    private float destroyTime;

    private void Start()
    {
        anim = GetComponent<Animator>();
        destroyTime = Time.time + clip.length;
        
    }

    private void Update()
    {
        if (Time.time > destroyTime)
            Destroy(this.gameObject);
    }
}
