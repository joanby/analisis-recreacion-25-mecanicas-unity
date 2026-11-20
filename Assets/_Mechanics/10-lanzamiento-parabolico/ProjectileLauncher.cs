using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Lanzamiento parabólico con predicción de trayectoria al estilo Angry Birds:
    /// arrastre para apuntar, LineRenderer que simula la física paso a paso y
    /// disparo que reproduce exactamente esa curva.
    ///
    /// Referencia de la lección 04-impl-lanzamiento-parabolico. La predicción usa
    /// la MISMA gravedad y el MISMO dt que la simulación real, así que línea y
    /// vuelo coinciden.
    /// </summary>
    public class ProjectileLauncher : MonoBehaviour
    {
        [Header("Lanzamiento")]
        [SerializeField] private Rigidbody2D proyectilPrefab;
        [SerializeField] private Transform puntoLanzamiento;
        [SerializeField] private float fuerzaMax = 12f;

        [Header("Predicción")]
        [SerializeField] private LineRenderer linea;
        [SerializeField] private int puntosSimulados = 30;

        private Camera _cam;
        private Vector2 _origenArrastre;
        private bool _apuntando;

        private void Awake()
        {
            _cam = Camera.main;
            if (linea != null) linea.enabled = false;
        }

        private void Update()
        {
            var m = UnityEngine.InputSystem.Mouse.current;
            if (m == null) return;

            if (m.leftButton.wasPressedThisFrame)
            {
                _origenArrastre = MundoDesdeRaton(m);
                _apuntando = true;
                if (linea != null) linea.enabled = true;
            }
            else if (m.leftButton.isPressed && _apuntando)
            {
                Vector2 vel = CalcularVelocidad(m);
                DibujarTrayectoria(puntoLanzamiento.position, vel);
            }
            else if (m.leftButton.wasReleasedThisFrame && _apuntando)
            {
                Lanzar(CalcularVelocidad(m));
                _apuntando = false;
                if (linea != null) linea.enabled = false;
            }
        }

        private Vector2 CalcularVelocidad(UnityEngine.InputSystem.Mouse m)
        {
            // Vector "tira y afloja": desde la posición actual hacia el origen,
            // con magnitud proporcional a la distancia y limitada por fuerzaMax.
            Vector2 actual = MundoDesdeRaton(m);
            Vector2 dir = _origenArrastre - actual;
            return Vector2.ClampMagnitude(dir, fuerzaMax);
        }

        /// <summary>
        /// Reproduce a mano lo que hará el motor: integra posición y velocidad con
        /// el mismo dt fijo y la misma gravedad de Physics2D.
        /// </summary>
        private void DibujarTrayectoria(Vector2 posInicial, Vector2 velInicial)
        {
            if (linea == null) return;
            linea.positionCount = puntosSimulados;

            float dt = Time.fixedDeltaTime;
            Vector2 pos = posInicial;
            Vector2 vel = velInicial;

            for (int i = 0; i < puntosSimulados; i++)
            {
                linea.SetPosition(i, pos);
                vel += Physics2D.gravity * dt; // gravityScale = 1 en el prefab
                pos += vel * dt;
            }
        }

        private void Lanzar(Vector2 velocidad)
        {
            Rigidbody2D p = Instantiate(proyectilPrefab, puntoLanzamiento.position, Quaternion.identity);
            p.linearVelocity = velocidad; // misma velocidad inicial que la predicción
        }

        private Vector2 MundoDesdeRaton(UnityEngine.InputSystem.Mouse m) =>
            _cam.ScreenToWorldPoint(m.position.ReadValue());
    }
}
