using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class typingScript : MonoBehaviour
{
    
    public TMP_Text dialogText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Typing(dialogText.text));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Typing(string text){
        dialogText.text = null;
        yield return new WaitForSeconds(2f);
        
        foreach (char letter in text.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
