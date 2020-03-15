using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public GameObject imageObject;
    public Image previewImage;
    //public GameObject myCube;
    public Material cubeMat;
    public Shader shaderMat;
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("Red", 255);
        PlayerPrefs.SetFloat("Blue", 255);
        PlayerPrefs.SetFloat("Green", 255);
        previewImage = imageObject.GetComponent<Image>();
        //cubeMat = myCube.GetComponent<Material>();
        //shaderMat = myCube.GetComponent<Shader>();
    }

    

    public void OnRedValueChanged(float newValue)
    {
        PlayerPrefs.SetFloat("Red", newValue);
        PlayerPrefs.Save();
        ChangePreviewColor();
    }
    public void OnBlueValueChanged(float newValue)
    {
        PlayerPrefs.SetFloat("Blue", newValue);
        PlayerPrefs.Save();
        ChangePreviewColor();
    }
    public void OnGreenValueChanged(float newValue)
    {
        PlayerPrefs.SetFloat("Green", newValue);
        PlayerPrefs.Save();
        ChangePreviewColor();
        
    }
    public void ChangePreviewColor()
    {
        float r = PlayerPrefs.GetFloat("Red");
        float g = PlayerPrefs.GetFloat("Green");
        float b = PlayerPrefs.GetFloat("Blue");
        previewImage.color = new Color(r,g,b,1.0f);
        //myCube.GetComponent<Renderer>().material.color = new Color(r, g, b);
    }
}
