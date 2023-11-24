using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInputUtility : MonoBehaviour
{
    private Button[] _buttons;
    // Start is called before the first frame update
    void Awake()
    {
        _buttons = GetComponentsInChildren<Button>();
        _buttons[0].Select();
    }
    

    
}
