using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Doble salto con "perdones" al estilo Celeste: ground check por overlap,
    /// contador de saltos, coyote time y jump buffer.
    ///
    /// Referencia de la lección 02-impl-doble-salto. El objetivo no es un
    /// plataformas completo, sino sentir la diferencia entre un salto tosco y
    /// uno que perdona el timing del jugador.
    ///
    /// Escena mínima: un personaje-cápsula con Rigidbody2D (gravedad activa),
    /// un Transform hijo "groundCheck" a los pies y un suelo en la capa configurada.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerJump : MonoBehaviour
    {
        [Header("Salto")]
        [Tooltip("Impulso vertical aplicado a cada salto.")]
        [SerializeField] private float velocidadSalto = 12f;
        [Tooltip("Número máximo de saltos: 2 = salto + doble salto.")]
        [SerializeField] private int saltosMaximos = 2;

        [Header("Ground check")]
        [Tooltip("Transform hijo colocado a los pies del personaje.")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float radioGroundCheck = 0.15f;
        [Tooltip("Capa(s) que cuentan como suelo. NO se detecta por tag ni por colisión manual.")]
        [SerializeField] private LayerMask capaSuelo;

        [Header("Game feel")]
        [Tooltip("Ventana de gracia para saltar tras dejar el borde (s).")]
        [SerializeField] private float coyoteTime = 0.1f;
        [Tooltip("Cuánto tiempo se recuerda una pulsación hecha antes de tocar suelo (s).")]
        [SerializeField] private float jumpBuffer = 0.1f;

        private Rigidbody2D _rb;
        private int _saltosRestantes;
        private float _coyoteCounter;
        private float _bufferCounter;

        private void Awake() => _rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            // 1) Ground check por overlap (no por colisión manual ni por tag).
            bool enSuelo = Physics2D.OverlapCircle(groundCheck.position, radioGroundCheck, capaSuelo);

            // 2) Coyote time: se recarga en suelo y decae en el aire.
            if (enSuelo)
            {
                _coyoteCounter = coyoteTime;
                _saltosRestantes = saltosMaximos;
            }
            else
            {
                _coyoteCounter -= Time.deltaTime;
            }

            // 3) Jump buffer: recuerda la pulsación anticipada.
            //    Se lee con el Input System (tecla Espacio / botón sur del mando).
            if (SaltoPulsadoEsteFrame())
                _bufferCounter = jumpBuffer;
            else
                _bufferCounter -= Time.deltaTime;

            // 4) ¿Saltamos? Buffer activo + (coyote disponible O saltos aéreos restantes).
            bool puedeSaltar = _coyoteCounter > 0f || _saltosRestantes > 0;
            if (_bufferCounter > 0f && puedeSaltar)
                Saltar();
        }

        private void Saltar()
        {
            // Impulso limpio: fijamos la Y directamente (más predecible que AddForce
            // para un plataformas). En Unity 6 la propiedad es linearVelocity
            // (antes 'velocity', ahora obsoleta).
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, velocidadSalto);
            _saltosRestantes--;
            _bufferCounter = 0f; // consume el input para no re-disparar
            _coyoteCounter = 0f; // evita gastar el coyote en el mismo salto
        }

        /// <summary>
        /// Lectura mínima del salto con el nuevo Input System sin necesidad de
        /// un asset de Input Actions. En un proyecto real se cablearía una acción.
        /// </summary>
        private bool SaltoPulsadoEsteFrame()
        {
            var teclado = UnityEngine.InputSystem.Keyboard.current;
            var mando = UnityEngine.InputSystem.Gamepad.current;
            bool tecla = teclado != null && teclado.spaceKey.wasPressedThisFrame;
            bool boton = mando != null && mando.buttonSouth.wasPressedThisFrame;
            return tecla || boton;
        }

        // Gizmo del círculo de ground check para verlo en la escena.
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, radioGroundCheck);
        }
    }
}
