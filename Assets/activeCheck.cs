using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activeCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<gameStartScript>().enabled = false;
        print("knigeActive ?? "+ PlayerPrefs.HasKey("knifeActive"));

        if(PlayerPrefs.HasKey("knifeActive")){
            this.gameObject.GetComponent<gameStartScript>().enabled = true;
        }
    }

}
