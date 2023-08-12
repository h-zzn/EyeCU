using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class typingScript : MonoBehaviour
{
    
    public TMP_Text dialogText;
    public TMP_Text dialogText2;
    public TMP_Text dialogText3;

    Animator animator;

    public static bool isDone = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if(dialogText2 == null || dialogText3 ==null){
            StartCoroutine(Typing(dialogText.text));
        }

        else{
            StartCoroutine(Typing(dialogText.text, dialogText2.text, dialogText3.text));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isDone", isDone);
    }

    IEnumerator Typing(string text){
        dialogText.text = null;

        yield return new WaitForSeconds(2f);
        
        foreach (char letter in text.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }

    }

    IEnumerator Typing(string text1, string text2, string text3){
        dialogText.text = null;
        dialogText2.text = null;
        dialogText3.text = null;

        yield return new WaitForSeconds(1f);
        
        foreach (char letter in text1.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(3f);
        dialogText.enabled = false;
        
        foreach (char letter in text2.ToCharArray()){
            dialogText2.text += letter;
            yield return new WaitForSeconds(0.1f);
        }

        

        yield return new WaitForSeconds(3f);
        dialogText2.enabled = false;
        
        foreach (char letter in text3.ToCharArray()){
            dialogText3.text += letter;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(3f);
        isDone = true;
        
        
    }
}
