using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movimento")]
    public float gravidade = -9.81f;
    public float speed = 6f;
    public float jumpForce = 2f;

    private CharacterController controller;
    private Vector3 velocity;

    Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    [Header("Mouse")]
    public float mouseSensibilidade = 200f;
    float xRotation = 0f;

    bool mouseTravado = true;

    [Header("Câmeras")]
    Camera primeiraPessoa;
    Camera segundaPessoa;
    Camera terceiraPessoa;

    int cameraAtual = 0;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        primeiraPessoa = transform.Find("CameraPrimeiraPessoa").GetComponent<Camera>();
        segundaPessoa = transform.Find("CameraSegundaPessoa").GetComponent<Camera>();
        terceiraPessoa = transform.Find("CameraTerceiraPessoa").GetComponent<Camera>();

        groundCheck = transform.Find("GroundCheck");

        AtivarCamera(0);

        TravarMouse(true);
    }

    void Update()
    {
        RotacaoMouse();
        Movimento();
        GravidadeEPulo();
        TrocarCamera();
        ControleMouse();
    }

    void RotacaoMouse()
    {
        if(!mouseTravado) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensibilidade * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensibilidade * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        Camera camAtual = CameraAtual();
        camAtual.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void Movimento()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    void GravidadeEPulo()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if(Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravidade);

        velocity.y += gravidade * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void TrocarCamera()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            cameraAtual++;

            if(cameraAtual > 2)
                cameraAtual = 0;

            AtivarCamera(cameraAtual);
        }
    }

    void ControleMouse()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            mouseTravado = !mouseTravado;
            TravarMouse(mouseTravado);
        }
    }

    void TravarMouse(bool travar)
    {
        if(travar)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Camera CameraAtual()
    {
        if(cameraAtual == 0) return primeiraPessoa;
        if(cameraAtual == 1) return segundaPessoa;
        return terceiraPessoa;
    }

    void AtivarCamera(int id)
    {
        primeiraPessoa.enabled = false;
        segundaPessoa.enabled = false;
        terceiraPessoa.enabled = false;

        if(id == 0) primeiraPessoa.enabled = true;
        if(id == 1) segundaPessoa.enabled = true;
        if(id == 2) terceiraPessoa.enabled = true;
    }
}