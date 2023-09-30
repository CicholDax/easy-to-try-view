using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using ETTView.Math;
using ETTView.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;

public class GameView : UIView
{
    [SerializeField] PlayerNode _playerNode;
    [SerializeField] float _nodeCreateTimeSpan = 1.0f;

    [SerializeField] float _speed = 0.1f;
    [SerializeField] float _decay = 0.01f;
    [SerializeField] float _nodeLifeTime = 5.0f;

    [SerializeField] ResultView _resultView;

    VectorInertia _playerInertia;

    float _lastNodeCreateTime;

    public async void OnClickGameOverButton()
    {
        await _resultView.Open();
    }


    public override async UniTask Opening(CancellationToken token)
    {
        _playerInertia = new VectorInertia() { MinMagnitude = _speed };

        _lastNodeCreateTime = Time.time;
        _playerNode = await PlayerNode.Create(transform, Vector3.zero, _nodeLifeTime);
    }

    private async void Update()
    {
        if( Phase == ETTView.Reopener.PhaseType.Opened )
        {
            //àÍíËéûä‘Ç≤Ç∆Ç…êgëÃÇâÑÇŒÇ∑
            if (_lastNodeCreateTime + _nodeCreateTimeSpan <= Time.time)
            {
                _playerNode = await PlayerNode.Create(transform, _playerNode.transform.position, _nodeLifeTime);
                _lastNodeCreateTime = Time.time;
            }

            if(Input.GetKey(KeyCode.D))
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

            _playerNode.transform.position = _playerInertia.Reflect(_playerNode.transform.position, _decay);
        }

        
    }

}
