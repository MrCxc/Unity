using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TableView : MonoBehaviour
{
    GameObject m_parentObj;
    GameObject m_contentObj;
    GameObject m_cell;
    ScrollRect m_pScrolRect;

    Dictionary<int, GameObject> m_displayingCells;
    List<GameObject> m_reusableCells;
    Hashtable m_data = new Hashtable();

    SlipDirection m_curDirection;

    float m_spacing;
    float m_viewportSize;
    float m_cellSize;
    float m_contentSize;
    float m_totalSlipDistance;

    int m_cellAmount;

    public enum SlipDirection
    {
        Horizontal,
        Vertical
    }

    void Init(GameObject parentObj, string cellItemName, float spacing, SlipDirection slipDir)
    {
        m_parentObj = parentObj;
        m_spacing = spacing;
        m_curDirection = slipDir;

        m_displayingCells = new Dictionary<int, GameObject>();
        m_reusableCells = new List<GameObject>();

        m_contentObj = CreateContent();

        m_cell = LoadCellItem(cellItemName);
        m_reusableCells.Add(m_cell);

        m_pScrolRect = CreateScrollView();

        m_parentObj.AddComponent<Mask>();

        if (slipDir == SlipDirection.Horizontal)
        {
            m_viewportSize = m_pScrolRect.viewport.GetComponent<RectTransform>().sizeDelta.x;
            m_cellSize = m_cell.GetComponent<RectTransform>().sizeDelta.x;
        }
        else
        {
            m_viewportSize = m_pScrolRect.viewport.GetComponent<RectTransform>().sizeDelta.y;
            m_cellSize = m_cell.GetComponent<RectTransform>().sizeDelta.y;
        }
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
        ret.transform.SetParent(m_parentObj.transform);
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

        GameObject ret = GameObject.Instantiate(perfab, m_contentObj.transform) as GameObject;

        RectTransform cellRectTrans = ret.GetComponent<RectTransform>();
        cellRectTrans.anchorMax = new Vector2(0.5f, 1);
        cellRectTrans.anchorMin = new Vector2(0.5f, 1);

        return ret;
    }


    ScrollRect CreateScrollView()
    {
        ScrollRect ret = m_parentObj.AddComponent<ScrollRect>();
        ret.viewport = m_parentObj.transform as RectTransform;
        ret.content = m_contentObj.transform as RectTransform;

        // Default vertical sliding
        ret.horizontal = false;
        ret.vertical = true;

        ret.movementType = ScrollRect.MovementType.Elastic;

        // Callback executed when the scroll position of the slider is changed
        ret.onValueChanged.AddListener(OnValueChanged);

        return ret;
    }

    void InitContent()
    {
        int pageAmount = Mathf.CeilToInt(m_viewportSize / m_cellSize);
        m_cellAmount = (pageAmount >= m_data.Count) ? m_data.Count : (pageAmount + 1);

        m_contentSize = m_cellAmount * (m_cellSize + m_spacing);
        m_contentSize = (m_contentSize < m_viewportSize) ? m_viewportSize : m_contentSize;

        m_totalSlipDistance = m_contentSize - m_viewportSize;

        Vector2 sizeDeltaViewport = m_pScrolRect.viewport.GetComponent<RectTransform>().sizeDelta;
        RectTransform contentRectTrans = m_contentObj.GetComponent<RectTransform>();
       
        if (m_curDirection == SlipDirection.Vertical)
        {
            contentRectTrans.sizeDelta = new Vector2(sizeDeltaViewport.x, m_contentSize);
            contentRectTrans.anchorMin = new Vector2(0.5f, 1f);
            contentRectTrans.anchorMax = new Vector2(0.5f, 1f);
            contentRectTrans.pivot = new Vector2(0.5f, 1f); 
        }
        else
        {
            contentRectTrans.sizeDelta = new Vector2(m_contentSize, sizeDeltaViewport.y);
            contentRectTrans.anchorMin = new Vector2(0f, 0.5f);
            contentRectTrans.anchorMax = new Vector2(0f, 0.5f);
            contentRectTrans.pivot = new Vector2(0f, 0.5f);
        }

        contentRectTrans.anchoredPosition = Vector2.zero;

        for (int i = 0; i < m_cellAmount; i++)
        {
            OnCellAtIndex(i);
        }
    }

    void OnCellAtIndex(int index)
    {
        GameObject cell = null;
        if (m_reusableCells.Count > 0)
        {
            cell = m_reusableCells[0];
            m_reusableCells.RemoveAt(0);
        }
        else
        {
            cell = GameObject.Instantiate(m_cell, m_contentObj.transform) as GameObject;
        }

        cell.transform.localScale = Vector2.one;

        RectTransform cellRectTrans = cell.GetComponent<RectTransform>();
        if (m_curDirection == SlipDirection.Vertical)
        {
            cellRectTrans.anchorMin = new Vector2(0.5f, 1f);
            cellRectTrans.anchorMax = new Vector2(0.5f, 1f);
            cellRectTrans.pivot = new Vector2(0.5f, 1f);

            float y = (1 + index) * m_spacing + index * m_cellSize;
            y = -1 * y;
            cellRectTrans.anchoredPosition = new Vector2(0, y);
        }
        else
        {
            cellRectTrans.anchorMin = new Vector2(0f, 0.5f);
            cellRectTrans.anchorMax = new Vector2(0f, 0.5f);
            cellRectTrans.pivot = new Vector2(0f, 0.5f);

            float x = (1 + index) * m_spacing + index * m_cellSize;
            cellRectTrans.anchoredPosition = new Vector2(x, 0);
        }

        cell.transform.Find("Button/Text").GetComponent<Text>().text = (index + 1) + ": " + m_data[index];

        cell.SetActive(true);
        cell.transform.SetAsLastSibling();

        m_displayingCells.Add(index, cell);
    }

    // the values of offset.x and offset.y are between -1 and 1,
    // representing the percentage of the width of the horizontal slip out of the visible area 
    // and the height of the visible area.
    void OnValueChanged(Vector2 offset)
    {
        if (m_viewportSize >= m_contentSize)
        {
            return;
        }

        float offsetX = m_totalSlipDistance * offset.x;
        float offsetY = m_totalSlipDistance * (1 - offset.y);

        // start and end position of the visible region
        float startPos = 0;
        float endPos = 0;

        if (m_curDirection == SlipDirection.Vertical)
        {
            startPos = offsetY;
            endPos = offsetY + m_viewportSize;
        }
        else
        {
            startPos = offsetX;
            endPos = offsetX + m_viewportSize;
        }

        endPos = endPos > m_contentSize ? m_contentSize : endPos;

        int startIndex = Mathf.CeilToInt(startPos / (m_cellSize + m_spacing));
        startIndex = startIndex < 0 ? 0 : startIndex;

        int endIndex = Mathf.CeilToInt(endPos / (m_cellSize + m_spacing));
        endIndex = endIndex > (m_cellAmount - 1) ? (m_cellAmount - 1) : endIndex;

        List<int> delList = new List<int>();
        foreach (KeyValuePair<int, GameObject> pair in m_displayingCells)
        {
            if (pair.Key < startIndex || pair.Key > endIndex)
            {
                delList.Add(pair.Key);

                // Recovery beyond the visible range of cell
                m_reusableCells.Add(pair.Value); 
            }
        }

        foreach (int index in delList)
        {
            m_displayingCells.Remove(index);
        }

        for (int i = startIndex; i <= endIndex; i++)
        {
            if (m_displayingCells.ContainsKey(i))
            {
                continue;
            }

            OnCellAtIndex(i);
        }
    }

    public void UpdateItems(Hashtable data)
    {
        if (0 == data.Count)
        {
            // cleanup
            return;
        }

        m_data = data;

        InitContent();
    }

    public static TableView Create(GameObject parentObj, string cellItemName, 
        float spacing = 2.0f, SlipDirection slipDir = SlipDirection.Vertical)
    {
        TableView tbView = null;

        if (null != parentObj && "" != cellItemName)
        {
            tbView = parentObj.AddComponent(typeof(TableView)) as TableView;
            tbView.Init(parentObj, cellItemName, spacing, slipDir);
        }
        else
        {
            Debug.LogError("Failed to create GameObject");
        }

        return tbView;
    }
}
