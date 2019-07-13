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

    public MoneyPopup moneyPopupPrefab;

    public Canvas HUD;

    [SerializeField]
    private AnimationCurve oxyShakeStrength;
    [SerializeField]
    private AnimationCurve shakeCurve;
    private float waitTime = 0.8f;
    private bool shaking = false;

    public Canvas worldSpaceCanvas;

    private Vector2 oxyStartPos;
    // Start is called before the first frame update
    void Start()
    {
        oxyRect = oxySlider.GetComponent<RectTransform>();
        oxyStartPos = oxyRect.anchoredPosition;

        FindObjectOfType<Spear>().FishCaughtEvent.AddListener(SpawnMoneyPopup2);
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

    public void SpawnMoneyPopup2(GameObject fish)
    {
        Fish asFish = fish.GetComponent<Fish>();

        MoneyPopup newPopup = Instantiate(moneyPopupPrefab, HUD.transform) as MoneyPopup;
        RectTransform popRect = newPopup.GetComponent<RectTransform>();
        popRect.SetParent(HUD.transform, false);

        RectTransform thisRect = HUD.GetComponent<RectTransform>();

        Vector2 newViewportPoint = Vector2.zero;
        newViewportPoint.x = Camera.main.WorldToViewportPoint(fish.transform.position).x * 800;
        newViewportPoint.y = Mathf.Clamp(40f + Camera.main.WorldToViewportPoint(fish.transform.position).y * 386, 0f, 386f);

        popRect.anchoredPosition = newViewportPoint;
        Debug.Log(newViewportPoint);

        newPopup.SetMoneyAmount(asFish.value);
    }


    public void SpawnMoneyPopup(Vector3 worldPosition, int value)
    {
        MoneyPopup newPopup = Instantiate(moneyPopupPrefab, transform) as MoneyPopup;
        RectTransform popRect = newPopup.GetComponent<RectTransform>();
        popRect.SetParent(transform, false);
        popRect.position = Camera.main.WorldToScreenPoint(worldPosition);

        newPopup.SetMoneyAmount(value);
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
