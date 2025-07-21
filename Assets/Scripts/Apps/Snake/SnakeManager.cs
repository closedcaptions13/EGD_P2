using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SnakeManager : MonoBehaviour
{
    [SerializeField] int fieldWidth;
    [SerializeField] int fieldHeight;

    [SerializeField] float gridSize;
    [SerializeField] float updateRate;

    [SerializeField] SmoothLineRenderer snakeRenderer;
    [SerializeField] SpriteRenderer foodRenderer;
    [SerializeField] GameObject snakeSprite;

    [SerializeField] private GameObject DeathScreen;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text scoreText;

    Vector2Int food;
    List<Vector2Int> snake = new();
    List<Vector2Int> snakeDeltas = new();

    private int foodEaten;

    Vector2Int previousDirection;
    Vector2Int direction;

    float timeOfLastUpdate;
    bool isMoving = false;
    bool justGrew = false;

    float UpdateOffset => (Time.time - timeOfLastUpdate) / updateRate;

    Vector3 GridToWorld(Vector2 xy)
        => (Vector3)(xy - new Vector2((float)fieldWidth / 2, (float)fieldHeight / 2)) * gridSize
         + AppRoot.ForObject(this).transform.position;

    void ResetToStart()
    {
        foodEaten = 0;
        scoreText.text = "Current Score: " + foodEaten;

        snake = new()
        {
            new(fieldWidth / 2, fieldHeight / 2),
            new(fieldWidth / 2, fieldHeight / 2 + 1),
        };

        snakeDeltas = new()
        {
            new(0, 1),
            new(0, 1)
        };

        direction = previousDirection = Vector2Int.zero;

        RegenerateFood();
    }

    void Start()
    {
        DeathScreen.SetActive(false);
    }

    void Awake()
    {
        // Initialize the game state //
        ResetToStart();

        // Update the snake at the provided update rate
        InvokeRepeating(nameof(SnakeUpdate), 0, updateRate);
    }

    readonly List<Vector2> snakeRenderedPoints = new();

    void RenderSnake()
    {
        snakeRenderedPoints.Clear();

        for (var i = 0; i < snake.Count; i++)
        {
            var offset = Vector2.zero;

            if (isMoving)
            {
                if (i == 0)
                {
                    offset = gridSize * (UpdateOffset - 1) * (Vector2)snakeDeltas[0];
                }
            }

            snakeRenderedPoints.Add(GridToWorld(snake[i]) + (Vector3)offset);

            // Handle wrap-around //
            if (i != snake.Count - 1)
            {
                var delta = snake[i + 1] - snake[i];
                var absDelta = delta;
                absDelta.x = Mathf.Abs(delta.x);
                absDelta.y = Mathf.Abs(delta.y);

                if (Mathf.Max(absDelta.x, absDelta.y) > 1)
                {
                    var afterThis = snake[i];
                    var beforeNext = snake[i + 1];

                    if (delta.x != 0)
                    {
                        if (delta.x < 0)
                        {
                            afterThis += Vector2Int.right;
                            beforeNext -= Vector2Int.right;
                        }
                        else
                        {
                            afterThis += Vector2Int.left;
                            beforeNext -= Vector2Int.left;
                        }
                    }
                    else
                    {
                        if (delta.y < 0)
                        {
                            afterThis += Vector2Int.up;
                            beforeNext -= Vector2Int.up;
                        }
                        else
                        {
                            afterThis += Vector2Int.down;
                            beforeNext -= Vector2Int.down;
                        }
                    }

                    snakeRenderedPoints.Add(GridToWorld(afterThis));
                    snakeRenderedPoints.Add(SmoothLineRenderer.BreakPoint);
                    snakeRenderedPoints.Add(GridToWorld(beforeNext));
                }
            }
        }

        if (UpdateOffset < 0.95f && isMoving && !justGrew)
        {
            var offset = gridSize * (UpdateOffset - 1) * (Vector2)snakeDeltas[^1];
            snakeRenderedPoints.Add(GridToWorld(snake[^1]) + (Vector3)offset);
        }

        snakeRenderer.BuildFromPoints(snakeRenderedPoints);
    }

    void Update()
    {
        // Update visuals //
        foodRenderer.transform.position = GridToWorld(food);

        RenderSnake();

        // Handle core movement logic //
        if (!AppRoot.SelectedForObject(this))
            return;

        var targetDirection = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W))
            targetDirection = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.A))
            targetDirection = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.S))
            targetDirection = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.D))
            targetDirection = Vector2Int.right;

        if (targetDirection != Vector2Int.zero && targetDirection != -previousDirection)
        {
            direction = targetDirection;
            isMoving = true;
        }
    }

    void RegenerateFood()
    {
        do
        {
            food = new(
                Random.Range(0, fieldWidth),
                Random.Range(0, fieldHeight)
            );
        }
        while (snake.Contains(food) && snake.Count < fieldWidth * fieldHeight);
    }

    void SnakeUpdate()
    {
        timeOfLastUpdate = Time.time;
        justGrew = false;

        if (!AppRoot.SelectedForObject(this))
            return;

        if (direction != Vector2Int.zero)
        {
            var newHead = snake[0] + direction;
            newHead.x += fieldWidth;
            newHead.x %= fieldWidth;
            newHead.y += fieldHeight;
            newHead.y %= fieldHeight;

            if (snake.Contains(newHead))
            {
                isMoving = false;

                Death();
                //ResetToStart();

                //Debug.Log("Snake died");

                return;
            }

            snake.Insert(0, newHead);
            snakeDeltas.Insert(0, direction);

            if (newHead == food)
            {
                // Regenerate new food location //
                foodEaten += 1;
                scoreText.text = "Current Score: " + foodEaten;

                RegenerateFood();
                justGrew = true;
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
                snakeDeltas.RemoveAt(snakeDeltas.Count - 1);
            }
        }

        previousDirection = direction;
    }

    void Death()
    {
        finalScoreText.text = "Total Eaten: " + foodEaten;
        DeathScreen.SetActive(true);
        foodRenderer.enabled = false;
        snakeSprite.SetActive(false);
    }

    public void Restart()
    {
        DeathScreen.SetActive(false);
        foodRenderer.enabled = true;
        snakeSprite.SetActive(true);

        ResetToStart();
    }
}
