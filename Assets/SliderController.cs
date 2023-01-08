using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    float progress;
    public float maxTimer;

    public Slider slider;
    [SerializeField]
    Image image;
    int state = 0;

    private void Awake()
    {

    }

    public void Setup(float timer)
    {
        maxTimer = timer;
        progress = maxTimer;
        slider.maxValue = maxTimer;
    }
    public void UpdateProgress()
    {
        slider.value = progress;
        if (state < 1 && (slider.maxValue - progress) / slider.maxValue >= .4)
        {
            state = 1;
            image.color = Color.yellow;
        }
        else if (state < 2 && (slider.maxValue - progress) / slider.maxValue >= .65)
        {
            state = 2;
            image.color = Color.red;
        }
    }
    private void Update()
    {
        if(maxTimer>0)
        {
            progress -= Time.deltaTime;
            UpdateProgress();
            if (progress <= 0)
            {
                TimesUp();
            }
        }
    }
    void TimesUp()
    {
        GameManager.Instance.AddStrike();
        gameObject.SetActive(false);
    }
}
