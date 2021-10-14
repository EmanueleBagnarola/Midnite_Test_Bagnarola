using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout  
{
	[System.Serializable]
	public struct RowData
	{
		public string[] Row;
	}

	public RowData[] Rows = new RowData[4]; 

	public string[] GetRowsData(int index)
    {
		return Rows[index].Row;
    }
}
