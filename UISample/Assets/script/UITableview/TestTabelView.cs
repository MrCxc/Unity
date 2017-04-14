using UnityEngine;
using System.Collections;

public class TestTabelView : MonoBehaviour {

	void Start () {
        TableView tb = TableView.Create(gameObject, "Item");
        
        Hashtable data = new Hashtable();
        data.Add(0, "111");
        data.Add(1, "222");
        data.Add(2, "333");
        data.Add(3, "444");
        data.Add(4, "555");
        data.Add(5, "666");
        data.Add(6, "777");
        data.Add(7, "888");

        tb.UpdateItems(data);
	}
}
