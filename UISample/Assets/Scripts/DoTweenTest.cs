using UnityEngine;
using System.Collections;
using DG.Tweening; 

public class DoTweenTest : MonoBehaviour {
    public Vector3 vec3 = Vector3.zero;
    public float fValue = 0f;
    public int iValue = 0;

    public RectTransform rectTrans;

    GameObject obj;
    bool bClick;

    public GameObject cubeObj;

	void Start () {
        // Lambda 

        /*
        DOTween.To(()=> vec3, x=> vec3 = x, new Vector3(10f, 10f, 10f), 3f);
        DOTween.To(()=> fValue, x=> fValue = x, 10f, 3f);
        DOTween.To(()=> iValue, x=> iValue = x, 10, 3f);
        */

        obj = gameObject;
        bClick = false;

        /*
        Tween twn = rectTrans.DOLocalMove(new Vector3(1000, 1000, 0), 0.3f);
        twn.SetAutoKill(false);
        twn.Pause();
        */

        //cubeObj.transform.DOLocalMoveX(300f, 3f).From(); // 绝对位置
        //cubeObj.transform.DOLocalMoveX(300f, 3f).From(true);// 相对位置

        cubeObj.transform.DOBlendableLocalMoveBy(new Vector3(300f, 300f, 0f), 3f);
	}
	
	void Update () {
	
	}

    public void OnClick()
    {
        if (!bClick)
        {
            //obj.transform.DOLocalMove(new Vector3(400, 400, 400), 2f);
           // obj.transform.DOMove(new Vector3(400, 400, 400), 2f);

            // 与rectTrans.DOPlayBackwards() 成对出现
           // rectTrans.DOPlayForward();
        }
        else
        {
            //obj.transform.DOLocalMove(new Vector3(0, 0, 0), 2f);
            //obj.transform.DOMove(new Vector3(0, 0, 0), 2f);

            //rectTrans.DOPlayBackwards();
        }

        bClick = !bClick;
    }
}
