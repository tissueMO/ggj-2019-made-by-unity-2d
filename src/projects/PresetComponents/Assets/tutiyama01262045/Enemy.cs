using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Roguelike
{
    using UnityEngine;
    using Assets.Scripts.PresetComponents.Roguelike.Interface;
    using Assets.Scripts.PresetComponents.Roguelike.SectionDivision;
    using static Assets.Scripts.PresetComponents.Roguelike.Interface.GeneratedMapBase;

    /// <summary>
    /// エネミーの操作挙動
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [SerializeField]
        private Sprite m_UpSprite;
        [SerializeField]
        private Sprite m_DownSprite;
        [SerializeField]
        private Sprite m_LeftSprite;
        [SerializeField]
        private Sprite m_RightSprite;

        private SpriteRenderer m_SpriteRenderer;  //このオブジェクトのSpriteRenderer

        private GeneratedMapBase m_GeneratedMapBase;
        private GeneratedMapTile[,] m_MapData;

        private Vector3 m_Target;  //移動先
        private Vector2Int m_ArrayPos;  //このオブジェクト配列内位置

        private float m_MoveAmount = 0.0f;  //移動するときの移動量

        private int m_MoveBlockAmount = 1;  //移動するときの移動ブロック数

        private int m_ChaseCheckBlockAmount = 3;  //追跡判別範囲のブロック数

        //プレイヤーが見ている方向
        private enum LookDirection
        {
            UP,
            DOWN,
            RIGHT,
            LEFT
        }

        private LookDirection m_LookDirection = LookDirection.UP;  //自分が向いている方向

        private bool m_IsMove = false;  //動けるかのフラグ

        private Player1 m_Player1;  //追いかけるプレイヤー

        public bool GetChaseFlag() { return m_ChaseFlag; }
        private bool m_ChaseFlag = false;  //追っているのか

        private bool m_MoveAnimationFlag = false;  //移動アニメーション中かどうか
        private int m_MoveAnimFrameMax = 60;     //移動アニメーションにかかるフレーム数
        private int m_OneSpriteFrame = 0;      //１スプライトにかかるフレーム数
        private int m_MoveAnimFrameCounter = 0;      //移動アニメーションのフレームカウンター


        //移動アニメーションで使用するスプライト配列
        [SerializeField]
        private Sprite[] m_UpMoveSprites;
        [SerializeField]
        private Sprite[] m_DownMoveSprites;
        [SerializeField]
        private Sprite[] m_LeftMoveSprites;
        [SerializeField]
        private Sprite[] m_RightMoveSprites;

        private bool m_InputReturnWaitFlag = false;

        #region Main

        /// <summary>
        /// このオブジェクトの移動を許可する
        /// </summary>
        public void StartTurn()
        {
            m_IsMove = true;
            m_ChaseFlag = false;
            //カウンターを初期化
            m_MoveAnimFrameCounter = m_MoveAnimFrameMax;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="map"></param>
        public void Initialize(GeneratedMapBase map)
        {
            //マップデータの取得←マップは変化しないということだったので処理速度的にあらかじめコピーします。
            this.m_GeneratedMapBase = map;
            m_MapData = m_GeneratedMapBase.TileData;
            m_ArrayPos = ((MapDataBySectionDivision)m_GeneratedMapBase).GetPlayerPosition();

            //スプライトレンダラーの取得
            m_SpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

            //1ブロック分進みたいので自分の横Scale / 移動にかかるフレーム数
            m_MoveAmount = this.transform.localScale.x / m_MoveAnimFrameMax;

            //１スプライトにかかるフレーム数の計算
            m_OneSpriteFrame = m_MoveAnimFrameMax / m_UpMoveSprites.Length;

            //追いかけるプレイヤーの取得
            m_Player1 = GameObject.FindObjectOfType<Player1>();

            //カウンターを初期化
            m_MoveAnimFrameCounter = m_MoveAnimFrameMax;

            //debug
            m_ArrayPos.x = m_Player1.m_PlayerArrayPos.x - 1;
            m_ArrayPos.y = m_Player1.m_PlayerArrayPos.y + 1;
            this.transform.position      = new Vector3(m_Player1.gameObject.transform.position.x - (this.transform.localScale.x * 1), m_Player1.gameObject.transform.position.y - (this.transform.localScale.x * 1), -0.5f);
            m_Player1.transform.position = new Vector3(m_Player1.transform.position.x, m_Player1.transform.position.y, -0.5f);
        }

        private void FixedUpdate()
        {
            //移動アニメーションをしているのかどうか
            if (m_MoveAnimationFlag)
            {
                //移動アニメーションの必要フレーム数以内かどうか
                if (m_MoveAnimFrameCounter <= 0)
                {
                    m_MoveAnimationFlag = false;
                    return;
                }

                //スプライトの変わり目かどうか
                if ((m_MoveAnimFrameCounter % m_OneSpriteFrame) == 0)
                {
                    //スプライトの配列の番号
                    int spriteNamber = (m_MoveAnimFrameCounter / m_OneSpriteFrame) - 1;

                    Debug.Log(spriteNamber);

                    //向いている方向によってアニメーションを変える
                    switch (m_LookDirection)
                    {
                        case LookDirection.UP:
                            m_SpriteRenderer.sprite = m_UpMoveSprites[spriteNamber];
                            break;
                        case LookDirection.DOWN:
                            m_SpriteRenderer.sprite = m_DownMoveSprites[spriteNamber];
                            break;
                        case LookDirection.LEFT:
                            m_SpriteRenderer.sprite = m_LeftMoveSprites[spriteNamber];
                            break;
                        case LookDirection.RIGHT:
                            m_SpriteRenderer.sprite = m_RightMoveSprites[spriteNamber];
                            break;
                    }
                }

                //現在の自分のポジションを保存
                m_Target = this.gameObject.transform.position;

                //向いている方向によって移動する
                switch (m_LookDirection)
                {
                    case LookDirection.UP:
                        m_Target.y += m_MoveAmount;
                        this.transform.position = m_Target;
                        break;
                    case LookDirection.DOWN:
                        m_Target.y -= m_MoveAmount;
                        this.transform.position = m_Target;
                        break;
                    case LookDirection.LEFT:
                        m_Target.x -= m_MoveAmount;
                        this.transform.position = m_Target;
                        break;
                    case LookDirection.RIGHT:
                        m_Target.x += m_MoveAmount;
                        this.transform.position = m_Target;
                        break;
                }

                //カウントダウン
                m_MoveAnimFrameCounter--;
            }
        }

        private void Update()
        {
            m_ChaseFlag = false;

            //動けるのかどうか
            if (m_IsMove)
            {
                //現在の自分のポジションを保存
                m_Target = this.gameObject.transform.position;

                //追跡するかしないかのチェック
                if(PlayerChaseCheck(m_Player1,m_ChaseCheckBlockAmount))
                {
                    m_ChaseFlag = true;
                    Chase(m_Player1);
                }
                else
                {
                    RandomMove();
                }
            }

            //debugcode
            if (Input.GetKeyDown(KeyCode.E) && !m_MoveAnimationFlag)
            {
                StartTurn();
            }

            //アニメーション中ではないかどうか
            if(!m_MoveAnimationFlag)
            {
                //スプライトを向きによって変更する
                SpriteChange();
            }
        }

        /// <summary>
        /// 移動できるかチェックする
        /// </summary>
        private bool MoveCheck(Direction direction)
        {
            //次に移動する配列番号の取得
            Vector2Int nextArrayPos = m_GeneratedMapBase.GetNextDirectionPosition(m_ArrayPos, direction);

            //次に移動する配列番地にあるタイルの取得
            GeneratedMapTile tile = m_MapData[nextArrayPos.x, nextArrayPos.y];

            bool checkFlag = true;  //チェック結果

            //タイルの種類ごとに処理判別
            switch (tile)
            {
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
        /// スプライトの変更
        /// </summary>
        private void SpriteChange()
        {
            switch (m_LookDirection)
            {
                case LookDirection.UP:
                    m_SpriteRenderer.sprite = m_UpSprite;
                    break;
                case LookDirection.DOWN:
                    m_SpriteRenderer.sprite = m_DownSprite;
                    break;
                case LookDirection.LEFT:
                    m_SpriteRenderer.sprite = m_LeftSprite;
                    break;
                case LookDirection.RIGHT:
                    m_SpriteRenderer.sprite = m_RightSprite;
                    break;
            }
        }

        /// <summary>
        /// このオブジェクトの配列番号を変える
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ArrayPosMove(int x, int y)
        {
            m_ArrayPos.x += x;
            m_ArrayPos.y += y;
        }

        /// <summary>
        /// 追跡状態でないときのランダムな移動
        /// </summary>
        private void RandomMove()
        {
            //次に進む方向を選ぶための乱数
            int random = Random.Range(0,4);
            Debug.Log(random);
            switch(random)
            {
                case 0:
                    //上に移動
                    if (MoveCheck(Direction.Top))
                    {
                        m_LookDirection = LookDirection.UP;
                        m_MoveAnimationFlag = true;
                        ArrayPosMove(0, -m_MoveBlockAmount);

                        m_IsMove = false;
                    }
                    break;
                case 1:
                    //下に移動
                    if (MoveCheck(Direction.Bottom))
                    {
                        m_LookDirection = LookDirection.DOWN;
                        m_MoveAnimationFlag = true;
                        ArrayPosMove(0, m_MoveBlockAmount);

                        m_IsMove = false;
                    }
                    break;
                case 2:
                    //左に移動
                    if (MoveCheck(Direction.Left))
                    {
                        m_LookDirection = LookDirection.LEFT;
                        m_MoveAnimationFlag = true;
                        ArrayPosMove(-m_MoveBlockAmount, 0);

                        m_IsMove = false;
                    }
                    break;
                case 3:
                    //右に移動
                    if (MoveCheck(Direction.Right))
                    {
                        m_LookDirection = LookDirection.RIGHT;
                        m_MoveAnimationFlag = true;
                        ArrayPosMove(m_MoveBlockAmount, 0);

                        m_IsMove = false;
                    }
                    break;

            }
        }

        /// <summary>
        /// プレイヤーが追跡範囲以内にないかどうか
        /// </summary>
        /// <param name="player">調査するプレイヤー</param>
        /// <param name="range">調査するマス数（縦横斜め）</param>
        private bool PlayerChaseCheck(Player1 player1, int range)
        {
            //プレイヤーの配列内番号の取得
            Vector2Int playerArrayPos = player1.m_PlayerArrayPos;

            //範囲内にいるのかどうか
            if (playerArrayPos.x <= m_ArrayPos.x + range && playerArrayPos.x >= m_ArrayPos.x - range
                && playerArrayPos.y <= m_ArrayPos.y + range && playerArrayPos.y >= m_ArrayPos.y - range)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 追う
        /// </summary>
        /// <param name="player">追う対象のプレイヤー</param>
        private void Chase(Player1 player1)
        {
            //プレイヤーの配列内番号の取得
            Vector2Int playerArrayPos = player1.m_PlayerArrayPos;
            bool moveXFlag  = false;  //X軸に移動するかのフラグ
            bool moveYFlag  = false;  //Y軸に移動するかのフラグ
            bool attackFlag = false;  //攻撃する

            //X軸が同じかどうか
            if (m_ArrayPos.x == playerArrayPos.x)
            {
                //Y軸が隣接しているかどうか
                if ((m_ArrayPos.y + 1) == playerArrayPos.y || (m_ArrayPos.y - 1) == playerArrayPos.y)
                {
                    //攻撃するためフラグを立てる
                    attackFlag = true;

                    if((m_ArrayPos.y + 1) == playerArrayPos.y)
                    {
                        m_LookDirection = LookDirection.DOWN;
                    }
                    else
                    {
                        m_LookDirection = LookDirection.UP;
                    }
                }
                else
                {
                    //Y軸を移動させるためフラグを立てる
                    moveYFlag = true;
                }
            }

            //Y軸が同じかどうか
            if (m_ArrayPos.y == playerArrayPos.y)
            {
                //X軸が隣接しているかどうか
                if ((m_ArrayPos.x + 1) == playerArrayPos.x || (m_ArrayPos.x - 1) == playerArrayPos.x)
                {
                    //攻撃するためフラグを立てる
                    attackFlag = true;

                    if((m_ArrayPos.x + 1) == playerArrayPos.x)
                    {
                        m_LookDirection = LookDirection.RIGHT;
                    }
                    else
                    {
                        m_LookDirection = LookDirection.LEFT;
                    }
                }
                else
                {
                    //X軸を移動させるためのフラグを立てる
                    moveXFlag = true;
                }
            }

            //ここまでどちらも移動フラグが立っていなかったら
            if(!moveXFlag && !moveYFlag && !attackFlag)
            {
                //Y座標を優先してそろえていく
                moveYFlag = true;
            }

            //攻撃フラグが立っているかどうか
            if(attackFlag)
            {
                Debug.Log("Attack");
                Attack(player1);
                m_IsMove = false;
            }

            //X軸の移動フラグが立っているかどうか
            if(moveXFlag)
            {
                //プレイヤーとの位置の比較（プレイヤーが右側にあるかどうか）
                if(m_ArrayPos.x <= player1.m_PlayerArrayPos.x)
                {
                    //右に移動
                    if (MoveCheck(Direction.Right))
                    {
                        m_LookDirection = LookDirection.RIGHT;
                        m_MoveAnimationFlag = true;
                        ArrayPosMove(m_MoveBlockAmount, 0);

                        m_IsMove = false;
                    }
                }
                else
                {
                    //左に移動
                    if (MoveCheck(Direction.Left))
                    {
                        m_LookDirection = LookDirection.LEFT;
                        m_MoveAnimationFlag = true;
                        ArrayPosMove(-m_MoveBlockAmount, 0);

                        m_IsMove = false;
                    }
                }
            }

            //Y軸の移動フラグが立っているかどうか
            if (moveYFlag)
            {
                //プレイヤーとの位置の比較（プレイヤーが右側にあるかどうか）
                if (m_ArrayPos.y <= player1.m_PlayerArrayPos.y)
                {
                    //下に移動
                    if (MoveCheck(Direction.Bottom))
                    {
                        m_LookDirection = LookDirection.DOWN;
                        m_MoveAnimationFlag = true;
                        ArrayPosMove(0, m_MoveBlockAmount);

                        m_IsMove = false;
                    }
                }
                else
                {
                    //上に移動
                    if (MoveCheck(Direction.Top))
                    {
                        m_LookDirection = LookDirection.UP;
                        m_MoveAnimationFlag = true;
                        ArrayPosMove(0, -m_MoveBlockAmount);

                        m_IsMove = false;
                    }
                }
            }
        }

        /// <summary>
        /// 攻撃
        /// </summary>
        /// <param name="player">攻撃する対象（プレイヤー）</param>
        private void Attack(Player1 player1)
        {
            player1.Damage();
        }

        /// <summary>
        /// ダメージ処理
        /// </summary>
        public void Damage()
        {
            //Enemyは声が出たりとかする？
        }

        #endregion Debug
    }
}


