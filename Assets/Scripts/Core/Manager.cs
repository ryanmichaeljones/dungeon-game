using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] private Dungeon _dungeonPrefab;
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private int _roomCount;

        private Camera _camera;
        private CameraFollow _cameraFollow;
        private InputField _roomCountInput;
        private Dungeon _dungeon;
        private Player _player;

        private const int MinRoomRadius = 20;

        public void Awake()
        {
            _camera = GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<Camera>();
            _cameraFollow = _camera.GetComponent<CameraFollow>();

            _camera.orthographic = true;
            _camera.orthographicSize = 8f;
            _camera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            _roomCountInput = GameObject.FindGameObjectWithTag("RoomCountInput")
                .GetComponent<InputField>();

            _roomCountInput.text = _roomCount.ToString();
            _roomCountInput.onValueChanged.AddListener(SetRoomCount);
        }

        public void Start() => CreateInstance();

        public void CreateInstance()
        {
            if (_dungeon != null) Destroy(_dungeon.gameObject);
            if (_player != null) Destroy(_player.gameObject);

            Stopwatch stopwatch = new();
            stopwatch.Start();
            int roomRadius = GetRoomRadius();

            _dungeon = Instantiate(_dungeonPrefab);
            _dungeon.Initialize(_roomCount, roomRadius);

            Coordinate startRoom = _dungeon.GetStartRoomCenter();
            _player = Instantiate(_playerPrefab, new Vector3(startRoom.X, 0.5f, startRoom.Z), Quaternion.identity);
            _cameraFollow.SetTarget(_player.transform);

            stopwatch.Stop();
            Debug.Log($"Execution time: {stopwatch.ElapsedMilliseconds}");
        }

        private int GetRoomRadius() => Mathf.Max(_roomCount * 3, MinRoomRadius);

        public void SetRoomCount(string input) => _roomCount = int.Parse(input);
    }
}