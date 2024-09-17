using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
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

    private string equationQuestion = "";
    public string curAnswer = "";

    public GameObject questionBoard = null;

    public GameObject ratTemplate = null;

    public Transform headPos = null;

    void onLoad(){
    }

    void Start()
    {
        GameObject[] Rats = GameObject.FindGameObjectsWithTag("Rat");
        for (int i = 0; i < Rats.Length; i++)
        {
            Rats[i % preColor.Length].GetComponent<SpriteRenderer>().color = preColor[i % preColor.Length];
        }

        // generateQuestion();
    }

    public bool tryToAnswer(string ans)
    {   
        return this.curAnswer==ans;
    }

    public void generateQuestion()
    {
        string[] intSet = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        string[] operatorSet = { "-", "+", "*" };
        int operand1 = Random.Range(0, 9);
        int operand2 = Random.Range(0, 9);
        string operator_ = operatorSet[Random.Range(0, 2)];
        int answer = 0;

        switch (operator_)
        {
            case "-":
                answer = operand1 - operand2;
                break;
            case "+":
                answer = operand1 + operand2;
                break;
            case "*":
                answer = operand1 * operand2;
                break;
            default:
                Debug.Log("Something went really wrong! ");
                break;
        }
        equationQuestion = intSet[operand1] + " " + operator_ + " " +
                           intSet[operand2] + " = " + "?";
        this.curAnswer = answer.ToString();
        questionBoard.GetComponent<TextMeshProUGUI>().text = equationQuestion;

        GameObject newIns = Instantiate(ratTemplate,
        new Vector3(this.headPos.position.x + Random.Range(4, -4),
         this.headPos.position.y + Random.Range(4, -4), 0), Quaternion.identity);

        newIns.name = this.curAnswer;
        newIns.GetComponent<Follower>().setNumber(this.curAnswer);
        newIns.GetComponent<SpriteRenderer>().color = this.preColor[Random.Range(0, 9)];
    }

    public GameObject getNewRat(string str){
        GameObject rat = Instantiate(ratTemplate, new Vector3(), Quaternion.identity);
        rat.GetComponent<SpriteRenderer>().color = this.preColor[Random.Range(0, 9)];
        rat.name = str;
        return rat;
    }

    public void spawnDummyRat(Vector3 pos, string dummyAns)
    {
        GameObject dummy = Instantiate(ratTemplate, pos, Quaternion.identity);

        dummy.name = dummyAns;
        dummy.GetComponent<SpriteRenderer>().color = this.preColor[Random.Range(0, 9)];
        StartCoroutine(this.DestroyRat(dummy));
    }

    public void setQuestion(string question, string answer, Vector3 pos)
    {
        this.equationQuestion = question;
        this.curAnswer = answer;

        questionBoard.GetComponent<TextMeshProUGUI>().text = question;
        questionBoard.GetComponent<TextMeshProUGUI>().color = Color.white;

         questionBoard.GetComponent<TextMeshProUGUI>().enableVertexGradient = false;
        
        questionBoard.GetComponent<TextMeshProUGUI>().fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, Color.clear); 
        questionBoard.GetComponent<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.0f);

        GameObject newIns = Instantiate(ratTemplate, pos, Quaternion.identity);



        // new Vector3(this.headPos.position.x+Random.Range(4,-4),this.headPos.position.y+Random.Range(4,-4), 0)

        newIns.name = answer;
        newIns.GetComponent<Follower>().setNumber(answer);
        newIns.GetComponent<SpriteRenderer>().color = this.preColor[Random.Range(0, 9)];
        StartCoroutine(this.DestroyRat(newIns));
    }
    public IEnumerator DestroyRat(GameObject obj)
    {
        yield return new WaitForSeconds(2);
        Tween scaleTween = null;
        if (obj)
        {
            scaleTween = obj.transform.DOScale(0f, 3f);
        }
        yield return new WaitForSeconds(3);
        if (obj && obj.CompareTag("Rat"))
        {
            if (scaleTween != null)
            {
                scaleTween.Kill();
            }
            Destroy(obj);
        }
        else
        {
            if (scaleTween != null)
            {
                scaleTween.Kill();
            }
            if (obj)
            {
                    Destroy(obj);
                // obj.transform.DOScale(10f, 0.1f).OnComplete(()=>{
                // });
            }
        }
    }
   public void setRed()
    {
        TextMeshProUGUI textMeshPro = questionBoard.GetComponent<TextMeshProUGUI>();
        
        if (textMeshPro != null)
        {
            textMeshPro.color = Color.red;
            
            textMeshPro.enableVertexGradient = true;
            textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, Color.red);
            textMeshPro.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 100f); 
            
        }
    }

    public void setGreen(){
        TextMeshProUGUI textMeshPro = questionBoard.GetComponent<TextMeshProUGUI>();
        
        if (textMeshPro != null)
        {   
            textMeshPro.text = textMeshPro.text.Substring(0, textMeshPro.text.Length - 1);
            textMeshPro.text += this.curAnswer;

          

            textMeshPro.color = Color.green;
            
            textMeshPro.enableVertexGradient = true;
            textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, Color.red);
            textMeshPro.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.5f); 
        }
    }

    public static int Floor(double value)
    {
        int truncated = (int)value; 
        return (value < truncated) ? truncated - 1 : truncated;
    }

}

