using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Roguelike;
using UnityEngine;

public class RotateEar : MonoBehaviour {
	public AudioListener al;
	public Player1 player;
	public Player1.LookDirection diretion;

	// Start is called before the first frame update
	void Start() {
		//プレイヤーを探す
		this.player = GameObject.Find("Player1").GetComponent<Player1>();
		this.al = this.GetComponent<AudioListener>();
	}

	// Update is called once per frame
	void Update() {
		this.diretion = this.player.m_LookDirection;
		switch(this.diretion) {
			case Player1.LookDirection.UP:
				this.transform.eulerAngles = new Vector3(0, 0, 0);
				break;

			case Player1.LookDirection.RIGHT:
				this.transform.eulerAngles = new Vector3(0, 0, -90);
				break;

			case Player1.LookDirection.LEFT:
				this.transform.eulerAngles = new Vector3(0, 0, 90);
				break;

			case Player1.LookDirection.DOWN:
				this.transform.eulerAngles = new Vector3(0, 0, 180);
				break;

		}
	}
}
