using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Number : Tile, IPointerDownHandler
{
    public string GetNumberID
    {
        get
        {
            return _numberID;
        }
    }

    [SerializeField]
    private TextMeshPro _numberText = null;
    [SerializeField]
    private MeshRenderer _meshRenderer = null;

    private string _numberID = string.Empty;

    private void Start()
    {
        _numberText.text = _numberID;

        _meshRenderer.material = Resources.Load<Material>("MaterialResources/" + GetNumberID + "_M");
    }

    public void SetNumberID(string numberID)
    {
        _numberID = numberID;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Touch Number: " + GetNumberID);
        EventsHandler.Instance.OnTileTouch?.Invoke(this);
    }
}
