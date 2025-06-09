using UnityEngine;
using UnityEngine.UI;

public class SliderButton : MenuElement
{
    public Color highlightColor;

    public Slider slider;
    public int incrementSpeed;

    private float sliderValue; // slider is rounding value to whole numbers
    public bool isIncreasing;
    public float holdStepValue;
    private float stepValue = 1;
    public override void Interact()
    {
        if (isIncreasing)
        {
            slider.value += stepValue;
        }
        else
        {
            slider.value -= stepValue;
        }
    }

    public override void HoldInteract()
    {
        if (isIncreasing)
        {
            slider.value += holdStepValue * Time.deltaTime;
        }
        else
        {
            slider.value -= holdStepValue * Time.deltaTime;
        }
    }

    public override void Select()
    {
        GetComponent<Image>().color = highlightColor;
    }

    public override void Deselect()
    {
        GetComponent<Image>().color = Color.white;
    }
}
