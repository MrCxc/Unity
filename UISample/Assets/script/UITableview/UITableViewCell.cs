using UnityEngine;
using System.Collections;

public class UITableViewCell : MonoBehaviour
{
	GameObject m_pViewCell;


	//获取cell内容视图
	public GameObject getViewCell () {
		return m_pViewCell;
	}

	//添加cell内容视图
	public void addViewCell (GameObject _viewCell) {
		_viewCell.transform.SetParent (this.transform);
		_viewCell.transform.localScale = this.transform.localScale;
		_viewCell.transform.localRotation = this.transform.localRotation;
		_viewCell.transform.localPosition = Vector2.zero;
		m_pViewCell = _viewCell;
	}

	//公共方法
	public static UITableViewCell createTableCell(){
		GameObject _object = new GameObject ();
		UITableViewCell _table = null;
		if (_object != null) {
			_table = _object.AddComponent (typeof(UITableViewCell))  as UITableViewCell;
		} else {
			Debug.Log ("出错了");
		}
		return _table;
	}
		
}

