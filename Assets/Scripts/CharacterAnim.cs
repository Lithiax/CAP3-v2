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
        StartCoroutine(test());
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
            charAnim.SetTrigger("madTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            charAnim.SetTrigger("embarassedTrigger");
        }
    }

    IEnumerator test()
    {
        charAnim.SetTrigger("madTrigger");
        yield return new WaitForSeconds(5f);
        charAnim.SetTrigger("embarassedTrigger");
    }
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
    }
}
