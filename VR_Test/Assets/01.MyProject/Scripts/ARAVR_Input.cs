using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! �̰��� Doxygen �� �ν��ϴ� �� �� �ּ�
//! ARAVR_Input Ŭ������ AR/VR �� �����ϴ� Input ����� �����ϴ� Ŭ�����̴�
public static class ARAVR_Input
{
    //! �پ��� ��⿡�� ����� ��ư�� ������ �̸� �����ص� ���̴�
    #region EnumType
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
    public enum ButtonTarget
    {
        Fire1,      /**< �߻� ��ư 1���̴�. */
        Fire2,      /**< �߻� ��ư 2���̴�. */
        Fire3,      /**< �߻� ��ư 3���̴�. */
        Jump        /**< ���� ��ư�̴�. */
    }
#endif

    //! �̸� �����س��� ��ư�� ��⺰�� �ٸ��� �����ص� ���̴�.
    public enum Button 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // PC BUILD �� ���� custom ��ó����, ����Ƽ ���� ��ó����
        One = ButtonTarget.Fire1,               /**< �߻� ��ư 1 �� ���� */
        Two = ButtonTarget.Jump,                /**< ���� ��ư�� ���� */
        Thumbstick = ButtonTarget.Fire1,        /**< �߻� ��ư 1�� ���� */
        IndexTrigger = ButtonTarget.Fire3,      /**< �߻� ��ư 3�� ���� */
        HandTrigger = ButtonTarget.Fire2        /**< �߻� ��ư 2�� ���� */
#endif
    }

    //! ��⺰�� �ٸ� ��Ʈ�ѷ��� �̸� �����ص� ���̴�
    public enum Controller 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE 
        LTouch,     /**< ���� ��Ʈ�ѷ� */
        RTouch      /**< ������ ��Ʈ�ѷ� */
#endif
    }
    #endregion


    #region ����
#if BUILD_PLATFORM_PC || UNITY_STANDALONE
    /**< ũ�ν���� �׸� �� ���� �������� ĳ���ϴ� ���� */
    private static Vector3 originalScale = Vector3.one * 0.02f;
#endif

#if TARGET_DEVICE_OCULUS
    /**< VR ���� ����� ī�޶� �������� ������ Tracking Space �� ������ �Ǵ� ���� */
    private static Transform rootTransform;
