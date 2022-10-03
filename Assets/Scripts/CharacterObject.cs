using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : Character
{
    public Animator charAnim;
    public Live2D.Cubism.Framework.Expression.CubismExpressionController expressionController;

    private void Awake()
    {
        charAnim = GetComponent<Animator>();
        expressionController = GetComponent<Live2D.Cubism.Framework.Expression.CubismExpressionController>();
        //StartCoroutine(test());
    }
}
