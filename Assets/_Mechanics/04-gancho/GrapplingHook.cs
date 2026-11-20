using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Gancho / grappling hook al estilo Zelda BotW: raycast para anclar,
    /// dos variantes de tracción (columpio con SpringJoint o tirón directo con
    /// AddForce) y un LineRenderer para dibujar la cuerda.
    ///
    /// Referencia de la lección 08-impl-gancho. Cambia 'modo' en el Inspector
    /// para comparar el balanceo físico (Columpio) con el tirón directo.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class GrapplingHook : MonoBehaviour
    {
        public enum Modo { Columpio, TironDirecto }

        [Header("Anclaje")]
        [SerializeField] private Transform origen;      // boca del gancho (normalmente la cámara/personaje)
        [SerializeField] private LayerMask capaAnclable;
        [SerializeField] private float rangoMax = 25f;

        [Header("Comportamiento")]
        [SerializeField] private Modo modo = Modo.Columpio;
        [SerializeField] private float fuerzaTiron = 30f;      // solo modo TironDirecto
        [SerializeField] private float springFuerza = 4.5f;    // solo modo Columpio
        [SerializeField] private float springDamper = 7f;
        [Tooltip("Empuje que el jugador puede añadir con WASD mientras se columpia (solo modo Columpio). No pisa el SpringJoint, se suma a él.")]
        [SerializeField] private float fuerzaBalanceo = 15f;

        [Header("Visual")]
        [SerializeField] private LineRenderer cuerda;

        [Header("Movimiento básico (no es la mecánica de esta lección)")]
        [Tooltip("Sin esto el personaje no puede acercarse a un anclaje por su cuenta. El Rigidbody ya trae su propia gravedad, solo falta X/Z.")]
        [SerializeField] private float velocidadMovimiento = 6f;

        private Rigidbody _rb;
        private SpringJoint _joint;
        private Vector3 _puntoAnclaje;
        private bool _enganchado;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            // Sin esto, el SpringJoint (columpio) o el AddForce (tirón) generan
            // torque y la cápsula acaba girando/tumbándose. Rotamos solo por
            // script si hace falta, nunca por física.
            _rb.constraints = RigidbodyConstraints.FreezeRotation;

            if (cuerda != null) cuerda.enabled = false;
        }

        private void Update()
        {
            if (BotonPulsado()) Lanzar();
            if (BotonSoltado()) Soltar();

            if (_enganchado && cuerda != null)
            {
                cuerda.SetPosition(0, origen.position);
                cuerda.SetPosition(1, _puntoAnclaje);
            }

            // Fuera del gancho, movimiento horizontal normal para poder llegar
            // hasta un anclaje. El Rigidbody ya trae su propia gravedad (Y);
            // mientras estás enganchado, no tocamos la velocidad: la gobiernan
            // el SpringJoint o el AddForce.
            if (!_enganchado)
            {
                Vector2 input = LeerInputMovimiento();
                Vector3 mover = (transform.right * input.x + transform.forward * input.y) * velocidadMovimiento;
                _rb.linearVelocity = new Vector3(mover.x, _rb.linearVelocity.y, mover.z);
            }
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

        private void FixedUpdate()
        {
            if (!_enganchado) return;

            if (modo == Modo.TironDirecto)
            {
                // El tirón directo aplica fuerza cada paso de física hacia el anclaje.
                Vector3 haciaAnclaje = (_puntoAnclaje - transform.position).normalized;
                _rb.AddForce(haciaAnclaje * fuerzaTiron, ForceMode.Acceleration);
            }
            else // Columpio
            {
                // El SpringJoint ya gobierna la tensión de la cuerda; esto solo
                // añade un empujón lateral con WASD para poder "jugar con el
                // momento" en vez de ser un péndulo puramente pasivo.
                Vector2 input = LeerInputMovimiento();
                if (input.sqrMagnitude > 0.01f)
                {
                    Vector3 empuje = (transform.right * input.x + transform.forward * input.y) * fuerzaBalanceo;
                    _rb.AddForce(empuje, ForceMode.Acceleration);
                }
            }
        }

        private void Lanzar()
        {
            if (!Physics.Raycast(origen.position, origen.forward, out RaycastHit hit, rangoMax, capaAnclable))
                return;

            _puntoAnclaje = hit.point;
            _enganchado = true;

            if (modo == Modo.Columpio)
            {
                // SpringJoint => balanceo físico realista.
                _joint = gameObject.AddComponent<SpringJoint>();
                _joint.autoConfigureConnectedAnchor = false;
                _joint.connectedAnchor = _puntoAnclaje;
                float distancia = Vector3.Distance(transform.position, _puntoAnclaje);
                _joint.maxDistance = distancia * 0.8f;
                _joint.minDistance = distancia * 0.25f;
                _joint.spring = springFuerza;
                _joint.damper = springDamper;
                _joint.massScale = 4.5f;
            }

            if (cuerda != null) { cuerda.enabled = true; cuerda.positionCount = 2; }
        }

        private void Soltar()
        {
            _enganchado = false;
            if (_joint != null) Destroy(_joint);
            if (cuerda != null) cuerda.enabled = false;
        }

        private bool BotonPulsado()
        {
            var m = UnityEngine.InputSystem.Mouse.current;
            return m != null && m.leftButton.wasPressedThisFrame;
        }

        private bool BotonSoltado()
        {
            var m = UnityEngine.InputSystem.Mouse.current;
            return m != null && m.leftButton.wasReleasedThisFrame;
        }
    }
}
