using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recycler : MonoBehaviour
{
    public void Pay(int mat){
    if (Materials.instance.price >= 20 && Materials.instance.trash >= 10){
                Materials.instance.price = Materials.instance.price - 20;
                Materials.instance.trash = Materials.instance.trash - 10;
        if(mat == 0)
            {
                Materials.instance.mat_0 = Materials.instance.mat_0 + 20;
            }
        if(mat == 1)
            {
                Materials.instance.mat_1 =Materials.instance.mat_1 + 20;
            }
        if(mat == 2)
            {
                Materials.instance.mat_2 = Materials.instance.mat_2 + 20;
            }
    }
    }
}
