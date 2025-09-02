using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ETTView
{
	[RequireComponent(typeof(Reopener))]
	public class Reopenable : MonoBehaviour
	{
		Reopener _reopener;
		Reopener Reopener
		{
			get
			{
				if (this != null && gameObject != null)
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
				else
				{
					return null;
				}
			}
		}

		public Reopener.PhaseType Phase { get { return Reopener?.Phase ?? Reopener.PhaseType.Closed; } }
		public bool IsOpen { get => Reopener?.enabled ?? false; }
		public bool IsPhaseStable { get => Phase == Reopener.PhaseType.Closed || Phase == Reopener.PhaseType.Opened || Phase == Reopener.PhaseType.Loaded; }

		public virtual async UniTask Open()
		{
			if (Reopener != null)
			{
				await Reopener.Open();
			}
		}

		public void OpenNowait()
		{
			Open().Forget();
		}

		public virtual async UniTask Close(bool destroy = false)
        {
            if (Reopener != null)
            {
                await Reopener.Close();
            }
            if (destroy && this != null && gameObject != null)
			{
				//Debug.Log(name + "が破棄されました。");

				if (this != null && gameObject != null)
					Destroy(gameObject);
			}
		}

		public void CloseNowait(bool destroy = false)
		{
			Close(destroy).Forget();
		}

		//生成時に一度だけ実行する処理
		//Ex.付随するプレハブの生成、初期化とか
		public virtual async UniTask Loading(CancellationToken token)
		{
		}

		//スタート、再スタート時に毎回実行する処理のうち、他より先に行いたいもの
		//Ex.状態の初期化とか、データの更新とか
		public virtual async UniTask Preopning(CancellationToken token)
		{
		}

		//スタート、再スタート時に毎回実行する処理
		//Ex.開始時のアニメーションなど
		public virtual async UniTask Opening(CancellationToken token)
		{
		}

		//終了時の処理
		//Ex.終了時のアニメーションとか
		public virtual async UniTask Closing(CancellationToken token)
		{
		}
	}
}