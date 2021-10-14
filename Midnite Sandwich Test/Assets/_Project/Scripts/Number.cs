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

    public bool IsMoved
    {
        get
        {
            return _isMoved;
        }
    }

    [SerializeField]
    private TextMeshPro _numberText = null;
    [SerializeField]
    private MeshRenderer _meshRenderer = null;
    [SerializeField]
    private int _numberID = 0;

    private int _startingNumberID = 0;

    private bool _isMoved = false;

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
        _isMoved = false;
    }

    public void MoveNumber(Vector3 destination)
    {
        _isMoved = true;
        //HideObject();
        _numberText.text = string.Empty;
        iTween.MoveTo(gameObject, destination - new Vector3(0, 0.01f, 0), 0.2f);
        Invoke(nameof(HideObject), 0.25f);
    }

    public void SetNumberID(int numberID)
    {
        _numberID = numberID;
        _numberText.text = numberID.ToString();
        _meshRenderer.material = Resources.Load<Material>("MaterialResources/" + GetNumberID + "_M");
    }

    private void HideObject()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Touch Number: " + GetNumberID);
        EventsHandler.Instance.OnTileTouch?.Invoke(this);
    }
}
