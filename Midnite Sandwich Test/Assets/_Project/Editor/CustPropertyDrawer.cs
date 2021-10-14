using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
		EditorGUI.PrefixLabel(position,label);
		Rect newposition = position;
		newposition.y += 18f;
		SerializedProperty data = property.FindPropertyRelative("Rows");
		//data.rows[0][]
		for(int j=0;j<4;j++){
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("Row");
			newposition.height = 18f;
			if(row.arraySize != 4)
				row.arraySize = 4;
			newposition.width = position.width/20;
			for(int i=0;i<4;i++){
				EditorGUI.PropertyField(newposition,row.GetArrayElementAtIndex(i),GUIContent.none);
				newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y += 18f;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
		return 18f * 8;
	}
}