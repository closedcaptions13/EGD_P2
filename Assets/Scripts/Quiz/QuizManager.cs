using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject questionBox;
    [SerializeField] private GameObject answerBox1;
    [SerializeField] private GameObject answerBox2;
    [SerializeField] private GameObject answerBox3;
    [SerializeField] private GameObject answerBox4;
    [SerializeField] private TMP_Text resultBox;

    [SerializeField] private GameObject dragDropQuestion;

    public TextAsset questionsJson;

    private int num;

    private int finalScore;


    [Serializable]
    public class Question
    {
        public string problem;
        public string correct;
        public string[] answers;
    }

    [Serializable]
    public class QuestionList
    {
        public Question[] question;
    }
    public QuestionList questionList = new QuestionList();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        questionList = JsonUtility.FromJson<QuestionList>(questionsJson.text);

        finalScore = 0;
        num = 0;
        updateQuestion();

        answerBox1.GetComponent<Button>().onClick.AddListener(() => chooseAnswer(answerBox1.GetComponent<Button>()));
        answerBox2.GetComponent<Button>().onClick.AddListener(() => chooseAnswer(answerBox2.GetComponent<Button>()));
        answerBox3.GetComponent<Button>().onClick.AddListener(() => chooseAnswer(answerBox3.GetComponent<Button>()));
        answerBox4.GetComponent<Button>().onClick.AddListener(() => chooseAnswer(answerBox4.GetComponent<Button>()));
    }


    private void updateQuestion()
    {
        questionBox.GetComponent<TMP_Text>().text = questionList.question[num].problem;
        answerBox1.GetComponentInChildren<TextMeshProUGUI>().text = questionList.question[num].answers[0];
        answerBox2.GetComponentInChildren<TextMeshProUGUI>().text = questionList.question[num].answers[1];
        answerBox3.GetComponentInChildren<TextMeshProUGUI>().text = questionList.question[num].answers[2];
        answerBox4.GetComponentInChildren<TextMeshProUGUI>().text = questionList.question[num].answers[3];
    }

    private void chooseAnswer(Button buttonNum)
    {
        if (buttonNum.GetComponentInChildren<TextMeshProUGUI>().text == questionList.question[num].correct)
        {
            UpdateFinalScore();
        }

        num++;
        if (num < questionList.question.Length)
        {
            updateQuestion();
        }
        else if (num == questionList.question.Length)
        {
            questionBox.SetActive(false);
            answerBox1.SetActive(false);
            answerBox2.SetActive(false);
            answerBox3.SetActive(false);
            answerBox4.SetActive(false);

            dragDropQuestion.SetActive(true);
        }
    }

    public void SubmitQuiz()
    {
        dragDropQuestion.SetActive(false);

        double percentage = (double)finalScore / 13 * 100;
        resultBox.text = "Final Score: " + finalScore + "/13\n\n" + string.Format("{0:F0}", percentage) + "%";
    }

    public void UpdateFinalScore()
    {
        finalScore++;
    }
}
