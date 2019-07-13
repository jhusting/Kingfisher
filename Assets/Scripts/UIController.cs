using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider oxySlider;
    private RectTransform oxyRect;

    public Slider strengthSlider;
    public Text currSpeed;

    public PlayerController pc;

    public CanvasGroup underHUD;

    [SerializeField]
    private AnimationCurve oxyShakeStrength;
    [SerializeField]
    private AnimationCurve shakeCurve;
    private float waitTime = 0.8f;
    private bool shaking = false;

    private Vector2 oxyStartPos;
    // Start is called before the first frame update
    void Start()
    {
        oxyRect = oxySlider.GetComponent<RectTransform>();
        oxyStartPos = oxyRect.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (pc.underWater)
        {
            underHUD.alpha = 1f;
        }
        else
            underHUD.alpha = 0f;
           */
        oxySlider.value = pc.GetOxygenPercent();
        currSpeed.text = "" + pc.moveSpeed;

        if (pc.spearStrength > 0f)
        {
            //strengthSlider.colors.
            strengthSlider.value = pc.spearStrength;
        }
        else
            strengthSlider.value = 0f;

        OxygenTick();
    }

    void OxygenTick()
    {
        float strength = oxyShakeStrength.Evaluate(1f - oxySlider.value);

        if(!shaking)
            StartCoroutine(OxyShake(0.3f*strength, strength));
    }

    IEnumerator OxyShake(float shakeTime, float strength)
    {
        float time = 0;
        shaking = true;

        for (; time < shakeTime; time += Time.deltaTime)
        {
            float newX = oxyStartPos.x + (shakeCurve.Evaluate(time / shakeTime) - 0.5f) * strength * 0.3f;

            oxyRect.anchoredPosition = new Vector2(newX, oxyStartPos.y);

            yield return null;
        }

        for (time = 0; time < waitTime; time += Time.deltaTime)
            yield return null;

        shaking = false; 
    }
}
