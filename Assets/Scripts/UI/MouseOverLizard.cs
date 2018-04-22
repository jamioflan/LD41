using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverLizard : MouseOver
{
	void Update ()
    {
        mouseOverText = GetComponent<Lizard>().lizardName;
    }
}
