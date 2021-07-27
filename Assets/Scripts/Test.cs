using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public System.Type x = typeof(BranchType);
    BranchType y;
    object z;
    public void Start()
    {
        y = BranchType.lType;
        z = y;
    }
}