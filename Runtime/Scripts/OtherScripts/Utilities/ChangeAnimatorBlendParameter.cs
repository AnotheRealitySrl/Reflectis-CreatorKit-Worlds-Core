using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimatorBlendParameter : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private string parameterName;

    public void ChangeValue (float newValue)
    {
        anim.SetFloat(parameterName, newValue);
    }
}
