using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Master_Counter : MonoBehaviour
{
#if UNITY_EDITOR
        [StyledString(12,1,1,1)]
#endif
    [SerializeField]
    private string counterValueStrg;

    public GameObject textToUpdate;

    private int counterValue = 0;

    void Update()
    {
        if (textToUpdate != null)
        {
            if (textToUpdate.GetComponent<Text>())
            {
                textToUpdate.GetComponent<Text>().text = counterValueStrg;
            }
            else if (textToUpdate.GetComponent<TextMeshProUGUI>())
            {
                textToUpdate.GetComponent<TextMeshProUGUI>().text = counterValueStrg;
            }
        }
    }

    [ContextMenu("IncrementCounter")]
    public void IncrementCounter()
    {
        counterValue++;
        counterValueStrg = counterValue.ToString();
    }

    [ContextMenu("DecrementCounter")]
    public void DecrementCounter()
    {
        counterValue--;
        counterValueStrg = counterValue.ToString();
    }

    public void SetCounterValue(int newCounterValue)
    {
        counterValue = newCounterValue;
    }

    public int GetCounterValue()
    {
        return counterValue;
    }
}
