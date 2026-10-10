using UnityEngine;

public class AnimationBridge : MonoBehaviour
{
    private IceElfMovement movementScript;

    void Start()
    {
        // Looks at the Parent object to find your main script
        movementScript = GetComponentInParent<IceElfMovement>();
    }

    // This is the function you will now select in the Animation Window
    public void TriggerFireArrow()
    {
        if (movementScript != null)
        {
            movementScript.FireArrow();
        }
    }
}