using UnityEngine;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    [Range(0f, 359f)]
    private float Alfa, Beta, Gamma;
    private float theta1, theta2, theta3, theta4, theta5, theta6;

    [SerializeField] AxisController axisController;
    private Vector3 posBefore;

    public Transform Link0;
    public Transform Link1;
    public Transform Link2;
    public Transform roll;
    public Transform pitch;
    public Transform yaw;
    public Slider anguleAlfa;
    public Slider anguleBeta;
    public Slider anguleGamma;
    public Transform tweezers;

    public float dLink0, dLink1, dLink2, drpy, dTweezer;
    public float velocity = 2;


    private void Start()
    {
        posBefore = Vector3.zero;
        anguleAlfa = GameObject.Find("Alfa").GetComponent<Slider>();
        anguleBeta = GameObject.Find("Beta").GetComponent<Slider>();
        anguleGamma = GameObject.Find("Gamma").GetComponent<Slider>();
        axisController = GameObject.Find("AxisController").GetComponent<AxisController>();
    }

    private void Movement()
    {
        transform.localPosition += axisController.GetAxis() * velocity * Time.deltaTime;
        transform.localPosition = transform.localPosition.y < 0? new Vector3(transform.localPosition.x, 0, transform.localPosition.z) : transform.localPosition;
    }

    private void RobotMovement()
    {
        if(axisController.GetAxis() != Vector3.zero) { Movement(); }
        float axisX = transform.localPosition.z;
        float axisZ = transform.localPosition.y;
        float axisY = transform.localPosition.x;
        InverseKinematic(axisX, axisY, axisZ);
        if (float.IsNaN(theta2) == false)
        {
            posBefore = new Vector3(axisY, axisZ, axisX);
            Link0.localEulerAngles = new Vector3(0f, theta1, 0f);
            Link1.localEulerAngles = new Vector3(theta2, 0f, 0f);
            Link2.localEulerAngles = new Vector3(theta3 + 90f, 0f, 0f);//Sumamos 90 grados
            roll.localEulerAngles = new Vector3(0f, 0f, theta4);
            pitch.localEulerAngles = new Vector3(theta5, 0f, 0f);
            yaw.localEulerAngles = new Vector3(0f, 0f, theta6);
        }
        else
        {
            transform.localPosition = posBefore;
        }
    }

    private void InverseKinematic(float axisX, float axisY, float axisZ)
    {
        float cosAlfa, senAlfa, cosBeta, senBeta, cosGamm, senGamm, r11, r21, r31, r12, r22, r32, r13, r23, r33, a2, d4, d6,
                      a22, d42, x2, X2, Y2, y2, z2, K, X, Y, Raiz3, costheta1, sentheta1, costheta3,
                      sentheta3, costheta2, sentheta2, costheta23, sentheta23, costheta4, sentheta4, costheta5,
                      sentheta5, x61, x62, x63, theta23, xc, yc, zc;


        Alfa = anguleAlfa.value;
        Beta = anguleBeta.value;
        Gamma = anguleGamma.value;

        cosAlfa = Mathf.Cos(-Alfa * Mathf.Deg2Rad);

        senAlfa = Mathf.Sin(-Alfa * Mathf.Deg2Rad);

        cosBeta = Mathf.Cos(Beta * Mathf.Deg2Rad);

        senBeta = Mathf.Sin(Beta * Mathf.Deg2Rad);

        cosGamm = Mathf.Cos(Gamma * Mathf.Deg2Rad);

        senGamm = Mathf.Sin(Gamma * Mathf.Deg2Rad);


        //'X-Y-Z

        //'X'

        r11 = (cosAlfa * cosBeta);    // 'r11

        r21 = (senAlfa * cosBeta);    // 'r21

        r31 = -senBeta;            //  'r31

        //'Y'

        r12 = (cosAlfa * senBeta * senGamm) - (senAlfa * cosGamm); //'r12

        r22 = (senAlfa * senBeta * senGamm) + (cosAlfa * cosGamm);// 'r22

        r32 = (cosBeta * senGamm);                        // 'r32

        //'Z'

        r13 = (cosAlfa * senBeta * cosGamm) + (senAlfa * senGamm);

        r23 = (senAlfa * senBeta * cosGamm) - (cosAlfa * senGamm);

        r33 = (cosBeta * cosGamm);


        a2 = dLink1;

        d4 = dLink2;

        d6 = drpy + dTweezer;

        a22 = a2 * a2;

        d42 = d4 * d4;


        xc = axisX - d6 * r13;

        yc = axisY - d6 * r23;

        zc = (axisZ - dLink0) - d6 * r33;


        x2 = xc * xc;

        y2 = yc * yc;

        z2 = zc * zc;

        //'Calculo theta1


        theta1 = (Mathf.Atan2(yc, xc)) * Mathf.Rad2Deg;


        //'Calculo theta3


        K = (x2 + y2 + z2 - a22 - d42) / (2 * a2);

        Raiz3 = Mathf.Sqrt((Mathf.Abs(d42)) - K * K);
        //Debug.Log("NaN: " + Raiz3.ToString());

        theta3 = -(Mathf.Atan2(K, Raiz3)) * Mathf.Rad2Deg;


        costheta1 = Mathf.Cos(theta1 * Mathf.Deg2Rad);

        sentheta1 = Mathf.Sin(theta1 * Mathf.Deg2Rad);

        costheta3 = Mathf.Cos(theta3 * Mathf.Deg2Rad);

        sentheta3 = Mathf.Sin(theta3 * Mathf.Deg2Rad);


        Y = (-a2 * costheta3) * zc - (costheta1 * xc + sentheta1 * yc) * (d4 - a2 * sentheta3);

        X = ((a2 * sentheta3 - d4) * zc + (a2 * costheta3) * (costheta1 * xc + sentheta1 * yc));


        //'Calculo theta2


        theta23 = (Mathf.Atan2(Y, X)) * Mathf.Rad2Deg;

        theta2 = theta23 - theta3;

        costheta2 = Mathf.Cos(theta2 * Mathf.Deg2Rad);

        sentheta2 = Mathf.Sin(theta2 * Mathf.Deg2Rad);


        costheta23 = (costheta2 * costheta3) - (sentheta2 * sentheta3);

        sentheta23 = (sentheta2 * costheta3) + (costheta2 * sentheta3);


        //'Calculo theta4


        Y = ((costheta1 * r23) - (sentheta1 * r13));

        X = (-(costheta1 * costheta23 * r13) - (sentheta1 * costheta23 * r23) + (sentheta23 * r33));


        theta4 = (Mathf.Atan2(Y, X)) * Mathf.Rad2Deg;

        costheta4 = Mathf.Cos(theta4 * Mathf.Deg2Rad);

        sentheta4 = Mathf.Sin(theta4 * Mathf.Deg2Rad);


        //'Calculo theta5

        Y = (-r13 * ((costheta1 * costheta23 * costheta4) + (sentheta1 * sentheta4)) - r23 * ((sentheta1 * costheta23 * costheta4) - (costheta1 * sentheta4)) + r33 * (sentheta23 * costheta4));

        X = (-r13 * (costheta1 * sentheta23) - r23 * (sentheta1 * sentheta23) - r33 * (costheta23));


        theta5 = (Mathf.Atan2(Y, X)) * Mathf.Rad2Deg;

        costheta5 = Mathf.Cos(theta5 * Mathf.Deg2Rad);

        sentheta5 = Mathf.Sin(theta5 * Mathf.Deg2Rad);


        //'Calculo theta6

        Y2 = (-r11 * ((costheta1 * costheta23 * sentheta4) - (sentheta1 * costheta4)) - r21 * ((sentheta1 * costheta23 * sentheta4) + (costheta1 * costheta4)) + r31 * (sentheta23 * sentheta4));

        x61 = (costheta1 * costheta23 * costheta4 + sentheta1 * sentheta4) * costheta5 - (costheta1 * sentheta23 * sentheta5);

        x62 = (sentheta1 * costheta23 * costheta4 - costheta1 * sentheta4) * costheta5 - (sentheta1 * sentheta23 * sentheta5);

        x63 = sentheta23 * costheta4 * costheta5 + costheta23 * sentheta5;

        X2 = (r11 * x61) + (r21 * x62) - (r31 * x63);


        theta6 = (Mathf.Atan2(Y2, X2)) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        RobotMovement();
    }
}
