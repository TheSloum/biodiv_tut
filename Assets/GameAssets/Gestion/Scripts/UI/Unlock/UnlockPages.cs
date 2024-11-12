using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPages : MonoBehaviour
{
    
    [SerializeField] private GameObject page1; 
    [SerializeField] private GameObject page2; 
    [SerializeField] private GameObject page3; 
    [SerializeField] private GameObject page4; 
    [SerializeField] private GameObject page5; 
    
    
    public void PageChange(int page){
        page1.active = false;
        page2.active = false;
        page3.active = false;
        page4.active = false;
        page5.active = false;

        if (page == 1){
        page1.active = true;
        } else if (page== 2){
        page2.active = true;
        }else if (page== 3){
        page3.active = true;            
        }else if (page== 4){
        page4.active = true;            
        }else if (page== 5){
        page5.active = true;            
        }
    }
    public void ClosePage(){
        gameObject.active = false;
    }
}
