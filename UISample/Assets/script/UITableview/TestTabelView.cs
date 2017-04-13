using UnityEngine;
using System.Collections;

public class TestTabelView : MonoBehaviour {

	void Start () {
        TableView tb = TableView.Create(gameObject, "Item");

        Hashtable data = new Hashtable();
        data.Add(1, "111");
        data.Add(2, "222");
        data.Add(3, "333");
        data.Add(4, "444");
        data.Add(5, "555");
        data.Add(6, "666");
        data.Add(7, "666");
        data.Add(8, "666");

        tb.UpdateItems(data);
	}
}
