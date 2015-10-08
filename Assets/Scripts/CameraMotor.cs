using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraMotor : MonoBehaviour
{
    public float Fade = 0.9f;
    public int PathLength = 10;
    public float BoundOffset = 50f;
    private float _boundPowOffset;
    public Transform Bounds;

    public float MaxZoom = 4f;
    public float ZoomFade = 0.9f;
    public float ZoomOffset = 0.95f;
    private float _minScale;
    private float _maxScale;

    private Camera _camera;
    private Vector2 _lastOffset;
    private const float _minOffset = 0.01f;
    private List<Vector2> _path = new List<Vector2>();
    private Rect _bounds;

    private Vector2 _virtualPos;
    private float _virtualScale;
    private Vector2 _size;

    void Start()
    {
        _camera = Camera.main;
        Vector2 corr = new Vector2(BoundOffset, BoundOffset);
        Vector2 size = (Vector2)Bounds.localScale - corr * 2f;
        Vector2 position = (Vector2)Bounds.position - size / 2f;
        _bounds = new Rect(position, size);
        Destroy(Bounds.gameObject);
        _virtualPos = transform.position;
        _boundPowOffset = Mathf.Pow(BoundOffset, 1.25f);
        _virtualScale = _camera.orthographicSize;
        _minScale = _virtualScale / MaxZoom;
        _maxScale = Mathf.Min(_bounds.height / 2f, _bounds.width / 2f * Screen.height / Screen.width) * ZoomOffset;
    }

    void Update()
    {
        float height = _camera.orthographicSize * 2f;
        float width = height * Screen.width / Screen.height;
        _size = new Vector2(width, height);
        Rect bounds = new Rect(_bounds.position + _size / 2, _bounds.size - _size);

        MoveCamera();
        ZoomCamera();

        float x = _virtualPos.x;
        float y = _virtualPos.y;
        float ox = 0f;
        float oy = 0f;
        if (x > bounds.xMax + _boundPowOffset) x = bounds.xMax + _boundPowOffset;
        if (x < bounds.xMin - _boundPowOffset) x = bounds.xMin - _boundPowOffset;
        if (y > bounds.yMax + _boundPowOffset) y = bounds.yMax + _boundPowOffset;
        if (y < bounds.yMin - _boundPowOffset) y = bounds.yMin - _boundPowOffset;
        _virtualPos = new Vector2(x, y);

        if (x > bounds.xMax) { x = bounds.xMax + Mathf.Pow(x - bounds.xMax, 0.8f); ox = -Mathf.Pow(x - bounds.xMax, 2f) / 800f; }
        if (x < bounds.xMin) { x = bounds.xMin - Mathf.Pow(bounds.xMin - x, 0.8f); ox = Mathf.Pow(bounds.xMin - x, 2f) / 800f; }
        if (y > bounds.yMax) { y = bounds.yMax + Mathf.Pow(y - bounds.yMax, 0.8f); oy = -Mathf.Pow(y - bounds.yMax, 2f) / 800f; }
        if (y < bounds.yMin) { y = bounds.yMin - Mathf.Pow(bounds.yMin - y, 0.8f); oy = Mathf.Pow(bounds.yMin - y, 2f) / 800f; }

        transform.position = new Vector3(x, y, transform.position.z);
        _lastOffset += new Vector2(ox, oy);

        if (_virtualScale > _maxScale / ZoomOffset) _virtualScale = _maxScale / ZoomOffset;
        if (_virtualScale < _minScale * ZoomOffset) _virtualScale = _minScale * ZoomOffset;

        _camera.orthographicSize = _virtualScale;
    }

    private void MoveCamera()
    {
        if (Input.touches.Length > 0)
        {
            var deltas = new List<Vector2>();
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    deltas.Add(touch.deltaPosition);
                }
            }
            if (deltas.Count > 0)
            {
                var delta = GetAverage(deltas);

                Vector2 offset = (_camera.ScreenToWorldPoint(Vector2.zero) - _camera.ScreenToWorldPoint(delta)) * 2f;

                _virtualPos += offset;
                _path.Add(offset);
                if (_path.Count == PathLength) _path.RemoveAt(0);
                _lastOffset = GetAverage(_path);
            }
        }
        else
        {
            if (_path.Count > 0)
                _path.Clear();
            if (Mathf.Abs(_lastOffset.x) > _minOffset || Mathf.Abs(_lastOffset.y) > _minOffset)
            {
                _lastOffset = _lastOffset * Fade;
                _virtualPos += _lastOffset;
            }
        }
    }

    private void ZoomCamera()
    {
        if (Input.touches.Length >= 2)
        {
            var touches = new Touch[2];
            int count = 0;
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    touches[count] = touch;
                    count++;
                    if (count == 2) break;
                }
            }
            if (count == 2)
            {
                float dist = Vector2.Distance(touches[0].position, touches[1].position);
                float prevDist = Vector2.Distance(touches[0].position - touches[0].deltaPosition * 2, touches[1].position - touches[1].deltaPosition * 2);

                _virtualScale = _camera.orthographicSize / dist * prevDist;


                //var scale = GetAverage(deltas);

                //Vector2 offset = (_camera.ScreenToWorldPoint(Vector2.zero) - _camera.ScreenToWorldPoint(delta)) * 2f;

                ////transform.Translate(offset);
                //_virtualPos += offset;
                //_path.Add(offset);
                //if (_path.Count == PathLength) _path.RemoveAt(0);
                //_lastOffset = GetAverage(_path);
                ////_lastOffset = offset;
                //Touch1.text = delta.ToString();
            }
        }
        else
        {
            if (_virtualScale > _maxScale) _virtualScale = _virtualScale * ZoomFade;
            if (_virtualScale < _minScale) _virtualScale = _virtualScale / ZoomFade;
            //if (_path.Count > 0)
            //    _path.Clear();
            //if (Mathf.Abs(_lastOffset.x) > _minOffset || Mathf.Abs(_lastOffset.y) > _minOffset)
            //{
            //    _lastOffset = _lastOffset * Fade;
            //    //transform.Translate(_lastOffset);
            //    _virtualPos += _lastOffset;
            //    Touch2.text = _lastOffset.ToString();
            //}
        }
    }

    Vector2 GetAverage(List<Vector2> list)
    {
        Vector2 result = Vector2.zero;
        foreach (var vec in list)
            result += vec;
        return result / list.Count;
    }

    Vector2 GetSumm(List<Vector2> list)
    {
        Vector2 result = Vector2.zero;
        foreach (var vec in list)
            result += vec;
        return result;
    }
}
