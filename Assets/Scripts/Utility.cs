using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Utility : MonoBehaviour
{   
    
    private Color[] preColor = {
        new Color(1f, 0.4f, 0.4f),  // Bright Red
        new Color(0.4f, 1f, 0.4f),  // Bright Green
        new Color(0.4f, 0.6f, 1f),  // Bright Blue
        new Color(1f, 1f, 0.4f),    // Bright Yellow
        new Color(0.4f, 1f, 1f),    // Bright Cyan
        new Color(1f, 0.4f, 1f),    // Bright Magenta
        new Color(0.8f, 0.8f, 0.8f),// Light Silver
        new Color(1f, 0.6f, 0.2f),  // Vibrant Orange
        new Color(0.6f, 0.4f, 1f),  // Bright Purple
        new Color(0.7f, 1f, 0.7f)   // Vibrant Mint
    };

    private string equationQuestion ="";
    public string curAnswer = "";

    public GameObject questionBoard=null;

    public GameObject ratTemplate=null;

    public Transform headPos=null;

    void Start()
    {
        GameObject[] Rats = GameObject.FindGameObjectsWithTag("Rat");
        for(int i=0;i<Rats.Length;i++){
            Rats[i%preColor.Length].GetComponent<SpriteRenderer>().color = preColor[i%preColor.Length];
        }   

        generateQuestion();
    }

    public bool tryToAnswer(string ans){
        return this.curAnswer==ans;
    }

    public void generateQuestion(){
        string [] intSet = {"0","1","2","3","4","5","6","7","8","9"};
        string [] operatorSet = {"-","+","*"};
        int operand1 = Random.Range(0,9);
        int operand2 = Random.Range(0,9);
        string operator_ = operatorSet[Random.Range(0,2)];
        int answer  = 0;

        switch(operator_){
            case "-":
                answer = operand1-operand2;
                break;
            case "+":
                answer = operand1+operand2;
                break;
            case "*":
                answer = operand1*operand2;
                break;
            default: 
                Debug.Log("Something went really wrong! ");
                break;
        }
        equationQuestion = intSet[operand1] +" "+ operator_ +" "+ 
                           intSet[operand2] + " = " + "?";
        this.curAnswer= answer.ToString();
        questionBoard.GetComponent<TextMeshProUGUI>().text = equationQuestion;

        GameObject newIns = Instantiate(ratTemplate, 
        new Vector3(this.headPos.position.x+Random.Range(4,-4),
         this.headPos.position.y+Random.Range(4,-4), 0), Quaternion.identity);

        newIns.name=this.curAnswer;
        newIns.GetComponent<Follower>().setNumber(this.curAnswer);
        newIns.GetComponent<SpriteRenderer>().color=this.preColor[Random.Range(0,9)];
    }
}

