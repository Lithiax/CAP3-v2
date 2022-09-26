using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnim : MonoBehaviour
{


    private Animator charAnim;
    private GameObject selfChar;
    private Live2D.Cubism.Framework.Expression.CubismExpressionController expressionController;

    private void Start()
    {
        charAnim = GetComponent<Animator>();
        expressionController = GetComponent<Live2D.Cubism.Framework.Expression.CubismExpressionController>();
        //StartCoroutine(test());
    }

    private void Update()
    {
        CharacterMotion();
        FacialExpressions();
    }

    void CharacterMotion()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("look around");
            charAnim.SetTrigger("lookAround");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("laughing");
            charAnim.SetTrigger("laughing");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("MIX");
            charAnim.SetTrigger("laughing");
            expressionController.CurrentExpressionIndex = 4;
        }
    }

    //IEnumerator test()
    //{
    //    charAnim.SetTrigger("madTrigger");
    //    yield return new WaitForSeconds(5f);
    //    charAnim.SetTrigger("embarassedTrigger");
    //}
    void FacialExpressions()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            expressionController.CurrentExpressionIndex = 0;

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            expressionController.CurrentExpressionIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            expressionController.CurrentExpressionIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            expressionController.CurrentExpressionIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            expressionController.CurrentExpressionIndex = 4;
        }
    }
}
