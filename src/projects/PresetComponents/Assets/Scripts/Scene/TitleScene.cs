using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour {
	// Update is called once per frame
	void Update() {
		if(Input.anyKeyDown) {
			GameObject.Find("FadeCanvas").GetComponent<Fade>().FadeIn(1.0f, new System.Action(() => {
				SceneManager.LoadScene("Roguelike");
			}));
		}
	}
}
