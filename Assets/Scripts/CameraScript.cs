using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    [SerializeField] private GameObject startButton;

    private Camera _camera;
    
    private float _targetZoomSize = 1f; // 목표 카메라 크기

    private Vector3 targetWorldPos;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _targetZoomSize = _camera.orthographicSize;
        transform.position = new Vector3(0, -60, -10);
        startButton.transform.position = Camera.main.WorldToScreenPoint(new Vector3(0, -2f, 0));
        Debug.Log(_targetZoomSize);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < 300 && !Player.instance.opening) 
            transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
        if (Player.instance.fallingTime > 1.5f)
        {
            _targetZoomSize = Mathf.Clamp(_targetZoomSize - Time.deltaTime, 3.5f, 5f);
            targetWorldPos = player.position;
            if (_camera.orthographicSize < 4.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, 
                    new Vector3(player.position.x / 2f, player.position.y, transform.position.z), Time.deltaTime*6);
            }
            //transform.position.x + (player.position.x/2 - transform.position.x) / Mathf.Abs(player.position.x)
        }
        else if(!Player.instance.ending)
        {
            _targetZoomSize = Mathf.Clamp(_targetZoomSize + Time.deltaTime*3f, 3.5f, 5f);
            targetWorldPos = new Vector3(transform.position.x, player.position.y, -10);
        }

        if (Player.instance.ending)
        {
            _targetZoomSize = Mathf.Clamp(_targetZoomSize + Time.deltaTime/2f, 5f, 10f);
            targetWorldPos = player.position;
        }
        else if (Player.instance.opening)
        {
            var p = Mathf.Clamp(transform.position.y + Time.deltaTime * 2.5f, -60, player.position.y);
            startButton.transform.position 
                = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, -60-p + -2f, 0));
            transform.position = new Vector3(0, p, -10);
            if (p >= player.position.y) Player.instance.opening = false;
        }
        else if (_camera.orthographicSize >= 4.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(0, player.position.y, -10), Time.deltaTime*6);
        }
        

        UpdateZoom();
    }
    
    private void UpdateZoom()
    {
        if (Mathf.Abs(_targetZoomSize - _camera.orthographicSize) < Mathf.Epsilon)
        {
            return;
        }

        //_camera.ScreenToWorldPoint(player.position);
        var cameraTransform = transform;
        var currentCameraPosition = cameraTransform.position;
        var offsetCamera = targetWorldPos - currentCameraPosition - (targetWorldPos - currentCameraPosition) / (_camera.orthographicSize/_targetZoomSize);

        // 카메라 크기 갱신
        _camera.orthographicSize = _targetZoomSize;
            
        // 줌 비율에 의한 카메라 위치 조정
        currentCameraPosition += offsetCamera;
        cameraTransform.position = currentCameraPosition;
    }
}
