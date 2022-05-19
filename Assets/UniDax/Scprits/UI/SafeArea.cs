using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace UniDax.UI
{
	/// <summary>
	///UIのセーフエリア調整クラス
	/// </summary>
	public class SafeArea : MonoBehaviour
	{
		/// <summary>上下のセーフエリアを無視する</summary>
		[SerializeField] bool _ignoreTopBottom = true;
		private int prevScreenWidth;
		private int prevScreenHeight;
		private Rect prevSafeArea;
		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Awake()
		{
		}
		public void Update()
		{
			if (isSafeAreaChanged)
			{
				DoAdjust();
			}
		}
		private bool isSafeAreaChanged
		{
			get
			{
				return (prevScreenWidth != Screen.width)
					|| (prevScreenHeight != Screen.height)
					|| (prevSafeArea != Screen.safeArea);
			}
		}
		private void DoAdjust()
		{
			var safeArea = Screen.safeArea;
			var padLeft = safeArea.xMin;
			var padRight = Screen.width - safeArea.xMax;
			var padBottom = safeArea.yMin;
			var padTop = Screen.height - safeArea.yMax;
			if (_ignoreTopBottom)
			{
				padBottom = padTop = 0;
			}
			//主にSafeAreaを正しく取得できないAndroid用の処理
			if (Application.platform == RuntimePlatform.Android)
			{
				const float BaseAspectRatio = 16.0f / 9.0f;
				float screenAspectRatio = (float)Screen.width / Screen.height;
				//16:9よりも横長(横持ち基準で)の場合
				if (screenAspectRatio > BaseAspectRatio)
				{
					//16:9を超える分の幅の半分
					var padLeftRight1 = (Screen.width - (Screen.height * BaseAspectRatio)) / 0.5f;
					//画面幅の6%.
					var padLeftRight2 = Screen.width * 0.06f;
					//上記2つの小さい方 少なくともこの値分だけ左右にSafeArea外領域を取る
					var padLeftRight = Mathf.Min(padLeftRight1, padLeftRight2);
					//Screen.safeAreaから算出したSafeArea外領域がpadLeftRightに満たなかったら上書きする
					padLeft = Mathf.Max(padLeft, padLeftRight);
					padRight = Mathf.Max(padRight, padLeftRight);
				}
			}
			prevScreenWidth = Screen.width;
			prevScreenHeight = Screen.height;
			prevSafeArea = Screen.safeArea;

			var rectt = GetComponent<RectTransform>();
			if (rectt != null)
			{
				rectt.anchorMin = Vector2.zero;
				rectt.anchorMax = Vector2.one;
				rectt.offsetMin = new Vector2(padLeft, padBottom);
				rectt.offsetMax = new Vector2(-padRight, -padTop);
			}
		}
	}
}