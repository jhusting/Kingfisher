using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPopup : MonoBehaviour
{
    public Text moneyText;
    [SerializeField]
    private AnimationCurve sizeCurve;
    private Vector3 startPosition;
    private RectTransform rect;

    private float time = 0f;
    private float speed = 0f;

    //private AnimationCurve wobbleCurve;
    // Start is called before the first frame update
    void Start()
    {
        //moneyText.text = "+1";
        rect = GetComponent<RectTransform>();
        startPosition = rect.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        speed = Mathf.Clamp(speed + 20f * Time.deltaTime, 0f, 60f);
        Vector3 currPosition = rect.anchoredPosition;

        currPosition.x = startPosition.x + 5f*Mathf.Sin(time*4f);
        currPosition.y += speed * Time.deltaTime;

        rect.anchoredPosition = currPosition;

        if(time < 1f)
        {
            float size = sizeCurve.Evaluate(time);

            rect.localScale = new Vector3(size, size, size);
        }
    }

    public void SetMoneyAmount(int x)
    {
        if(rect)
            startPosition = rect.anchoredPosition;
        moneyText.text = "+" + x;
    }
}
