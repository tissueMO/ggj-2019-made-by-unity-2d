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

	/// <summary>
	/// プレイヤー１の操作挙動
	/// </summary>
	public class Player1 : MonoBehaviour {
		[SerializeField]
		private GeneratedMapBase m_GeneratedMapBase;
		private GeneratedMapTile[,] m_MapData;

		private float m_InputHorizontal = 0.0f;   //水平方向の入力
		private float m_InputVertical = 0.0f;   //垂直方向の入力
		private float m_InputThreshold = 0.5f;   //入力のしきい値
		private bool m_InputHorizontalFlag = false;  //水平方向に入力できているのか
		private bool m_InputVerticalFlag = false;  //垂直方向に入力できているのか

		private Vector3 m_Target;  //移動先
		private Vector2Int m_PlayerArrayPos;  //現在のプレイヤーの配列内位置

		private float m_MoveAmount = 0.0f;  //移動するときの移動量

		[SerializeField]
		private float m_Step = 2f;  //動くスピード

		//プレイヤーが見ている方向
		private enum LookDirection {
			UP,
			DOWN,
			RIGHT,
			LEFT
		}

		private LookDirection m_LookDirection = LookDirection.UP;  //自分が向いている方向
		public bool m_IsMove = false;  //動けるかのフラグ
		
		#region Main

		public void Initialize(GeneratedMapBase map) {
			//マップデータの取得←マップは変化しないということだったので処理速度的にあらかじめコピーします。
			this.m_GeneratedMapBase = map;
			m_MapData = m_GeneratedMapBase.TileData;
			m_PlayerArrayPos = ((MapDataBySectionDivision)m_GeneratedMapBase).GetPlayerPosition();  //架空のアクセサーで代用

			m_MoveAmount = this.transform.localScale.x;
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

			//動けるのかどうか
			if(m_IsMove) {
				//現在の自分のポジションを保存
				m_Target = this.gameObject.transform.position;

				//NormalMove();
				RadioMove();

				//移動
				this.transform.position = Vector3.MoveTowards(this.transform.position, m_Target, m_Step * Time.deltaTime);

				m_IsMove = false;
			} else {
				if(!m_InputVerticalFlag && !m_InputHorizontalFlag) {
					m_IsMove = true;
				}
			}
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
					break;

				//床だった場合
				case GeneratedMapTile.Floor:
					checkFlag = true;
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
				}
			} else {
				if(MoveCheck(Direction.Bottom)) {
					m_Target.y -= m_MoveAmount;
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
					}
				} else {
					if(MoveCheck(Direction.Left)) {
						m_Target.x -= m_MoveAmount;
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
			if(m_InputHorizontal >= 0) {
				switch(m_LookDirection) {
					case LookDirection.UP:
						if(MoveCheck(Direction.Top)) {
							m_Target.y += m_MoveAmount;
						}
						break;
					case LookDirection.LEFT:
						if(MoveCheck(Direction.Left)) {
							m_Target.x -= m_MoveAmount;
						}
						break;
					case LookDirection.DOWN:
						if(MoveCheck(Direction.Bottom)) {
							m_Target.y -= m_MoveAmount;
						}
						break;
					case LookDirection.RIGHT:
						if(MoveCheck(Direction.Right)) {
							m_Target.x += m_MoveAmount;
						}
						break;
				}
			} else {
				switch(m_LookDirection) {
					case LookDirection.UP:
						if(MoveCheck(Direction.Bottom)) {
							m_Target.y -= m_MoveAmount;
						}
						break;
					case LookDirection.LEFT:
						if(MoveCheck(Direction.Right)) {
							m_Target.x += m_MoveAmount;
						}
						break;
					case LookDirection.DOWN:
						if(MoveCheck(Direction.Top)) {
							m_Target.y += m_MoveAmount;
						}
						break;
					case LookDirection.RIGHT:
						if(MoveCheck(Direction.Left)) {
							m_Target.x -= m_MoveAmount;
						}
						break;
				}
			}
		}

		/// <summary>
		/// 水平方向の移動（ラジオ）
		/// </summary>
		private void RadioHorizontalMove() {
			//水平方向の右側に傾いているかどうか
			if(m_InputHorizontal >= 0) {
				if(MoveCheck(Direction.Right)) {
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
		}
		
		#endregion Debug
	}

}
