using UnityEngine;
using UnityEngine.UI;

public class Slowmotion : MonoBehaviour
{
    private float slowMo = 0.1f;
    private float  normalTime = 1.0f;
    private bool doSlowmo = false;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Text textUI;

    void Update()
    {
        if (playerMovement.rb.linearVelocity.magnitude > 0)
        {
            if (doSlowmo)
            {
                Time.timeScale = normalTime;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                textUI.text = Time.timeScale.ToString("0");
                doSlowmo = false;
            }
            else
            {
                if (!doSlowmo)
                {
                    Time.timeScale = slowMo;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                    textUI.text = Time.timeScale.ToString("0");
                    doSlowmo = true;
                }
            }
        }
    }
}
