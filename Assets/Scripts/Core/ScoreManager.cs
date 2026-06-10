using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private float scoreRate = 10f;

    private void Update()
    {
        if (!DinoGameManager.instance.isPlaying) return;

        float speedMultiplier = DinoGameManager.instance.CurrentSpeed / 5f;
        float scoreToAdd = scoreRate * Time.deltaTime * speedMultiplier;
        DinoGameManager.instance.AddScore(scoreToAdd);
    }
}
