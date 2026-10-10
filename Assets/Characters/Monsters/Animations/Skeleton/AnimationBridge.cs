using UnityEngine;

public class IceElfAnimation : MonoBehaviour
{
    private EnemyAI2D parentScript;

    void Start()
    {
        // Find the EnemyAI2D script on the parent object
        parentScript = GetComponentInParent<EnemyAI2D>();
    }

    // This is the function the Animation Event will call
    public void ProcDamage()
    {
        if (parentScript != null)
        {
            parentScript.ProcDamage();
        }
        else
        {
            Debug.LogError("Bridge couldn't find EnemyAI2D on Parent!");
        }
    }
}