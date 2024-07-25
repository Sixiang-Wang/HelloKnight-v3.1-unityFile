using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    // Start is called before the first frame update
    public bool endDash;

    private void OnEnable()
    {
        GetComponent<Animator>().SetTrigger("Dash");
    }
    private void FixedUpdate()
    {
        if (endDash)
        {
            endDash = false;
            this.gameObject.SetActive(false);
        }
    }
}
