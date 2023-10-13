using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 이동 속도
    public float speed = 5f;
    // 캐릭터 컨트롤러 컴포넌트
    private CharacterController characterController = default;

    // 점프 크기
    public float jumpPower = 5f;

    // { 중력 관련 변수
    // 중력 가속도의 크기
    public float gravity = -20f;
    // 수직 속도
    float yVelocity = 0;
    // } 중력 관련 변수

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 사용자의 입력
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");

        // 방향
        Vector3 direction = new Vector3(h, 0, v);

        // 사용자가 바라보는 방향으로 입력 값 변화
        direction = Camera.main.transform.TransformDirection(direction);

        // { 업데이트 타임에 중력으르 적용하는 로직
        yVelocity += gravity * Time.deltaTime;

        // 바닥에 있을 경우, 수직 항력을 처리하기 위해 속도를 0으로 한다
        if (characterController.isGrounded)
        {
            yVelocity = 0;
        }

        // 사용자가 점프 버튼을 누르면 속도에 점프 크기를 할당한다
        if (ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
        {
            yVelocity = jumpPower;
        }

        direction.y = yVelocity;

        characterController.Move(direction * speed * Time.deltaTime);
        // } 업데이트 타임에 중력으르 적용하는 로직
    }
}
