using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour { 
	// Use this for initialization
	void Start () {
        Color color = this.gameObject.GetComponent<Renderer>().material.color;
        this.gameObject.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.45f);
    }
}
