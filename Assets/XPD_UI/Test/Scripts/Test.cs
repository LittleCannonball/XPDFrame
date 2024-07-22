using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : Attribute
{
    public string name;
    public Test(string name)
    {
        this.name = name;
    }
}
