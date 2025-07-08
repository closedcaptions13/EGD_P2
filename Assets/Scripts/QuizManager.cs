using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class QuizManager : MonoBehaviour
{
    // [SerializeField] private TMP_Text questionBox;
    // [SerializeField] private TMP_Text answerBox1;
    // [SerializeField] private TMP_Text answerBox2;
    // [SerializeField] private TMP_Text answerBox3;
    // [SerializeField] private TMP_Text answerBox4;

    public TextAsset questionsJson;

    // private Dictionary<String, String> allQuestions;

    [System.Serializable]
    public class Question
    {
        public string question;
        public string correctAnswer;
        public string[] incorrectAnswers;
    }

    [System.Serializable]
    public class QuestionList
    {
        public Question[] question;
    }
    public QuestionList questionList = new QuestionList();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        questionList = JsonUtility.FromJson<QuestionList>(questionsJson.text);
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     getNextQuestion();
    // }

    // private void getNextQuestion()
    // {
        
    // }
}
