using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TableView : MonoBehaviour
{
    GameObject m_pParentObj;
    GameObject m_pContent;
    GameObject m_pCellItem;
    ScrollRect m_pScrolRect;

    List<GameObject> m_pCellViewList = new List<GameObject>(6);
    Hashtable m_pData = new Hashtable();

    float m_spacing;
    float m_contentStartPos;
    float m_contentEndPos;

    void Init(GameObject parentObj, string cellItemName, float spacing)
    {
        m_pParentObj = parentObj;
        m_spacing = spacing;

        m_pContent = CreateContent();
        if (null == m_pContent)
        {
            return;
        }

        m_pCellItem = LoadCellItem(cellItemName);
        if (null == m_pCellItem)
        {
            return;
        }

        m_pCellViewList.Add(m_pCellItem);

        m_pScrolRect = CreateScrollView();
        if (null == m_pScrolRect)
        {
            return;
        }

        m_pParentObj.AddComponent<Mask>();
    }

    GameObject CreateContent()
    {
        GameObject ret = new GameObject();
        if (null == ret)
        {
            Debug.LogError("Create GameObject failed");
            return null;
        }

        ret.name = "Content";
        ret.transform.SetParent(m_pParentObj.transform);
        ret.AddComponent<RectTransform>();

        return ret;
    }

    GameObject LoadCellItem(string name)
    {
        Object perfab = Resources.Load("prefab/" + name);
        if (null == perfab)
        {
            Debug.LogError("Load perfab resources that name is " + name + " fail");
            return null;
        }

        GameObject ret = GameObject.Instantiate(perfab, m_pContent.transform) as GameObject;

        RectTransform cellRectTransform = ret.GetComponent<RectTransform>();
        cellRectTransform.anchorMax = new Vector2(0.5f, 1);
        cellRectTransform.anchorMin = new Vector2(0.5f, 1);

        return ret;
    }


    ScrollRect CreateScrollView()
    {
        ScrollRect ret = m_pParentObj.AddComponent<ScrollRect>();
        ret.viewport = m_pParentObj.transform as RectTransform;
        ret.content = m_pContent.transform as RectTransform;

        // Default vertical sliding
        ret.horizontal = false;
        ret.vertical = true;

        ret.movementType = ScrollRect.MovementType.Elastic;

        // Callback executed when the scroll position of the slider is changed
        ret.onValueChanged.AddListener(OnValueChanged);

        return ret;
    }

    void OnValueChanged(Vector2 value)
    {
        float curContentY = m_pContent.GetComponent<RectTransform>().localPosition.y;
        if (curContentY < m_contentEndPos)
        {
            return;
        }

        Debug.Log(curContentY);
    }

    void CreateCellItems(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject cell = null;
            if (i < m_pCellViewList.Count)
            {
                cell = m_pCellViewList[i];
            }
            else
            {
                cell = GameObject.Instantiate(m_pCellItem, m_pContent.transform) as GameObject;
                m_pCellViewList.Add(cell);
            }

            float y = -1 * ( i * m_spacing + (0.5f + i) * cell.GetComponent<RectTransform>().sizeDelta.y);
            cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
            cell.SetActive(true);
        }
    }

    void SetDataToCellItems()
    {
        int i = 0;
        IDictionaryEnumerator enumerator = m_pData.GetEnumerator();

        while (enumerator.MoveNext() && i < m_pCellViewList.Count)
        {
            GameObject cell = m_pCellViewList[i];
            GameObject text = cell.transform.Find("Button/Text").gameObject;
            text.GetComponent<Text>().text = enumerator.Value as string;
            
            i++;
        }
    }

    public void UpdateItems(Hashtable data)
    {
        if (0 == data.Count)
        {
            // cleanup
            return;
        }

        m_pData = data;

        Vector2 viewportSize = m_pScrolRect.viewport.sizeDelta;
        Vector2 cellSize = m_pCellItem.GetComponent<RectTransform>().sizeDelta;

        int pageViewAmount = Mathf.CeilToInt(viewportSize.y / cellSize.y);
        int cellListAmount = (pageViewAmount >= data.Count) ? data.Count : (pageViewAmount + 1);

        //resize m_pContent
        float height = cellListAmount * (cellSize.y + m_spacing) - m_spacing;
        height = (height < viewportSize.y) ? viewportSize.y : height;

        RectTransform contentRect = m_pContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(viewportSize.x, height);

        // if anchoredPosition.y is greater than value, 
        // the value of anchoredPosition.y is set to value.
        float value = (height - viewportSize.y) / 2;
        m_contentStartPos = -1 * value;
        m_contentEndPos = value;

        //  init position
        contentRect.anchoredPosition = new Vector2(0, m_contentStartPos);

        CreateCellItems(cellListAmount);
        SetDataToCellItems();
    }

    // spacing: The spacing to use between cellItem elements, the default value is 2
    public static TableView Create(GameObject parentObj, string cellItemName, float spacing = 2.0f)
    {
        TableView tbView = null;

        if (null != parentObj && "" != cellItemName)
        {
            tbView = parentObj.AddComponent(typeof(TableView)) as TableView;
            tbView.Init(parentObj, cellItemName, spacing);
        }
        else
        {
            Debug.LogError("Failed to create GameObject");
        }

        return tbView;
    }
}
