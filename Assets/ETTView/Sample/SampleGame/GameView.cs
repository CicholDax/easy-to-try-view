using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using ETTView.Math;
using ETTView.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ETTView.SampleGame
{
    public class GameView : UIView
    {
        [SerializeField] PlayerNode _playerNode;
        [SerializeField] float _nodeCreateTimeSpan = 1.0f;

        [SerializeField] Item _item;

        [SerializeField] float _speed = 0.1f;
        [SerializeField] float _decay = 0.01f;
        [SerializeField] float _nodeLifeTime = 5.0f;
        [SerializeField] Canvas _canvas;
        [SerializeField] CanvasGroup _canvasGroup;

        [SerializeField] UIViewState _testStateA;
        [SerializeField] UIViewState _testStateBPrefab;

        VectorInertia _playerInertia;
        float _lastNodeCreateTime;

        public async void OnClickGameOverButton()
        {
            //雑
            var canvas = GameObject.FindGameObjectWithTag("BootCanvas");

            if (canvas != null)
                ResultView.Create(canvas.transform).Forget();
        }


        public override async UniTask Opening(CancellationToken token)
        {
            _playerInertia = new VectorInertia() { MinMagnitude = _speed };

            _lastNodeCreateTime = Time.time;
            _playerNode = await PlayerNode.Create(transform, Vector3.zero, _nodeLifeTime);
        }

        public override async UniTask Closing(CancellationToken token)
        {
            await _canvasGroup.DOFade(0.0f, 2.0f);
        }

        private async void Update()
        {
            if (Phase == ETTView.Reopener.PhaseType.Opened)
            {
                //一定時間ごとに身体を延ばす
                if (_lastNodeCreateTime + _nodeCreateTimeSpan <= Time.time)
                {
                    _playerNode = await PlayerNode.Create(transform, _playerNode.transform.position, _nodeLifeTime);
                    _lastNodeCreateTime = Time.time;
                }

                //一定時間ごとに

                if (Input.GetKey(KeyCode.D))
                {
                    _playerInertia.AddSpeed(Vector3.right * _speed);

                }
                if (Input.GetKey(KeyCode.A))
                {
                    _playerInertia.AddSpeed(Vector3.left * _speed);

                }
                if (Input.GetKey(KeyCode.W))
                {
                    _playerInertia.AddSpeed(Vector3.up * _speed);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    _playerInertia.AddSpeed(Vector3.down * _speed);
                }

                if (Input.GetKeyDown(KeyCode.F1))
                {
                    _testStateA.Open().Forget();
                }
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    CreateFromPrefab(_testStateBPrefab, _testStateA.transform.parent);
                }

                _playerNode.transform.position = _playerInertia.Reflect(_playerNode.transform.position, _decay);

                //画面外チェック
                var viewPoint = Camera.main.WorldToViewportPoint(_playerNode.transform.position);

                Vector3 correctedViewportPoint = viewPoint;

                var hitNor = Vector3.zero;
                if (viewPoint.x > 1)
                {
                    correctedViewportPoint.x = 1;
                    hitNor = Vector3.left;
                }
                else if (viewPoint.x < 0)
                {
                    correctedViewportPoint.x = 0;
                    hitNor = Vector3.right;
                }

                if (viewPoint.y > 1)
                {
                    correctedViewportPoint.y = 1;
                    hitNor = Vector3.up;
                }
                else if (viewPoint.y < 0)
                {
                    correctedViewportPoint.y = 0;
                    hitNor = Vector3.down;
                }

                if (hitNor != Vector3.zero)
                {
                    Vector3 correctedWorldPoint = Camera.main.ViewportToWorldPoint(correctedViewportPoint);
                    correctedWorldPoint.z = _playerNode.transform.position.z; // オブジェクトの元のz座標を維持
                    _playerNode.transform.position = correctedWorldPoint;
                    _playerInertia.Speed = _playerInertia.Speed.WallReflect(hitNor);
                    _playerInertia.Speed *= 2.3f;
                }
            }


        }
    }
}