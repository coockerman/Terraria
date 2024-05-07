using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuongDan : MonoBehaviour
{
    public GameObject[] listHuongDan;
    int countHuongDan;

    private void Start()
    {
        countHuongDan = 0;
        StartCoroutine(StartHuongDan());
    }
    IEnumerator StartHuongDan()
    {

        while (true)
        {
            if(countHuongDan < listHuongDan.Length)
            {
                listHuongDan[countHuongDan].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(5);

            if (countHuongDan < listHuongDan.Length)
            {
                listHuongDan[countHuongDan].gameObject.SetActive(false);
            }
            countHuongDan++;
            yield return new WaitForSeconds(3);
        }

    }
}
