using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [Serializable]
    public struct PickupProperties
    {
        public Pickup Prefab;
        public float  Chance;
    }

    public static  GameController     Instance    { get; private set; }
    public         int                Score
    {
        get => _score;
        set
        {
            _score = value;
            _scoreText.text = "Score: " + _score;
        }
    }

    public         bool               InvertControls
    {
        get => _invertControls;
        set
        {
            _invertControls = value;

            if (_invertControlsCoroutine != null)
                StopCoroutine(_invertControlsCoroutine);

            _invertControlsCoroutine = StartCoroutine(ResetInvertControls());
        }
    }

    public         bool               GameRunning { get; private set; } = true;

    [SerializeField]
    private        PickupProperties[] _pickupsProperties;
    [SerializeField]                                           
    private        BoxCollider        _floor                            = default;
    [SerializeField]                                                    
    private        float              _pickupRadius                     = 0.5f;
    [SerializeField]                                                    
    private        float              _invertDuration                   = 5.0f;
    [SerializeField]                                                    
    private        float              _gameplayTime                     = 15.0f;
    [SerializeField]
    private        TMP_Text           _scoreText;
    [SerializeField]
    private        TMP_Text           _remainingTimeText;
    [SerializeField]
    private        TMP_Text           _finalScoreText;
    [SerializeField]
    private        Canvas             _gameCanvas;
    [SerializeField]
    private        Canvas             _endingCanvas;

    private        int                _score;
    private        bool               _invertControls;
    private        Coroutine          _invertControlsCoroutine;
    private static List<Pickup>       InstantiatedPickups { get; }      = new();

    public void OnReplayButtonPressed() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void OnMainMenuButtonPressed() =>
        SceneManager.LoadScene("MainMenuScene");

    public static void RegisterPickup(Pickup pickup)
    {
        Debug.Assert(!InstantiatedPickups.Contains(pickup),
            "Duplicate registration of pickup!", pickup);

        InstantiatedPickups.Add(pickup);
    }

    public static void UnregisterPickup(Pickup pickup)
    {
        Debug.Assert(
            InstantiatedPickups.Contains(pickup),
            "Unregistration of unknown pickup!", pickup);

        InstantiatedPickups.Remove(pickup);
    }

    private void OnEnable() =>
        Instance = this;

    private void OnDisable() =>
        Instance = null;

    private void Update()
    {
        _gameplayTime -= Time.deltaTime;

        if (_gameplayTime <= 0.0f)
        {
            _gameplayTime = 0.0f;
            if (GameRunning)
            {
                _gameCanvas.gameObject.SetActive(false);
                _endingCanvas.gameObject.SetActive(true);
                _finalScoreText.text = "Score: " + _score;
                GameRunning = false;
            }
        }

        _remainingTimeText.text = $"Remaining time: {_gameplayTime:F2}";

        if (!GameRunning)
            return;

        if (InstantiatedPickups.Count < 1)
            SpawnPickup(RouletteWheelSelection());
    }

    private void SpawnPickup(Pickup prefab)
    {
        var pickup = Instantiate(prefab);
        var bounds = _floor.bounds;

        bounds.size -= new Vector3(_pickupRadius, 0.0f, _pickupRadius) * 2.0f;

        var position = new Vector3(
            Mathf.Lerp(bounds.min.x, bounds.max.x, Random.value),
            bounds.max.y + _pickupRadius,
            Mathf.Lerp(bounds.min.z, bounds.max.z, Random.value)
        );

        pickup.transform.position = position;
    }

    private IEnumerator ResetInvertControls()
    {
        yield return new WaitForSeconds(_invertDuration);
        _invertControlsCoroutine = null;
        _invertControls = false;
    }

    private Pickup RouletteWheelSelection()
    {
        var sum = _pickupsProperties
            .Select(x => x.Chance)
            .Sum();

        var point = Random.value * sum;
        var accumulator = 0.0f;

        foreach (var pickupProperty in _pickupsProperties)
        {
            accumulator += pickupProperty.Chance;
            if (accumulator >= point)
                return pickupProperty.Prefab;
        }

        return _pickupsProperties.First().Prefab;
    }
}
