using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! 이것은 Doxygen 이 인식하는 한 줄 주석
//! ARAVR_Input 클래스는 AR/VR 에 대응하는 Input 기능을 구현하는 클래스이다
public static class ARAVR_Input
{
    //! 다양한 기기에서 사용할 버튼의 종류를 미리 정의해둔 것이다
    #region EnumType
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
    public enum ButtonTarget
    {
        Fire1,      /**< 발사 버튼 1번이다. */
        Fire2,      /**< 발사 버튼 2번이다. */
        Fire3,      /**< 발사 버튼 3번이다. */
        Jump        /**< 점프 버튼이다. */
    }
#endif

    //! 미리 정의해놓은 버튼을 기기별로 다르게 매핑해둔 것이다.
    public enum Button 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // PC BUILD 를 위한 custom 전처리기, 유니티 제공 전처리기
        One = ButtonTarget.Fire1,               /**< 발사 버튼 1 을 매핑 */
        Two = ButtonTarget.Jump,                /**< 점프 버튼을 매핑 */
        Thumbstick = ButtonTarget.Fire1,        /**< 발사 버튼 1을 매핑 */
        IndexTrigger = ButtonTarget.Fire3,      /**< 발사 버튼 3을 매핑 */
        HandTrigger = ButtonTarget.Fire2        /**< 발사 버튼 2을 매핑 */
#endif
    }

    //! 기기별로 다른 컨트롤러를 미리 정의해둔 것이다
    public enum Controller 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE 
        LTouch,     /**< 왼쪽 컨트롤러 */
        RTouch      /**< 오른쪽 컨트롤러 */
#endif
    }
    #endregion


    #region 변수
#if BUILD_PLATFORM_PC || UNITY_STANDALONE
    /**< 크로스헤어 그릴 때 기존 스케일을 캐싱하는 변수 */
    private static Vector3 originalScale = Vector3.one * 0.02f;
#endif

#if TARGET_DEVICE_OCULUS
    /**< VR 에서 사용할 카메라를 기준으로 연산한 Tracking Space 의 기준이 되는 변수 */
    private static Transform rootTransform;
#endif
    #endregion


    /**
     * @brief 오른쪽 컨트롤러의 위치를 얻어오는 프로퍼티이다
     * @return 스크린 좌표를 얻어서 월드좌표로 변환한 값을 리턴한다
     */
    #region 오른쪽 컨트롤러 위치
    public static Vector3 RHandPosition
    {
        get
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
            // 마우스 스크린 좌표 얻어오기
            Vector3 pos = Input.mousePosition;
            // z 값은 0.7 m 로 설정
            pos.z = 0.7f;
            // 스크린 좌표를 월드 좌표로 변환
            pos = Camera.main.ScreenToWorldPoint(pos);
            RHand.position = pos;
#elif TARGET_DEVICE_OCULUS
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            pos = GetTransform().TransformPoint(pos);
#else
            Vector3 pos = default;
#endif
            return pos;
        }
    }
