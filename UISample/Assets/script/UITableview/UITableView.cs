using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//列表排列方向
public enum TableDirection{
	Horizontal,		//水平
	Vertical		//直
}

public class UITableView : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
	TableCellWithShowedDelegate m_pTableCellWithShowedDelegate = null;//设置委托
	float m_fCellHight;			//行的高度
	int m_iCellTotalNum;		//行的总数量
	int m_iCellPortNum;			//可视行的数量
	Vector2 m_pTableViewSize;	//列表的宽高
	Vector2 m_pContentViewSize;	//列表内容的宽高
	GameObject m_pScrollView;	//列表Scroll控件
	RectTransform m_pContentView;	//列表内容
	List<UITableViewCell> m_pCellViewList = null;//可视的行存储队列

	//初始化数据
	void init(Vector2 rectPos,Vector2 rectSize,Transform parent,TableDirection dir){
		m_pCellViewList = new List<UITableViewCell> ();

		GameObject _scrollViewPerfab = Resources.Load ("prefab/test/ScrollView") as GameObject;
		m_pScrollView = GameObject.Instantiate (_scrollViewPerfab) as GameObject;
		m_pScrollView.transform.SetParent (parent);
		m_pScrollView.transform.localScale = _scrollViewPerfab.transform.localScale;
		m_pScrollView.transform.localPosition = rectPos;
		m_pScrollView.GetComponent<RectTransform> ().sizeDelta = rectSize;//new Vector2 (540.0f, 640.0f);

		m_pScrollView.GetComponent<ScrollRect>().onValueChanged.AddListener (onValueChanged);
		m_pContentView = m_pScrollView.GetComponent<ScrollRect> ().content;


		m_pTableViewSize = rectSize;

	}

	void onValueChanged(Vector2 value){

		float _showStartY = m_pContentView.GetComponent<RectTransform> ().localPosition.y;
		float _showEndY = _showStartY + m_pTableViewSize.y;
		if (_showStartY < 0 || _showEndY > m_pContentViewSize.y) {
			return;
		}
		int _listCount = m_pCellViewList.Count;
		//先搜索列表上面有没有可显示的行
		bool _bIsHaveCell = false;
		for(int i = 0; i < _listCount; i++){
			UITableViewCell _cellItem = m_pCellViewList [i];
			float _cellUpPosY = System.Math.Abs(_cellItem.transform.localPosition.y) - m_fCellHight * 0.5f;
			if(_cellUpPosY <= _showStartY ){
				_bIsHaveCell = true;
				break;
			}
		}
		if (_bIsHaveCell == false) {
			//计算出要显示的是哪一行，更新行
			float _fTopIndex = (_showStartY/m_fCellHight)-(int)(_showStartY/m_fCellHight);
			int _iTopIndex = (int)(_showStartY / m_fCellHight);
			int _showTopIndex = _fTopIndex == 0.0f ? _iTopIndex : _iTopIndex + 1;
			updateCellAtIndex (_showTopIndex-1);
		} else {
			//搜索列表下部分有没有可显示的行
			bool _bBottomHaveCell = false;
			for(int i = 0; i < _listCount; i ++){
				UITableViewCell _cellItem = m_pCellViewList[i];
				float _cellDownPosY = System.Math.Abs (_cellItem.transform.localPosition.y) + m_fCellHight * 0.5f;
				if (_cellDownPosY >= _showEndY) {
					_bBottomHaveCell = true;
					break;
				}
			}

			if (_bBottomHaveCell == false) {
				//计算出要显示的是哪一行，更新行
				float _fBottomIndex = (_showEndY/m_fCellHight)-(int)(_showEndY/m_fCellHight);
				int _iBottomIndex = (int)(_showEndY / m_fCellHight);
				int _showBottomIndex = _fBottomIndex == 0.0f ? _iBottomIndex : _iBottomIndex + 1;
				updateCellAtIndex (_showBottomIndex-1);
			}
		}
	}

	//按索引获取更新行
	void updateCellAtIndex(int idx){
		bool _isHaveDeq = isHaveDequeueCell ();
		if (_isHaveDeq == true) {
			UITableViewCell _cell = tableDequeueCellIndex (idx);
			if (m_pTableCellWithShowedDelegate != null) {
				m_pTableCellWithShowedDelegate(_cell, idx, true);
			}
		}else{
			UITableViewCell _cell = createTableCell (idx);
			m_pCellViewList.Add (_cell);
			if (m_pTableCellWithShowedDelegate != null) {
				m_pTableCellWithShowedDelegate(_cell, idx, true);
			}

		}
	}

	void createCellItem(UITableViewCell tabelCell,int index, bool isNew){
		GameObject _cell = null;
		if (isNew == true) {
			GameObject _cellPerfab = Resources.Load ("ListItemCell") as GameObject;
			_cell = GameObject.Instantiate (_cellPerfab) as GameObject;
			tabelCell.addViewCell (_cell);
		} else {
			_cell = tabelCell.getViewCell ();
		}


	}

	UITableViewCell createTableCell(int index){
		UITableViewCell _tableCell = UITableViewCell.createTableCell ();
		_tableCell.transform.SetParent (m_pContentView.transform);
		_tableCell.transform.localScale = m_pContentView.transform.localScale;
		_tableCell.transform.localRotation = m_pContentView.transform.localRotation;

		_tableCell.transform.localPosition = new Vector2 (_tableCell.transform.localPosition.x,positionWithIndex(index));

		return _tableCell;
	}

	float positionWithIndex(int index){
		float _cellPosY = -(m_fCellHight * 0.5f + m_fCellHight * index);
		Debug.Log ("positionWithIndex:index="+index+" _cellPosY="+_cellPosY);

		return _cellPosY;

	}
	//检测当前列表的滚动的位置中 列表上是否有已生成但在隐藏位置的行
	bool isHaveDequeueCell(){
		bool _bIsHaveDeqCell = false;
		float _showStartY = m_pContentView.GetComponent<RectTransform> ().localPosition.y;
		float _showEndY = _showStartY + m_pTableViewSize.y;
		int _listCount = m_pCellViewList.Count;
		for(int i = 0; i < _listCount; i++){
			UITableViewCell _cellItem = m_pCellViewList [i];
			float _cellDownPosY = System.Math.Abs (_cellItem.transform.localPosition.y) + m_fCellHight * 0.5f;
			if (_cellDownPosY > _showEndY) {
				_bIsHaveDeqCell = true;
				break;
			}
		}
		return _bIsHaveDeqCell;
	}

	//根据要显示的行索引，找出一个隐藏的行 并重新设置位置为要显示的行的位置并返回
	UITableViewCell tableDequeueCellIndex(int idx){
		UITableViewCell _pTableViewCell = null;
		float _showStartY = m_pContentView.GetComponent<RectTransform> ().localPosition.y;
		float _showEndY = _showStartY + m_pTableViewSize.y;
		int _listCount = m_pCellViewList.Count;

		//检测当前列表的滚动的位置中 列表上面是否有已生成但在隐藏位置的行
		bool _bTopIsHaveDeqCell = false;
		for(int i = 0; i < _listCount; i++){
			UITableViewCell _cellItem = m_pCellViewList [i];
			float _cellDownPosY = System.Math.Abs (_cellItem.transform.localPosition.y) - m_fCellHight * 0.5f;
			if (_cellDownPosY < _showEndY-m_fCellHight) {
				_bTopIsHaveDeqCell = true;
				break;
			}
		}
		int _iDequeueCellIndex = 0;
		if (_bTopIsHaveDeqCell == true) {
			//找出列表上面位置 处于隐藏位置的最顶的一行
			for(int index = 1; index < _listCount; index++){
				UITableViewCell _pHideCell = m_pCellViewList [_iDequeueCellIndex];
				UITableViewCell _pTempCell = m_pCellViewList [index];
				if (System.Math.Abs(_pTempCell.transform.localPosition.y) < System.Math.Abs(_pHideCell.transform.localPosition.y)){
					_iDequeueCellIndex = index;
				}
			}
		} else {
			for (int i = 1; i < _listCount; i++) {
				UITableViewCell _pHideCell = m_pCellViewList [_iDequeueCellIndex];
				UITableViewCell _pTempCell = m_pCellViewList [i];
				if (System.Math.Abs (_pTempCell.transform.localPosition.y) < System.Math.Abs (_pHideCell.transform.localPosition.y)) {
					_iDequeueCellIndex = i;
				}
			}
		}
		float _cellPosY = -(m_fCellHight * 0.5f + m_fCellHight * idx);
		m_pCellViewList[_iDequeueCellIndex].transform.localPosition = new Vector2 (m_pCellViewList[idx].transform.localPosition.x,_cellPosY);
		_pTableViewCell = m_pCellViewList [_iDequeueCellIndex];
		return _pTableViewCell;
	}

	//更新加载列表
	public void reloadData(){
		updateData ();
		float _showStartY = m_pContentView.GetComponent<RectTransform> ().localPosition.y;
		int _iFirstIndex = (int)(_showStartY / m_fCellHight);
		int _showFirstIndex = _iFirstIndex == 0.0f ? _iFirstIndex : _iFirstIndex + 1;
		for (int i = _showFirstIndex; i < _showFirstIndex+m_iCellPortNum; i++) {
			updateCellAtIndex (i);
		}
	}

	void updateData(){
		//计算复用的cell的数量
		float m_iPortNum = (m_pTableViewSize.y/m_fCellHight)-(int)(m_pTableViewSize.y/m_fCellHight);//取余
		int m_iPortNum1 = (int)(m_pTableViewSize.y/m_fCellHight);
		m_iCellPortNum = m_iPortNum == 0.0f ? m_iPortNum1 : m_iPortNum1+1;
		if (m_iCellPortNum > m_iCellTotalNum) {
			m_iCellPortNum = m_iCellTotalNum;
		}
		m_pContentViewSize.x = m_pTableViewSize.x;
		m_pContentViewSize.y = m_iCellTotalNum * m_fCellHight;
		m_pContentView.GetComponent<RectTransform> ().sizeDelta = m_pContentViewSize;
	}

	public void OnDrag (PointerEventData eventData){
//		Debug.Log ("开始拖动pressPosition="+eventData.pressPosition+"position="+eventData.position);

	}

	public void OnBeginDrag (PointerEventData eventData){
//		Debug.Log ("开始拖动pressPosition="+eventData.pressPosition+"position="+eventData.position);
	}

	public void OnEndDrag (PointerEventData eventData){
//		Debug.Log ("结束拖动pressPosition="+eventData.pressPosition+"position="+eventData.position);
	}
	//获取和设置行数量
	public void setCellTotalNum(int num){
		m_iCellTotalNum = num;
	}
	public float getCellTotalNum(){
		return m_iCellTotalNum;
	}
	//获取和设置行的高度
	public void setCellHight(float h){
		m_fCellHight = h;

	}
	public float getCellHight(){
		return m_fCellHight;
	}

	public void setTableCellWithShowedDelegate(TableCellWithShowedDelegate del){
		m_pTableCellWithShowedDelegate = del;
	}

	public TableCellWithShowedDelegate getTableCellWithShowedDelegate(){
		return m_pTableCellWithShowedDelegate;
	}
	// Use this for initialization
	void Start ()
	{
	
	}


	//公共方法
	public static UITableView createTable(Vector2 rectPos,Vector2 rectSize,
		Transform parent,TableDirection dir = TableDirection.Vertical){
		GameObject _object = new GameObject ();
		UITableView _table = null;
		if (_object != null) {
//			_object.name = "UITableView";
			_table = _object.AddComponent (typeof(UITableView))  as UITableView;
			_table.init (rectPos, rectSize, parent, dir);
		} else {
			Debug.Log ("出错了");
		}

		return _table;
	}
	public delegate void TableCellWithShowedDelegate (UITableViewCell cell,int index, bool isNew);
}

	
