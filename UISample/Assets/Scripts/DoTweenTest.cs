using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening; 

public class DoTweenTest : MonoBehaviour {
    /*
    public Vector3 vec3 = Vector3.zero;
    public float fValue = 0f;
    public int iValue = 0;

    public RectTransform rectTrans;

    GameObject obj;

    public GameObject cubeObj;
    */

    public Text m_text;
    public Camera m_camera;

    bool bClick;

	void Start () {
        // Lambda 

        /*
        DOTween.To(()=> vec3, x=> vec3 = x, new Vector3(10f, 10f, 10f), 3f);
        DOTween.To(()=> fValue, x=> fValue = x, 10f, 3f);
        DOTween.To(()=> iValue, x=> iValue = x, 10, 3f);
        */

        /*
        obj = gameObject;
        bClick = false;
        */

        /*
        Tween twn = rectTrans.DOLocalMove(new Vector3(1000, 1000, 0), 0.3f);
        twn.SetAutoKill(false);
        twn.Pause();
        */

        //cubeObj.transform.DOLocalMoveX(300f, 3f).From(); // 绝对位置
        //cubeObj.transform.DOLocalMoveX(300f, 3f).From(true);// 相对位置

        //cubeObj.transform.DOBlendableLocalMoveBy(new Vector3(300f, 300f, 0f), 3f);

        /*
        // 动画设置
        Tween twn = cubeObj.transform.DOLocalMoveX(-300f, 2f).From();
        twn.SetEase(Ease.OutBounce);
        twn.OnComplete(AnimationComplete);
        */

        // text 
        m_text.text = "";

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(m_text.DOText("The afternoon grows light because at last.", 2f));
        mySequence.Append(m_text.DOColor(Color.red, 2f));
        mySequence.Append(m_text.DOFade(100f, 0.3f));
        mySequence.Insert(4.0f, m_text.DOBlendableColor(new Color(1.0f, 1.0f, 1.0f), 3.0f));
        mySequence.Join(m_camera.DOShakePosition(1.0f, 10.0f));

        //StartCoroutine(SomeCoroutine());

        mySequence.Play();
	}
	
	void Update () {

	}

    void AnimationComplete()
    {
        Debug.Log("finished.");
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

        //m_camera.DOShakePosition(1.0f, 10.0f);
        //m_camera.DOShakeRotation(1.0f);

        /*
        m_camera.clearFlags = CameraClearFlags.SolidColor;
        m_camera.DOColor(new Color(1.0f, 0.0f, 0.0f), 2f);
        */
    }

    IEnumerator SomeCoroutine()
    {
        Debug.Log("-------------");
        Tween myTween = transform.DOMoveX(45, 1);
        yield return myTween.WaitForCompletion();
        // This log will happen after the tween has completed
        Debug.Log("Tween completed!");
    }
}