#endregion


    /**
     * @brief 오른쪽 컨트롤러의 방향을 얻어오는 프로퍼티이다
     * @return 카메라를 기준으로 컨트롤러의 정면 방향을 연산해서 리턴한다
     */
    #region 오른쪽 컨트롤러 방향
    public static Vector3 RHandDirection    
    {
        get 
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
            Vector3 direction = RHandPosition - Camera.main.transform.position;
            RHand.forward = direction;
#elif TARGET_DEVICE_OCULUS
            Vector3 direction = 
            OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
#else
            Vector3 direction = default;
#endif
            return direction;
        }
    }
    #endregion


    /**
     * @brief 왼쪽 컨트롤러의 위치를 얻어오는 프로퍼티이다
     * @return 스크린 좌표를 얻어서 월드좌표로 변환한 값을 리턴한다
     */
    #region 왼쪽 컨트롤러 위치
    public static Vector3 LHandPosition
    {
        get
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
            // 마우스 스크린 좌표 얻어오기
            Vector3 pos = Input.mousePosition;
            // z 값은 0.7 m 로 설정
            pos.z = 0.7f;
            // 스크린 좌표를 월드 좌표로 변환
            pos = Camera.main.ScreenToWorldPoint(pos);
            LHand.position = pos;
#elif TARGET_DEVICE_OCULUS
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            pos = GetTransform().TransformPoint(pos);
#else
            Vector3 pos = default;
#endif
            return pos;
        }
    }
    #endregion


    /**
     * @brief 왼쪽 컨트롤러의 방향을 얻어오는 프로퍼티이다
     * @return 카메라를 기준으로 컨트롤러의 정면 방향을 연산해서 리턴한다
     */
    #region 왼쪽 컨트롤러 방향
    public static Vector3 LHandDirection
    {
        get
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
            Vector3 direction = LHandPosition - Camera.main.transform.position;
            LHand.forward = direction;
#elif TARGET_DEVICE_OCULUS
            Vector3 direction = 
            OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
#else
            Vector3 direction = default;
#endif
            return direction;
        }
    }
    #endregion


    #region Scene 에 등록된 오른쪽 컨트롤러 찾아 반환
    private static Transform rHand; /**< Scene 에 등록된 오른쪽 컨트롤러를 캐싱하는 변수 */
    /**
     * @brief Scene 에 등록된 오른쪽 컨트롤러를 찾아 반환하는 프로퍼티이다
     * @return 오른쪽 컨트롤러의 Transform 을 리턴한다
     */
    public static Transform RHand
    {
        get
        {
            if (lHand == null)
            {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
                // RHand 라는 이름으로 게임 오브젝틑를 만든다.
                GameObject handObj = new GameObject("RHand");
                // 만들어진 객체의 트랜스폰을 rHand 변수에 할당한다.
                rHand = handObj.transform;
                // 컨트롤러를 카메라의 자식 객체로 등록한다.
                rHand.parent = Camera.main.transform;
#elif TARGET_DEVICE_OCULUS
                rHand = GameObject.Find("RightControllerAnchor").transform;
#endif // BUILD_PLATFORM_PC
            }
            return rHand;
        }
    }
    #endregion


    #region Scene 에 등록된 왼쪽 컨트롤러 찾아 반환
    private static Transform lHand; /**< Scene 에 등록된 왼쪽 컨트롤러를 캐싱하는 변수 */
    /**
     * @brief Scene 에 등록된 왼쪽 컨트롤러를 찾아 반환하는 프로퍼티이다
     * @return 왼쪽 컨트롤러의 Transform 을 리턴한다
     */
    public static Transform LHand
    {
        get
        {
            if(lHand == null) 
            {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
                // LHand 라는 이름으로 게임 오브젝틑를 만든다.
                GameObject handObj = new GameObject("LHand");
                // 만들어진 객체의 트랜스폰을 lHand 변수에 할당한다.
                lHand = handObj.transform;
                // 컨트롤러를 카메라의 자식 객체로 등록한다.
                lHand.parent = Camera.main.transform;
#elif TARGET_DEVICE_OCULUS
                lHand = GameObject.Find("LeftControllerAnchor").transform;
#endif // BUILD_PLATFORM_PC
            }
            return lHand;
        }
    }
    #endregion


    #region 컨트롤러의 특정 버튼을 누르고 있는 동안 true 를 반환
    /**
     * @brief 컨트롤러의 특정 버튼을 누르고 있는 동안 true 를 반환하는 함수
     * @param virtualMask : 미리 정의된 버튼의 종류를 받아온다
     * @param hand : 어느 컨트롤러의 버튼을 누를 것인지 받아온다
     * @return virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 리턴한다
     */
    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch) 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 전달한다
        return Input.GetButton(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif
    }
    #endregion


    #region 컨트롤러의 특정 버튼을 눌렀을 때 true 를 반환
    /**
     * @brief 컨트롤러의 특정 버튼을 눌렀을 때 true 를 반환하는 함수
     * @param virtualMask : 미리 정의된 버튼의 종류를 받아온다
     * @param hand : 어느 컨트롤러의 버튼을 누를 것인지 받아온다
     * @return virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 리턴한다
     */
    public static bool GetDown(Button virtualMask, Controller hand = Controller.RTouch) 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 전달한다
        return Input.GetButtonDown(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif
    }
    #endregion


    #region 컨트롤러의 특정 버튼을 눌렀다 때었을 때 true 를 반환
    /**
     * @brief 컨트롤러의 특정 버튼을 눌렀다 때었을 때 true 를 반환하는 함수
     * @param virtualMask : 미리 정의된 버튼의 종류를 받아온다
     * @param hand : 어느 컨트롤러의 버튼을 누를 것인지 받아온다
     * @return virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 리턴한다
      */
    public static bool GetUp(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 전달한다
        return Input.GetButtonUp(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif
    }
    #endregion


    #region 컨트롤러의 Axis 입력을 반환
    /**
     * @brief 컨트롤러의 Axis 입력을 반환하는 함수
     * @param axis : Horizontal, Vertical 값을 받아온다
     * @param hand : 어느 컨트롤러의 버튼을 누를 것인지 받아온다
     * @return 컨트롤러의 Axis 입력을 실수 형태로 리턴한다
     */
    public static float GetAxis(string axis, Controller hand = Controller.LTouch) 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
        return Input.GetAxis(axis);
#else
        return default;
#endif
    }
    #endregion


    #region 컨트롤러에 진동 호출하기
    public static void PlayVibration(Controller hand) 
    {
        
    }
    #endregion


    #region 카메라가 바라보는 방향을 기준으로 센터를 잡는다
    public static void Recenter()
    {

    }
    #endregion


    #region 타겟이 바라보는 방향을 기준으로 센터를 잡는다
    /**
     * @brief 타겟이 바라보는 방향을 기준으로 센터를 잡는다
     * @param target : 바라보는 방향을 결정할 Target 의 Transform 을 받아온다
     * @param direction : 바라볼 방향을 받아온다
     */
    public static void Recenter(Transform target, Vector3 direction)
    {
        target.forward = target.rotation * direction;
    }
    #endregion


    #region 레이가 닿는 곳에 크로스헤어를 위치시키고 싶다
    /**
     * @brief 레이가 닿는 곳에 크로스헤어를 위치시키는 함수
     * @param crosshair : 컨트롤러의 정면에 그릴 크로스헤어 오브젝트의 Transform 을 받아온다
     * @param isHand : 컨트롤러가 존재하는지 여부를 받아온다
     * @param hand : 
     */
    public static void DrawCrosshair(Transform crosshair, bool isHand = true,
        Controller hand = Controller.RTouch) 
    {
        Ray ray;
        // 컨트롤러의 위치와 방향을 이용해 레이를 제작한다
        if (isHand) 
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif
        }
        else 
        {
            // 카메라를 기준으로 화면의 정중앙으로 레이를 쏜다
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }

        // 눈에 안보이는 Plane 을 만든다
        Plane plane = new Plane(Vector3.up, 0);
        float distance = 0;
        // Plane 을 이용해 ray 를 쏜다

        // 레이에 충돌한 지점이 있는 경우
        if (plane.Raycast(ray, out distance)) 
        {
            // 레이의 GetPoint 함수를 이용해서 충돌 지점의 위치를 가져온다
            crosshair.position = ray.GetPoint(distance);
            crosshair.forward = -Camera.main.transform.forward;
            // 크로스헤어의 크기를 최소 기본 크기에서 거리에 따라 더 커지도록 한다
            crosshair.localScale = originalScale * Mathf.Max(1.0f, distance);
        }
        // 없는 경우
        else 
        {
            crosshair.position = ray.origin + ray.direction * 100.0f;
            crosshair.forward = -Camera.main.transform.forward;
            distance = (crosshair.position - ray.origin).magnitude;
            crosshair.localScale = originalScale * Mathf.Max(1.0f, distance);
        }
    }
    #endregion


    #region TrackingSpace 의 Transform 을 리턴하는 함수
    /**
     * @brief TrackingSpace 의 Transform 을 리턴하는 함수
     * @return TrackingSpace 의 Transform 
     */
#if TARGET_DEVICE_OCULUS
    private static Transform GetTransform() 
    {
        if (rootTransform == null) 
        {
            rootTransform = GameObject.Find("TrackingSpace").transform;
        }

        return rootTransform;
    }
#endif
#endregion
}

