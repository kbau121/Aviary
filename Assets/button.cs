using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Character")
        {
        
            ButtonPressed();
        }
    }

    void ButtonPressed()
    {
        Debug.Log("Button Pressed");
    }
}