#endif
    #endregion


    /**
     * @brief ������ ��Ʈ�ѷ��� ��ġ�� ������ ������Ƽ�̴�
     * @return ��ũ�� ��ǥ�� �� ������ǥ�� ��ȯ�� ���� �����Ѵ�
     */
    #region ������ ��Ʈ�ѷ� ��ġ
    public static Vector3 RHandPosition
    {
        get
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
            // ���콺 ��ũ�� ��ǥ ������
            Vector3 pos = Input.mousePosition;
            // z ���� 0.7 m �� ����
            pos.z = 0.7f;
            // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
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
     * @brief ������ ��Ʈ�ѷ��� ������ ������ ������Ƽ�̴�
     * @return ī�޶� �������� ��Ʈ�ѷ��� ���� ������ �����ؼ� �����Ѵ�
     */
    #region ������ ��Ʈ�ѷ� ����
    public static Vector3 RHandDirection    
    {
        get 
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
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
     * @brief ���� ��Ʈ�ѷ��� ��ġ�� ������ ������Ƽ�̴�
     * @return ��ũ�� ��ǥ�� �� ������ǥ�� ��ȯ�� ���� �����Ѵ�
     */
    #region ���� ��Ʈ�ѷ� ��ġ
    public static Vector3 LHandPosition
    {
        get
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
            // ���콺 ��ũ�� ��ǥ ������
            Vector3 pos = Input.mousePosition;
            // z ���� 0.7 m �� ����
            pos.z = 0.7f;
            // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
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
     * @brief ���� ��Ʈ�ѷ��� ������ ������ ������Ƽ�̴�
     * @return ī�޶� �������� ��Ʈ�ѷ��� ���� ������ �����ؼ� �����Ѵ�
     */
    #region ���� ��Ʈ�ѷ� ����
    public static Vector3 LHandDirection
    {
        get
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
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


    #region Scene �� ��ϵ� ������ ��Ʈ�ѷ� ã�� ��ȯ
    private static Transform rHand; /**< Scene �� ��ϵ� ������ ��Ʈ�ѷ��� ĳ���ϴ� ���� */
    /**
     * @brief Scene �� ��ϵ� ������ ��Ʈ�ѷ��� ã�� ��ȯ�ϴ� ������Ƽ�̴�
     * @return ������ ��Ʈ�ѷ��� Transform �� �����Ѵ�
     */
    public static Transform RHand
    {
        get
        {
            if (lHand == null)
            {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
                // RHand ��� �̸����� ���� �������z�� �����.
                GameObject handObj = new GameObject("RHand");
                // ������� ��ü�� Ʈ�������� rHand ������ �Ҵ��Ѵ�.
                rHand = handObj.transform;
                // ��Ʈ�ѷ��� ī�޶��� �ڽ� ��ü�� ����Ѵ�.
                rHand.parent = Camera.main.transform;
#elif TARGET_DEVICE_OCULUS
                rHand = GameObject.Find("RightControllerAnchor").transform;
#endif // BUILD_PLATFORM_PC
            }
            return rHand;
        }
    }
    #endregion


    #region Scene �� ��ϵ� ���� ��Ʈ�ѷ� ã�� ��ȯ
    private static Transform lHand; /**< Scene �� ��ϵ� ���� ��Ʈ�ѷ��� ĳ���ϴ� ���� */
    /**
     * @brief Scene �� ��ϵ� ���� ��Ʈ�ѷ��� ã�� ��ȯ�ϴ� ������Ƽ�̴�
     * @return ���� ��Ʈ�ѷ��� Transform �� �����Ѵ�
     */
    public static Transform LHand
    {
        get
        {
            if(lHand == null) 
            {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
                // LHand ��� �̸����� ���� �������z�� �����.
                GameObject handObj = new GameObject("LHand");
                // ������� ��ü�� Ʈ�������� lHand ������ �Ҵ��Ѵ�.
                lHand = handObj.transform;
                // ��Ʈ�ѷ��� ī�޶��� �ڽ� ��ü�� ����Ѵ�.
                lHand.parent = Camera.main.transform;
#elif TARGET_DEVICE_OCULUS
                lHand = GameObject.Find("LeftControllerAnchor").transform;
#endif // BUILD_PLATFORM_PC
            }
            return lHand;
        }
    }
    #endregion


    #region ��Ʈ�ѷ��� Ư�� ��ư�� ������ �ִ� ���� true �� ��ȯ
    /**
     * @brief ��Ʈ�ѷ��� Ư�� ��ư�� ������ �ִ� ���� true �� ��ȯ�ϴ� �Լ�
     * @param virtualMask : �̸� ���ǵ� ��ư�� ������ �޾ƿ´�
     * @param hand : ��� ��Ʈ�ѷ��� ��ư�� ���� ������ �޾ƿ´�
     * @return virtualMask �� ���� ���� ButtonTarget Ÿ������ ��ȯ�ؼ� �����Ѵ�
     */
    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch) 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
        // virtualMask �� ���� ���� ButtonTarget Ÿ������ ��ȯ�ؼ� �����Ѵ�
        return Input.GetButton(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif
    }
    #endregion


    #region ��Ʈ�ѷ��� Ư�� ��ư�� ������ �� true �� ��ȯ
    /**
     * @brief ��Ʈ�ѷ��� Ư�� ��ư�� ������ �� true �� ��ȯ�ϴ� �Լ�
     * @param virtualMask : �̸� ���ǵ� ��ư�� ������ �޾ƿ´�
     * @param hand : ��� ��Ʈ�ѷ��� ��ư�� ���� ������ �޾ƿ´�
     * @return virtualMask �� ���� ���� ButtonTarget Ÿ������ ��ȯ�ؼ� �����Ѵ�
     */
    public static bool GetDown(Button virtualMask, Controller hand = Controller.RTouch) 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
        // virtualMask �� ���� ���� ButtonTarget Ÿ������ ��ȯ�ؼ� �����Ѵ�
        return Input.GetButtonDown(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif
    }
    #endregion


    #region ��Ʈ�ѷ��� Ư�� ��ư�� ������ ������ �� true �� ��ȯ
    /**
     * @brief ��Ʈ�ѷ��� Ư�� ��ư�� ������ ������ �� true �� ��ȯ�ϴ� �Լ�
     * @param virtualMask : �̸� ���ǵ� ��ư�� ������ �޾ƿ´�
     * @param hand : ��� ��Ʈ�ѷ��� ��ư�� ���� ������ �޾ƿ´�
     * @return virtualMask �� ���� ���� ButtonTarget Ÿ������ ��ȯ�ؼ� �����Ѵ�
      */
    public static bool GetUp(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
        // virtualMask �� ���� ���� ButtonTarget Ÿ������ ��ȯ�ؼ� �����Ѵ�
        return Input.GetButtonUp(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif
    }
    #endregion


    #region ��Ʈ�ѷ��� Axis �Է��� ��ȯ
    /**
     * @brief ��Ʈ�ѷ��� Axis �Է��� ��ȯ�ϴ� �Լ�
     * @param axis : Horizontal, Vertical ���� �޾ƿ´�
     * @param hand : ��� ��Ʈ�ѷ��� ��ư�� ���� ������ �޾ƿ´�
     * @return ��Ʈ�ѷ��� Axis �Է��� �Ǽ� ���·� �����Ѵ�
     */
    public static float GetAxis(string axis, Controller hand = Controller.LTouch) 
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
        return Input.GetAxis(axis);
#else
        return default;
#endif
    }
    #endregion


    #region ��Ʈ�ѷ��� ���� ȣ���ϱ�
    public static void PlayVibration(Controller hand) 
    {
        
    }
    #endregion


    #region ī�޶� �ٶ󺸴� ������ �������� ���͸� ��´�
    public static void Recenter()
    {

    }
    #endregion


    #region Ÿ���� �ٶ󺸴� ������ �������� ���͸� ��´�
    /**
     * @brief Ÿ���� �ٶ󺸴� ������ �������� ���͸� ��´�
     * @param target : �ٶ󺸴� ������ ������ Target �� Transform �� �޾ƿ´�
     * @param direction : �ٶ� ������ �޾ƿ´�
     */
    public static void Recenter(Transform target, Vector3 direction)
    {
        target.forward = target.rotation * direction;
    }
    #endregion


    #region ���̰� ��� ���� ũ�ν��� ��ġ��Ű�� �ʹ�
    /**
     * @brief ���̰� ��� ���� ũ�ν��� ��ġ��Ű�� �Լ�
     * @param crosshair : ��Ʈ�ѷ��� ���鿡 �׸� ũ�ν���� ������Ʈ�� Transform �� �޾ƿ´�
     * @param isHand : ��Ʈ�ѷ��� �����ϴ��� ���θ� �޾ƿ´�
     * @param hand : 
     */
    public static void DrawCrosshair(Transform crosshair, bool isHand = true,
        Controller hand = Controller.RTouch) 
    {
        Ray ray;
        // ��Ʈ�ѷ��� ��ġ�� ������ �̿��� ���̸� �����Ѵ�
        if (isHand) 
        {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if �� && || ���� ����
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif
        }
        else 
        {
            // ī�޶� �������� ȭ���� ���߾����� ���̸� ���
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }

        // ���� �Ⱥ��̴� Plane �� �����
        Plane plane = new Plane(Vector3.up, 0);
        float distance = 0;
        // Plane �� �̿��� ray �� ���

        // ���̿� �浹�� ������ �ִ� ���
        if (plane.Raycast(ray, out distance)) 
        {
            // ������ GetPoint �Լ��� �̿��ؼ� �浹 ������ ��ġ�� �����´�
            crosshair.position = ray.GetPoint(distance);
            crosshair.forward = -Camera.main.transform.forward;
            // ũ�ν������ ũ�⸦ �ּ� �⺻ ũ�⿡�� �Ÿ��� ���� �� Ŀ������ �Ѵ�
            crosshair.localScale = originalScale * Mathf.Max(1.0f, distance);
        }
        // ���� ���
        else 
        {
            crosshair.position = ray.origin + ray.direction * 100.0f;
            crosshair.forward = -Camera.main.transform.forward;
            distance = (crosshair.position - ray.origin).magnitude;
            crosshair.localScale = originalScale * Mathf.Max(1.0f, distance);
        }
    }
    #endregion


    #region TrackingSpace �� Transform �� �����ϴ� �Լ�
    /**
     * @brief TrackingSpace �� Transform �� �����ϴ� �Լ�
     * @return TrackingSpace �� Transform 
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

