using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Roguelike;

/// <summary>
/// ステージ上に置いてあるもの（床、壁、罠、敵、アイテム）のベースクラス
/// マウスオーバー、マウスクリック時の処理を実装
/// </summary>
public abstract class StageObjectBase : MonoBehaviour {

	SpriteRenderer mSpriteRenderer;
	AudioSource mClickAudioSource;
	Transform mAudioSourceTransform;
	GameMaster gm;

	protected virtual void Start() {
		mSpriteRenderer = GetComponent<SpriteRenderer>();
		mAudioSourceTransform = transform.Find("Audio Source");
		mClickAudioSource = GetComponentInChildren<AudioSource>();
	}

	protected virtual void Update() {
		// ゲームマスターを取得
		if(this.gm == null) {
			this.gm = GameObject.Find("GameMaster").GetComponent<GameMaster>();
		}
	}

	//マウスオーバー時のハイライトオミット
	/*
    protected void OnMouseOver()
    {
        mSpriteRenderer.color = new Color(100,100,100,255);
        Debug.Log("mouseOver");
    }
    protected void OnMouseExit()
    {
        mSpriteRenderer.color = new Color(255, 255, 255, 255);
        Debug.Log("MouseExit");
    }
    */

	protected void OnMouseDown() {
		SetSoundSourcePosition();
		mClickAudioSource.Play();
		Debug.Log("soundPlay");

		//警告を抑えるための無意味なコンティヌーウィズ
		SetOriginalPosition().ContinueWith((message) => message);
	}

	private Vector3 SetSoundSourcePosition() {
		var player1 = this.gm.Player1Instance;
		var playerToThisVector = mAudioSourceTransform.position - player1.transform.position;

		switch(player1.m_LookDirection) {
			case Assets.Scripts.Roguelike.Player1.LookDirection.UP:
				return transform.position;
			case Assets.Scripts.Roguelike.Player1.LookDirection.RIGHT:
				playerToThisVector = RotateMatrix(playerToThisVector, 90);
				break;
			case Assets.Scripts.Roguelike.Player1.LookDirection.DOWN:
				playerToThisVector = RotateMatrix(playerToThisVector, 180);
				break;
			case Assets.Scripts.Roguelike.Player1.LookDirection.LEFT:
				playerToThisVector = RotateMatrix(playerToThisVector, 270);
				break;
		}
		mAudioSourceTransform.position = player1.transform.position + playerToThisVector;
		return transform.position;
	}

	private Vector2 RotateMatrix(Vector2 vector, int euler) {
		Vector2 vectorRotated = vector;
		var rad = Mathf.Deg2Rad * euler;

		var x = vector.x * Mathf.Cos(rad) - vector.y * Mathf.Sin(rad);
		var y = vector.x * Mathf.Sin(rad) + vector.y * Mathf.Cos(rad);

		vectorRotated = new Vector2(x, y);

		return vectorRotated;
	}

	private async Task SetOriginalPosition() {
		await Task.Delay(100);
		mAudioSourceTransform.position = transform.position;
	}

}
