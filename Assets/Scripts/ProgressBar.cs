using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    [SerializeField] private float lerpSpeed = 3f;


    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeValue(float value)
    {
        progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, value, lerpSpeed);
    }

    public void ChangeColor(Color color)
    {
        progressBar.color = color;
    }
}
