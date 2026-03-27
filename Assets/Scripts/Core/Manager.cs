using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] private Dungeon _dungeonPrefab;
        [SerializeField] private int _roomCount;

        private Camera _camera;
        private InputField _roomCountInput;
        private Dungeon _dungeon;

        private const int MinRoomRadius = 20;

        public void Awake()
        {
            _camera = GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<Camera>();

            _roomCountInput = GameObject.FindGameObjectWithTag("RoomCountInput")
                .GetComponent<InputField>();

            _roomCountInput.text = _roomCount.ToString();
            _roomCountInput.onValueChanged.AddListener(SetRoomCount);
        }

        public void Start() => CreateInstance();

        public void CreateInstance()
        {
            if (_dungeon != null) Destroy(_dungeon.gameObject);

            Stopwatch stopwatch = new();
            stopwatch.Start();
            int roomRadius = GetRoomRadius();

            _dungeon = Instantiate(_dungeonPrefab);
            _dungeon.Initialize(_roomCount, roomRadius);

            stopwatch.Stop();
            Debug.Log($"Execution time: {stopwatch.ElapsedMilliseconds}");

            _camera.transform.localPosition = new Vector3(roomRadius, roomRadius, roomRadius / 2);
        }

        private int GetRoomRadius() => Mathf.Clamp(_roomCount, MinRoomRadius, int.MaxValue);

        public void SetRoomCount(string input) => _roomCount = int.Parse(input);
    }

}