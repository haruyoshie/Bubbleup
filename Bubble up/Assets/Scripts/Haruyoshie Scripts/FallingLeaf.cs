using UnityEngine;

public class FallingLeaf : MonoBehaviour
{
    public float fallSpeed = 2f; // Velocidad de caída vertical
    public float oscillationSpeed = 2f; // Velocidad de oscilación horizontal
    public float oscillationWidth = 2f; // Amplitud de la oscilación horizontal

    private float oscillationOffset; // Desfase para diferenciar el movimiento entre hojas

    void Start()
    {
        // Asigna un desfase aleatorio para evitar sincronización entre objetos
        oscillationOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        // Movimiento hacia abajo
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Movimiento oscilatorio en el eje X
        float xOscillation = Mathf.Sin(Time.time * oscillationSpeed + oscillationOffset) * oscillationWidth;
        transform.position += new Vector3(xOscillation * Time.deltaTime, 0, 0);
    }
}