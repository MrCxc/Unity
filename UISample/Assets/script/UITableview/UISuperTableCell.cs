using UnityEngine;
using System.Collections;

public class UISuperTableCell : MonoBehaviour {

	//公共方法
	public static UISuperTableCell createCell(string perfabName){
		GameObject _cellPerfab = Resources.Load (perfabName) as GameObject;
		GameObject _object = GameObject.Instantiate (_cellPerfab) as GameObject;
		UISuperTableCell _cell = null;
		if (_object != null) {
			_cell = _object.GetComponent(typeof(UISuperTableCell)) as UISuperTableCell;
			_cell.cellGameObject = _object;
		} else {
			Debug.Log ("出错了");
		}
		return _cell;
	}

	public GameObject cellGameObject {
		get;
		set;
	}

	public int cellIndex {
		get;
		set;
	}
		
}
