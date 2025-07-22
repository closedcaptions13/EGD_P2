using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PowerpointManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //set up image
   
    public RawImage displayslide;

    private int currentSlide = 0;

    //set up arrray 
    public Texture[] Slides = new Texture[18];


    void Start()
    {

        displayslide.texture = Slides[currentSlide];
        
    }

    /*public void NextButton()
    {
        //currentSlide++;

        if (currentSlide > Slides.Length - 1)
        {
            currentSlide = 0;
        }

        Debug.Log(Slides[currentSlide]);
        displayslide.texture = Slides[currentSlide];
    }*/

/*public void PreviousButton()
{
    //currentSlide--;

    if (currentSlide < 0)
    {
        currentSlide = Slides.Length -1;
    }

    Debug.Log(Slides[currentSlide]);
        displayslide.texture = Slides[currentSlide];
    }*/



// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || (Input.GetKeyDown(KeyCode.DownArrow)))
        {
            currentSlide++;

            if (currentSlide > Slides.Length - 1)
            {
                currentSlide = 0;
            }

            displayslide.texture = Slides[currentSlide];
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || (Input.GetKeyDown(KeyCode.UpArrow)))
        {
            currentSlide--;

            if (currentSlide < 0)
            {
                currentSlide = Slides.Length - 1;
            }

            displayslide.texture = Slides[currentSlide];
        }
    }
}
