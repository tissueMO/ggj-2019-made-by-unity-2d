using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カードゲームのプレイヤー
/// </summary>
public class CardPlayer : MonoBehaviour {

	/// <summary>
	/// 手札の枚数
	/// </summary>
	public const int CardHandsCount = 5;

	/// <summary>
	/// 手札
	/// </summary>
	private int[] cardHands;

	/// <summary>
	/// 手札を除くデッキ
	/// </summary>
	private int[] cardUnit;

}
