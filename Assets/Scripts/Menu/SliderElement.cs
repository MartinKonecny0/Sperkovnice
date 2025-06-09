using UnityEngine;
using UnityEngine.UI;

public class SliderElement : MenuElement
{
    private Slider slider;
    public float holdStepValue = 1;
    public float stepValue = 1;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public override void Right()
    {
        slider.value += stepValue;
    }
    public override void Left()
    {
        slider.value -= stepValue;
    }
    public override void RightHold()
    {
        slider.value += holdStepValue * Time.deltaTime;
    }
    public override void LeftHold()
    {
        slider.value -= holdStepValue * Time.deltaTime;
    }
    public override void Interact()
    {
        slider.value = 0;
    }
    public override void Select()
    {

    }
    public override void Deselect()
    {

    }
}
