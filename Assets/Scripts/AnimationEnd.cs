using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEnd : MonoBehaviour
{
    public void OnCompleteAnimation()
    {
        Destroy(this.gameObject);
    }
}
