using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Wall-run al estilo Titanfall: detección de pared por raycasts laterales,
    /// proyección de la velocidad sobre la pared, temporizador de duración y
    /// salto de pared combinando la normal con un componente vertical.
    ///
    /// Referencia de la lección 04-impl-wall-run. Usa CharacterController (3D),
    /// que no está gobernado por la gravedad del motor: la aplicamos a mano y la
    /// "apagamos" mientras corremos por la pared.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class WallRun : MonoBehaviour
    {
        [Header("Detección de pared")]
        [SerializeField] private LayerMask capaPared;
        [SerializeField] private float distanciaCheck = 0.7f;

        [Header("Wall-run")]
        [SerializeField] private float velocidadCarrera = 8f;
        [Tooltip("Duración máxima de una carrera de pared antes de caer (s).")]
        [SerializeField] private float duracionMax = 1.5f;
        [SerializeField] private float gravedad = -20f;

        [Header("Salto de pared")]
        [SerializeField] private float empujeLateral = 8f;
        [SerializeField] private float empujeVertical = 7f;
        [Tooltip("Ventana tras salir de la pared (por salto o por timeout) en la que no te puedes re-enganchar ni el WASD pisa el impulso de salida.")]
        [SerializeField] private float cooldownSalida = 0.25f;

        [Header("Movimiento básico (no es la mecánica de esta lección)")]
        [Tooltip("Sin esto el personaje no puede ni acercarse a una pared. WASD relativo a hacia dónde mira.")]
        [SerializeField] private float velocidadMovimiento = 6f;
        [Tooltip("Salto normal desde el suelo. Sin él nunca te separas del suelo para lanzarte a una pared.")]
        [SerializeField] private float velocidadSaltoSuelo = 8f;

        private CharacterController _cc;
        private Vector3 _velocidad;
        private bool _corriendoPared;
        private float _timer;
        private Vector3 _normalPared;
        private float _cooldown;

        private void Awake() => _cc = GetComponent<CharacterController>();

        private void Update()
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;

            // Raycast a izquierda y derecha del personaje contra la capa "pared".
            bool paredDcha = Physics.Raycast(transform.position, transform.right, out RaycastHit hitDcha, distanciaCheck, capaPared);
            bool paredIzda = Physics.Raycast(transform.position, -transform.right, out RaycastHit hitIzda, distanciaCheck, capaPared);
            bool hayPared = paredDcha || paredIzda;

            // Entramos en wall-run si tocamos pared, no estamos en el suelo, avanzamos
            // y no acabamos de salir de una (si no, te re-enganchas en el frame
            // siguiente al timeout o al salto, y nunca llegas a soltarte de verdad).
            bool avanzando = InputAvance() > 0.1f;
            if (hayPared && !_cc.isGrounded && avanzando && !_corriendoPared && _cooldown <= 0f)
                EntrarWallRun(paredDcha ? hitDcha.normal : hitIzda.normal);

            if (_corriendoPared)
            {
                _timer -= Time.deltaTime;
                // isGrounded también saca del wall-run: sin esto, si rozas una
                // pared en el aire y aterrizas pegado a ella, el estado se queda
                // "atascado" y el siguiente Space dispara el salto de PARED
                // (sobre todo lateral) en vez del salto normal desde el suelo.
                if (!hayPared || _timer <= 0f || _cc.isGrounded)
                    SalirWallRun();
                else if (SaltoPulsado())
                    SaltarDesdePared();
            }

            AplicarMovimiento();
        }

        private void EntrarWallRun(Vector3 normal)
        {
            _corriendoPared = true;
            _timer = duracionMax;
            _normalPared = normal;
            _velocidad.y = 0f; // "apagamos" la gravedad acumulada
        }

        private void SalirWallRun()
        {
            _corriendoPared = false;
            _cooldown = cooldownSalida; // misma salida por timeout o por salto
        }

        private void SaltarDesdePared()
        {
            // Impulso hacia fuera de la pared + componente vertical.
            _velocidad = _normalPared * empujeLateral + Vector3.up * empujeVertical;
            SalirWallRun();
        }

        private void AplicarMovimiento()
        {
            if (_corriendoPared)
            {
                // Dirección de carrera = producto vectorial entre la normal y el eje
                // vertical. Elegimos el sentido que apunta "hacia delante".
                Vector3 dir = Vector3.Cross(_normalPared, Vector3.up);
                if (Vector3.Dot(dir, transform.forward) < 0f) dir = -dir;
                _velocidad = dir * velocidadCarrera;
            }
            else
            {
                // Fuera de la pared: movimiento básico WASD relativo al personaje.
                // Durante el cooldown de salida no tocamos X/Z para nada (ni con
                // input ni sin él): así el impulso del salto de pared te separa
                // de verdad antes de que el WASD lo cancele. Terminado el
                // cooldown, X/Z sigue al input SIEMPRE —incluido sin input, que
                // debe frenar a cero; si no, el personaje no se para nunca.
                if (_cooldown <= 0f)
                {
                    Vector2 input = LeerInputMovimiento();
                    Vector3 mover = (transform.right * input.x + transform.forward * input.y) * velocidadMovimiento;
                    _velocidad.x = mover.x;
                    _velocidad.z = mover.z;
                }

                if (_cc.isGrounded)
                {
                    // Valor pequeño, no cero: mantiene el CharacterController pegado
                    // al suelo y a isGrounded fiable (con 0f, la física interna del
                    // CC hace que "flote" y a veces se lea como no-grounded).
                    if (_velocidad.y < 0f) _velocidad.y = -2f;

                    // Salto normal: sin esto nunca te separas del suelo para
                    // lanzarte a una pared en el aire.
                    if (SaltoPulsado())
                        _velocidad.y = velocidadSaltoSuelo;
                }
                else
                {
                    _velocidad.y += gravedad * Time.deltaTime;
                }
            }

            _cc.Move(_velocidad * Time.deltaTime);
        }

        private Vector2 LeerInputMovimiento()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb == null) return Vector2.zero;

            float x = 0f;
            if (kb.aKey.isPressed) x -= 1f;
            if (kb.dKey.isPressed) x += 1f;

            float y = 0f;
            if (kb.sKey.isPressed) y -= 1f;
            if (kb.wKey.isPressed) y += 1f;

            return new Vector2(x, y);
        }

        private float InputAvance() => LeerInputMovimiento().magnitude;

        private bool SaltoPulsado()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            return kb != null && kb.spaceKey.wasPressedThisFrame;
        }
    }
}
