using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Number : Tile, IPointerDownHandler
{
    public int GetNumberID
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
    [SerializeField]
    private int _numberID = 0;

    private int _startingNumberID = 0;

    public override void Start()
    {
        base.Start();

        _startingNumberID = _numberID;
        SetNumberID(_numberID);
    }

    public override void ResetToStartingPosition()
    {
        base.ResetToStartingPosition();
        SetNumberID(_startingNumberID);
    }

    public void SetNumberID(int numberID)
    {
        _numberID = numberID;
        _numberText.text = numberID.ToString();
        _meshRenderer.material = Resources.Load<Material>("MaterialResources/" + GetNumberID + "_M");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Touch Number: " + GetNumberID);
        EventsHandler.Instance.OnTileTouch?.Invoke(this);
    }
}
