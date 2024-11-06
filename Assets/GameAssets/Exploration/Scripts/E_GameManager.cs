using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_GameManager : MonoBehaviour{
	public static E_GameManager instance = null;

  public GameObject oxygenManagerObject;
  private E_OxygenManager oxygenManager;


	void Awake(){
		if(E_GameManager.instance == null){
			E_GameManager.instance = this;
		}

    oxygenManager = oxygenManagerObject.GetComponent<E_OxygenManager>();
	}

  public E_OxygenManager GetOxygenManager()
  {
    return oxygenManager;
  }

  void Start()
  {

  }

}
