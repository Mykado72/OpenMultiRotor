using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoSomethingInCoroutine : MonoBehaviour {

    public int timerCoroutine;
    void Start()
    {
        StartCoroutine(CheckSomething());
    }

    IEnumerator CheckSomething()
    {
        var wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            //NowRotating = (rot != this.transform.rotation);
            timerCoroutine++;
            //where Tolerance is small... like 0.01.
            //due to float error it might be better to use this... your choice
        }
    }
}
