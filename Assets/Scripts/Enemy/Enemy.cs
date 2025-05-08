using Game;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Vector3 _destination;
        private SoundType _type;
        private GameObject _player;

        private void Awake()
        {
            GameManager.OnPlayerMadeSound += GetLastSoundPosition;
        }

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _destination = _player.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log((Vector3.Distance(transform.position, _destination)));

            switch (_type)
            {
                case SoundType.Normal:
                    if (Vector3.Distance(transform.position, _destination) < 15)
                    {
                        _agent.SetDestination(_destination);
                    }
                    break;
                case SoundType.Loud:
                    if (Vector3.Distance(transform.position, _destination) < 30)
                    {
                        _agent.SetDestination(_destination);
                    }
                    break;
                case SoundType.Quiet:
                    if (Vector3.Distance(transform.position, _destination) < 5)
                    {
                        _agent.SetDestination(_destination);
                    }
                    break;
            }
            
            
           
        }

        private void GetLastSoundPosition(SoundType type, Vector3 soundPosition)
        {
            _type = type;
            _destination = soundPosition;
            
        }
    }
}
