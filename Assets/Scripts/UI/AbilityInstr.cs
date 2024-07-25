using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityInstr : MonoBehaviour
{

    

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        /*
        if (!printInstr && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
        */




    }

    public void TriggerprintInstrTimer()
    {

        StartCoroutine(openInstr());
    }

    private IEnumerator openInstr()
    {
        yield return new WaitForSeconds(0.1f);
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.2f);

        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
