using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using UniDax;

namespace UniDax.UI
{
	[RequireComponent(typeof(Reopener))]
	public abstract class Reopnable : MonoBehaviour
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
		protected bool ReopenerEnable { get => Reopener.enabled; }

		public async UniTask Open()
		{
			await Reopener.Open();
		}

		public async UniTask Close(bool destroy = false)
		{
			await Reopener.Close();

			if (destroy) Destroy(gameObject);
		}

		//生成時に一度だけ実行する処理
		//Ex.付随するプレハブの生成、初期化とか
		public virtual async UniTask Load()
		{
			//await UniTask.WaitForEndOfFrame();
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
			//await UniTask.WaitForEndOfFrame();
		}

		//更新処理
		public virtual async UniTask OnLoadedUpdate()
		{
			//await UniTask.Yield(PlayerLoopTiming.Update);
		}

		//終了時の処理
		//Ex.終了時のアニメーションとか
		public virtual async UniTask Closing()
		{
			//await UniTask.WaitForEndOfFrame();
		}
	}
}