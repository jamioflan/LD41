using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverLizard : MouseOver
{
	void Update ()
    {
        mouseOverText = GetComponent<Lizard>().lizardName;

		switch(GetComponent<Lizard>().assignment)
		{
			case Lizard.Assignment.FARMER: mouseOverText += " the Farmer"; break;
			case Lizard.Assignment.TAILOR: mouseOverText += " the Tailor"; break;
			case Lizard.Assignment.TRAP: mouseOverText += " the Trapper"; break;
			case Lizard.Assignment.HATCHERY: mouseOverText += " the Breeder"; break;
			case Lizard.Assignment.WORKER: mouseOverText += " the Worker"; break;
		}
	}
}
