using UnityEngine;

public class SceneCameraControl : MonoBehaviour
{
    private Vector3 moveTarget = Vector3.zero;
    private Vector3 rotateTarget = new Vector3(0, 0, 1);

    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    public enum MouseMove
    {
        X = 0,
        Y = 1,
        ScrollWheel = 2
    }

    private static readonly string[] MouseMoveString = new string[]
    {
        "Mouse X",
        "Mouse Y",
        "Mouse ScrollWheel"
    };

    public MouseMove moveTrigger = MouseMove.ScrollWheel;
    public bool enableMove = true;
    public bool invertMoveDirection = false;
    public float moveSpeed = 6f;
    public bool limitMoveX = false;
    public bool limitMoveY = false;
    public bool limitMoveZ = false;
    public bool smoothMove = true;
    public float smoothMoveSpeed = 10f;

    public MouseButton rotateTrigger = MouseButton.Right;
    public bool enableRotate = true;
    public bool invertRotateDirection = false;
    public float rotateSpeed = 3f;
    public bool limitRotateX = false;
    public bool limitRotateY = false;
    public bool smoothRotate = true;
    public float smoothRotateSpeed = 10f;

    public MouseButton gripTrigger = MouseButton.Middle;
    public bool enableGrip = true;
    public bool invertGripDirection = false;
    public float gripSpeed = 3f;
    public bool limitGripX = false;
    public bool limitGripY = false;
    public bool smoothGrip = true;
    public float smoothGripSpeed = 10f;

    void Start()
    {
        this.moveTarget = this.transform.position;
    }

	void Update ()
    {
        Move();
        Rotate();
        Grip();
    }

    // 入力が有効なときだけ target を更新します。
    // 入力が有効でないとき、すべての処理をスキップすると、
    // Lerp, Slerp によるスムーズな移動・回転が実行されなくなります。

    private void Move()
    {
        if (!this.enableMove)
        {
            return;
        }

        float moveAmount = Input.GetAxis(SceneCameraControl.MouseMoveString[(int)this.moveTrigger]);

        if (moveAmount != 0)
        {
            float direction = this.invertMoveDirection ? -1 : 1;
            this.moveTarget = this.transform.forward;
            this.moveTarget *= this.moveSpeed * moveAmount * direction;
            this.moveTarget += this.transform.position;

            if (this.limitMoveX)
            {
                this.moveTarget.x = 0;
            }

            if (this.limitMoveY)
            {
                this.moveTarget.y = 0;
            }

            if (this.limitMoveZ)
            {
                this.moveTarget.z = 0;
            }
        }

        if (this.smoothMove)
        {
            if (this.moveTarget == this.transform.position)
            {
                this.moveTarget = this.transform.position;
            }

            this.transform.position =
                Vector3.Lerp(this.transform.position,
                             this.moveTarget,
                             Time.deltaTime * this.smoothMoveSpeed);
        }
        else
        {
            this.transform.position = moveTarget;
        }
    }

    private void Rotate()
    {
        if (!this.enableRotate)
        {
            return;
        }

        float direction = this.invertRotateDirection ? -1 : 1;
        float mouseX = Input.GetAxis(SceneCameraControl.MouseMoveString[(int)MouseMove.X]) * direction;
        float mouseY = Input.GetAxis(SceneCameraControl.MouseMoveString[(int)MouseMove.Y]) * direction;

        if (Input.GetMouseButton((int)this.rotateTrigger))
        {
            if (!this.limitRotateX)
            {
                this.rotateTarget = Quaternion.Euler(0, mouseX * this.rotateSpeed, 0) * this.rotateTarget;
            }

            if (!this.limitRotateY)
            {
                this.rotateTarget = 
                    Quaternion.AngleAxis(mouseY * this.rotateSpeed,
                                         Vector3.Cross(this.transform.forward, Vector3.up)) * this.rotateTarget;
            }
        }

        if (this.smoothRotate)
        {
            this.transform.rotation =
                Quaternion.Slerp(this.transform.rotation,
                                 Quaternion.LookRotation(this.rotateTarget),
                                 Time.deltaTime * this.smoothRotateSpeed);
        }
        else
        {
            this.transform.rotation = Quaternion.LookRotation(this.rotateTarget);
        }
    }

    private void Grip()
    {
        if (!this.enableGrip)
        {
            return;
        }

        // direction の方向が Move, Rotate と逆方向な点に注意する。

        float direction = this.invertGripDirection ? 1 : -1;
        float mouseX = Input.GetAxis(SceneCameraControl.MouseMoveString[(int)MouseMove.X]) * direction;
        float mouseY = Input.GetAxis(SceneCameraControl.MouseMoveString[(int)MouseMove.Y]) * direction;

        if (Input.GetMouseButton((int)this.gripTrigger))
        {
            this.moveTarget = this.transform.position;

            if (!this.limitGripX)
            {
                this.moveTarget += this.transform.right * mouseX * gripSpeed;
            }

            if (!this.limitGripY)
            {
                this.moveTarget += Vector3.up * mouseY * gripSpeed;
            }
        }

        if (this.smoothGrip)
        {
            this.transform.position =
                Vector3.Lerp(this.transform.position,
                             this.moveTarget,
                             Time.deltaTime * this.smoothGripSpeed);
        }
        else
        {
            this.transform.position = this.moveTarget;
        }
    }
}