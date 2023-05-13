using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ETTView
{
	[RequireComponent(typeof(Reopener))]
	public class Reopnable : MonoBehaviour
	{
		Reopener _reopener;
		Reopener Reopener
		{
			get
			{
				if (_reopener == null)
				{
					_reopener = GetComponent<Reopener>();
					if (_reopener == null)
					{
						_reopener = gameObject.AddComponent<Reopener>();
					}
				}
				return _reopener;
			}
		}

		public Reopener.StateType State { get { return Reopener.State; } }
		public bool IsOpen { get => Reopener.enabled; }
		public bool IsStateStable { get => State == Reopener.StateType.Closed || State == Reopener.StateType.Opened || State == Reopener.StateType.Loaded; }

		public virtual async UniTask Open()
		{
			await Reopener.Open();
		}

		public virtual async UniTask Close(bool destroy = false)
		{
			await Reopener.Close();
			if (destroy && gameObject != null) Destroy(gameObject);
		}

		//生成時に一度だけ実行する処理
		//Ex.付随するプレハブの生成、初期化とか
		public virtual async UniTask Load()
		{
		}

		//スタート、再スタート時に毎回実行する処理のうち、他より先に行いたいもの
		//Ex.状態の初期化とか、データの更新とか
		public virtual async UniTask Preopning()
		{
		}

		//スタート、再スタート時に毎回実行する処理
		//Ex.開始時のアニメーションなど
		public virtual async UniTask Opening()
		{
		}

		//終了時の処理
		//Ex.終了時のアニメーションとか
		public virtual async UniTask Closing()
		{
		}
	}
}