using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Roguelike {
	using UnityEngine;
	using Assets.Scripts.PresetComponents.Roguelike.Interface;
	using Assets.Scripts.PresetComponents.Roguelike.SectionDivision;
	using static Assets.Scripts.PresetComponents.Roguelike.Interface.GeneratedMapBase;
	using Assets.Scripts.PresetComponents.Roguelike;
	using System.Collections;

	/// <summary>
	/// プレイヤー１の操作挙動
	/// </summary>
	public class Player1 : MonoBehaviour {

		[SerializeField]
		private Sprite m_UpSprite;
		[SerializeField]
		private Sprite m_DownSprite;
		[SerializeField]
		private Sprite m_LeftSprite;
		[SerializeField]
		private Sprite m_RightSprite;

		private SpriteRenderer m_SpriteRenderer;  //このオブジェクトのSpriteRenderer

		[SerializeField]
		private GeneratedMapBase m_GeneratedMapBase;
		private GeneratedMapTile[,] m_MapData;

		private float m_InputHorizontal = 0.0f;   //水平方向の入力
		private float m_InputVertical = 0.0f;   //垂直方向の入力
		[SerializeField]
		private float m_InputThreshold = 0.5f;   //入力のしきい値
		private bool m_InputHorizontalFlag = false;  //水平方向に入力できているのか
		private bool m_InputVerticalFlag = false;  //垂直方向に入力できているのか

		private Vector3 m_Target;  //移動先
		private Vector2Int m_PlayerArrayPos;  //現在のプレイヤーの配列内位置

		//[SerializeField]
		private float m_MoveAmount = 1.0f;  //移動するときの移動量

		private int m_MoveBlockAmount = 1;  //移動するときの移動ブロック数

		public const int MoveAnimationFrame = 15;   // アニメーション移動が完了するまでのフレーム数

		//プレイヤーが見ている方向
		public enum LookDirection {
			UP,
			DOWN,
			RIGHT,
			LEFT
		}

		public LookDirection m_LookDirection {
			get;
			private set;
		} = LookDirection.DOWN;  //自分が向いている方向

		[SerializeField]
		private bool m_IsMove = false;  //動けるかのフラグ


		private bool m_InputReturnWaitFlag = false;  //入力値が０に戻るのを待っているかどうかのフラグ

		[SerializeField]
		private Animator animator;

		#region Main

		/// <summary>
		/// このオブジェクトの移動を許可する
		/// </summary>
		public void StartTurn() {
			m_IsMove = true;
			m_InputReturnWaitFlag = true;
		}

		public void Initialize(GeneratedMapBase map) {
			//マップデータの取得←マップは変化しないということだったので処理速度的にあらかじめコピーします。
			this.m_GeneratedMapBase = map;
			m_MapData = m_GeneratedMapBase.TileData;
			m_PlayerArrayPos = ((MapDataBySectionDivision)m_GeneratedMapBase).GetPlayerPosition();

			//スプライトレンダラーの取得
			m_SpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
			this.animator = this.GetComponent<Animator>();

			//m_MoveAmount = this.transform.localScale.x;
		}

		private void Update() {
			//入力情報の取得
			m_InputHorizontal = Input.GetAxisRaw("Horizontal");
			m_InputVertical = Input.GetAxisRaw("Vertical");

			//入力チェックフラグを降ろす
			m_InputVerticalFlag = false;
			m_InputHorizontalFlag = false;

			//水平方向の入力ができているかチェック
			if(m_InputHorizontal >= m_InputThreshold | m_InputHorizontal <= -m_InputThreshold) {
				m_InputHorizontalFlag = true;
			}

			//垂直方向の入力ができているかチェック
			if(m_InputVertical >= m_InputThreshold | m_InputVertical <= -m_InputThreshold) {
				m_InputVerticalFlag = true;
			}

			//入力戻り待ち
			if(m_InputReturnWaitFlag) {
				if(!m_InputVerticalFlag && !m_InputHorizontalFlag) {
					m_InputReturnWaitFlag = false;
				}
			}

			//動けるのかどうか
			if(m_IsMove && !m_InputReturnWaitFlag) {
				if(m_InputVerticalFlag || m_InputHorizontalFlag) {
					//現在の自分のポジションを保存
					m_Target = this.gameObject.transform.position;

					//NormalMove();
					RadioMove();

					//移動
					//this.transform.position = m_Target;
					this.StartCoroutine(this.moveSmoothAnimation(m_Target));

					// ゴール判定
					GameObject.Find("Goal(Clone)").GetComponent<Goal>().ExitIfTrigger(this.m_PlayerArrayPos);

				}
			}

			//debugcode
			if(Input.GetKey(KeyCode.Q)) {
				StartTurn();
			}

			//スプライトを向きによって変更する <- Animatorのトリガー発動に変更
			SpriteChange();
		}

		/// <summary>
		/// 移動できるかチェックする
		/// </summary>
		private bool MoveCheck(Direction direction) {
			//次に移動する配列番号の取得
			Vector2Int nextArrayPos = m_GeneratedMapBase.GetNextDirectionPosition(m_PlayerArrayPos, direction);

			//次に移動する配列番地にあるタイルの取得
			GeneratedMapTile tile = m_MapData[nextArrayPos.x, nextArrayPos.y];

			bool checkFlag = true;  //チェック結果

			//タイルの種類ごとに処理判別
			switch(tile) {
				//壁だった場合
				case GeneratedMapTile.Wall:
					checkFlag = false;
					Debug.Log("Wall");
					break;

				//天井だった場合
				case GeneratedMapTile.Ceil:
					checkFlag = false;
					Debug.Log("Ceil");
					break;
			}

			return checkFlag;
		}

		/// <summary>
		/// 通常の移動（ラジオ式ではない）
		/// </summary>
		private void NormalMove() {
			//どちらも入力判定がでているかどうか
			if(m_InputVerticalFlag && m_InputHorizontalFlag) {
				//水平方向の方が入力値が大きいかどうか
				if(Mathf.Abs(m_InputVertical) <= Mathf.Abs(m_InputHorizontal)) {
					NormalHorizontalMove();
				} else {
					NormalVertexMove();
				}
			}

			//水平方向のみ入力されているかどうか
			if(!m_InputVerticalFlag && m_InputHorizontalFlag) {
				NormalHorizontalMove();
			}

			//垂直方向のみ入力されているかどうか
			if(m_InputVerticalFlag && !m_InputHorizontalFlag) {
				NormalVertexMove();
			}
		}

		/// <summary>
		/// 垂直方向に移動（通常）
		/// </summary>
		private void NormalVertexMove() {
			//垂直方向の上側に傾いているかどうか
			if(m_InputVertical >= 0) {
				if(MoveCheck(Direction.Top)) {
					m_Target.y += m_MoveAmount;
					ArrayPosMove(0, -m_MoveBlockAmount);
				}
			} else {
				if(MoveCheck(Direction.Bottom)) {
					m_Target.y -= m_MoveAmount;
					ArrayPosMove(0, m_MoveBlockAmount);
				}
			}
		}

		/// <summary>
		/// 水平方向に移動（通常）
		/// </summary>
		private void NormalHorizontalMove() {
			//水平方向の方が入力値が大きいかどうか
			if(Mathf.Abs(m_InputVertical) <= Mathf.Abs(m_InputHorizontal)) {
				//水平方向の右側に傾いているかどうか
				if(m_InputHorizontal >= 0) {
					if(MoveCheck(Direction.Right)) {
						m_Target.x += m_MoveAmount;
						ArrayPosMove(m_MoveBlockAmount, 0);
					}
				} else {
					if(MoveCheck(Direction.Left)) {
						m_Target.x -= m_MoveAmount;
						ArrayPosMove(-m_MoveBlockAmount, 0);
					}
				}
			}
		}

		/// <summary>
		/// ラジオ式の移動
		/// </summary>
		private void RadioMove() {
			//どちらも入力判定がでているかどうか
			if(m_InputVerticalFlag && m_InputHorizontalFlag) {
				//水平方向の方が入力値が大きいかどうか
				if(Mathf.Abs(m_InputVertical) <= Mathf.Abs(m_InputHorizontal)) {
					RadioHorizontalMove();
				} else {
					RadioVertexMove();
				}
			}

			//水平方向のみ入力されているかどうか
			if(m_InputHorizontalFlag && !m_InputVerticalFlag) {
				RadioHorizontalMove();
			}

			//垂直方向のみ入力されているかどうか
			if(!m_InputHorizontalFlag && m_InputVerticalFlag) {
				RadioVertexMove();
			}
		}

		/// <summary>
		/// 垂直方向の移動（ラジオ）
		/// </summary>
		private void RadioVertexMove() {
			//垂直方向の上側に傾いているかどうか
			if(m_InputVertical >= 0) {
				switch(m_LookDirection) {
					case LookDirection.UP:
						if(MoveCheck(Direction.Top)) {
							m_Target.y += m_MoveAmount;
							ArrayPosMove(0, -m_MoveBlockAmount);
						}
						break;
					case LookDirection.LEFT:
						if(MoveCheck(Direction.Left)) {
							m_Target.x -= m_MoveAmount;
							ArrayPosMove(-m_MoveBlockAmount, 0);
						}
						break;
					case LookDirection.DOWN:
						if(MoveCheck(Direction.Bottom)) {
							m_Target.y -= m_MoveAmount;
							ArrayPosMove(0, m_MoveBlockAmount);
						}
						break;
					case LookDirection.RIGHT:
						if(MoveCheck(Direction.Right)) {
							m_Target.x += m_MoveAmount;
							ArrayPosMove(m_MoveBlockAmount, 0);
						}
						break;
				}
			} else {
				switch(m_LookDirection) {
					case LookDirection.UP:
						if(MoveCheck(Direction.Bottom)) {
							m_Target.y -= m_MoveAmount;
							ArrayPosMove(0, m_MoveBlockAmount);
						}
						break;
					case LookDirection.LEFT:
						if(MoveCheck(Direction.Right)) {
							m_Target.x += m_MoveAmount;
							ArrayPosMove(m_MoveBlockAmount, 0);
						}
						break;
					case LookDirection.DOWN:
						if(MoveCheck(Direction.Top)) {
							m_Target.y += m_MoveAmount;
							ArrayPosMove(0, -m_MoveBlockAmount);
						}
						break;
					case LookDirection.RIGHT:
						if(MoveCheck(Direction.Left)) {
							m_Target.x -= m_MoveAmount;
							ArrayPosMove(-m_MoveBlockAmount, 0);
						}
						break;
				}
			}

			m_IsMove = false;
		}

		/// <summary>
		/// 水平方向の移動（ラジオ）
		/// </summary>
		private void RadioHorizontalMove() {
			//水平方向の右側に傾いているかどうか
			if(m_InputHorizontal >= 0) {

				//右回りに方向転換
				switch(m_LookDirection) {
					case LookDirection.UP:
						m_LookDirection = LookDirection.RIGHT;
						break;
					case LookDirection.RIGHT:
						m_LookDirection = LookDirection.DOWN;
						break;
					case LookDirection.DOWN:
						m_LookDirection = LookDirection.LEFT;
						break;
					case LookDirection.LEFT:
						m_LookDirection = LookDirection.UP;
						break;

				}
			} else {
				//左回りに方向転換
				switch(m_LookDirection) {
					case LookDirection.UP:
						m_LookDirection = LookDirection.LEFT;
						break;
					case LookDirection.LEFT:
						m_LookDirection = LookDirection.DOWN;
						break;
					case LookDirection.DOWN:
						m_LookDirection = LookDirection.RIGHT;
						break;
					case LookDirection.RIGHT:
						m_LookDirection = LookDirection.UP;
						break;
				}
			}

			m_InputReturnWaitFlag = true;
		}

		/// <summary>
		/// スプライトの変更
		/// </summary>
		private void SpriteChange() {
			switch(m_LookDirection) {
				case LookDirection.UP:
					//m_SpriteRenderer.sprite = m_UpSprite;
					this.resetTriggerAll();
					this.animator.SetTrigger("Up");
					//Debug.Log("Player: Up");
					break;
				case LookDirection.DOWN:
					//m_SpriteRenderer.sprite = m_DownSprite;
					this.resetTriggerAll();
					this.animator.SetTrigger("Down");
					//Debug.Log("Player: Down");
					break;
				case LookDirection.LEFT:
					//m_SpriteRenderer.sprite = m_LeftSprite;
					this.resetTriggerAll();
					this.animator.SetTrigger("Left");
					//Debug.Log("Player: Left");
					break;
				case LookDirection.RIGHT:
					//m_SpriteRenderer.sprite = m_RightSprite;
					this.resetTriggerAll();
					this.animator.SetTrigger("Right");
					//Debug.Log("Player: Right");
					break;
			}
		}

		/// <summary>
		/// このオブジェクトの配列番号を変える
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void ArrayPosMove(int x, int y) {
			m_PlayerArrayPos.x += x;
			m_PlayerArrayPos.y += y;
		}

		/// <summary>
		/// すべてのアニメーショントリガーを解除
		/// </summary>
		private void resetTriggerAll() {
			this.animator.ResetTrigger("Left");
			this.animator.ResetTrigger("Down");
			this.animator.ResetTrigger("Right");
			this.animator.ResetTrigger("Up");
		}

		/// <summary>
		/// 移動を滑らかにするアニメーションコルーチン
		/// </summary>
		/// <returns></returns>
		private IEnumerator moveSmoothAnimation(Vector2 afterPosition) {
			var beforePosition = (Vector2)this.transform.position;
			var moveDelta = afterPosition - beforePosition;
			var oneDelta = moveDelta / (float)MoveAnimationFrame;

			for(int i = 0; i < MoveAnimationFrame; i++) {
				this.transform.position += new Vector3(oneDelta.x, oneDelta.y);
				yield return new WaitForEndOfFrame();
			}
		}

		#endregion Debug
	}

}
