using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SwipeDirection
{
    up,
    down,
    left,
    right
}

public class SwipeController : MonoBehaviour
{
    [SerializeField]
    private float _swipeThreshold = 20f;
    [SerializeField]
    private bool _detectSwipeOnlyAfterRelease = false;

    private Vector2 _fingerDown = Vector2.zero;
    private Vector2 _fingerUp = Vector2.zero;

    private Tile _lastTouchedTile = null;

    private bool _detectSwipe = true;

    private void Start()
    {
        EventsHandler.Instance.OnTileTouch?.AddListener((tile) =>
        {
            _lastTouchedTile = tile;
        });
    }

    private void Update()
    {
        CheckTouch();
    }

    private void CheckTouch()
    {
        if (Input.touchCount == 0)
            _detectSwipe = true;

        if (!_detectSwipe)
            return;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _fingerUp = touch.position;
                _fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!_detectSwipeOnlyAfterRelease)
                {
                    _fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                _fingerDown = touch.position;
                CheckSwipe();
            }
        }
    }

    private void CheckSwipe()
    {
        //Check if Vertical swipe
        if (VerticalMove() > _swipeThreshold && VerticalMove() > HorizontalMove())
        {
            if (_fingerDown.y - _fingerUp.y > 0)//Up swipe
            {
                OnSwipeUp();
            }
            else if (_fingerDown.y - _fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown();
            }
            _fingerUp = _fingerDown;
        }

        //Check if Horizontal swipe
        else if (HorizontalMove() > _swipeThreshold && HorizontalMove() > VerticalMove())
        {
            if (_fingerDown.x - _fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            }
            else if (_fingerDown.x - _fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
            }
            _fingerUp = _fingerDown;
        }
    }

    private float VerticalMove()
    {
        return Mathf.Abs(_fingerDown.y - _fingerUp.y);
    }

    private float HorizontalMove()
    {
        return Mathf.Abs(_fingerDown.x - _fingerUp.x);
    }

    private void OnSwipeUp()
    {
        //Debug.Log("Swipe UP");
        CommitSwipe(SwipeDirection.up);
    }

    private void OnSwipeDown()
    {
        //Debug.Log("Swipe Down");
        CommitSwipe(SwipeDirection.down);
    }

    private void OnSwipeLeft()
    {
        //Debug.Log("Swipe Left");
        CommitSwipe(SwipeDirection.left);
    }

    private void OnSwipeRight()
    {
        //Debug.Log("Swipe Right");
        CommitSwipe(SwipeDirection.right);
    }

    private void CommitSwipe(SwipeDirection swipeDirection)
    {
        if (_lastTouchedTile == null)
            return;

        bool success = false;
        Vector2 swipeDestination = Vector2.zero;

        switch (swipeDirection)
        {
            case SwipeDirection.up:
                swipeDestination = _lastTouchedTile.GetCoords + new Vector2(0, 1);
                break;

            case SwipeDirection.down:
                swipeDestination = _lastTouchedTile.GetCoords + new Vector2(0, -1);
                break;

            case SwipeDirection.left:
                swipeDestination = _lastTouchedTile.GetCoords + new Vector2(-1, 0);
                break;

            case SwipeDirection.right:
                swipeDestination = _lastTouchedTile.GetCoords + new Vector2(1, 0);
                break;
        }

        if (GridHandler.Instance.IsDestinationOccupied(swipeDestination) && GridHandler.Instance.IsInGridRange(swipeDestination))
            success = true;

        // Check Diagonal move
        if(swipeDestination.x != _lastTouchedTile.GetCoords.x && swipeDestination.y != _lastTouchedTile.GetCoords.y)
        {
            Debug.LogWarning("DIAGONAL MOVE IS ILLEGAL!");
            success = false;
        }

        if (success)
        {
            EventsHandler.Instance.OnTileMovement?.Invoke(_lastTouchedTile, swipeDestination);
        }

        Debug.Log("Swipe success: " + success);

        _detectSwipe = false;
    }
}
