using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Parry por ventana temporal al estilo Sekiro: una pequeña FSM de combate
    /// (Idle → Parrying → Recovering) y una resolución de impacto que bifurca
    /// en deflect / bloqueo / golpe según el timestamp del parry.
    ///
    /// Referencia de la lección 02-impl-parry. Todos los tiempos son campos
    /// serializados: ningún número mágico repartido por el código.
    /// Este componente vive en el Player (lleva el Rigidbody y el movimiento).
    /// El collider que de verdad recibe el golpe puede ser un hijo —el Escudo—
    /// en vez del propio Player: OnTriggerEnter no burbujea a los padres, así
    /// que si el impacto ocurre en un hijo, ese hijo necesita su propio
    /// ShieldHitReceiver que llame a RecibirImpacto() de este componente.
    /// El arma enemiga (ver EnemyAttack) es un trigger con tag EnemyWeapon.
    /// Un Deflect avisa al EnemyAttack del atacante para que rompa su postura
    /// (cambio de material + pausa del ataque).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ParrySystem : MonoBehaviour
    {
        public enum CombatState { Idle, Parrying, Recovering }
        public enum HitResult { Deflect, Block, Hit }

        [Header("Ventanas (s)")]
        [Tooltip("Margen tras iniciar el parry en el que un golpe se desvía.")]
        [SerializeField] private float parryWindow = 0.2f;
        [Tooltip("Recuperación tras un parry antes de volver a Idle.")]
        [SerializeField] private float recoverTime = 0.3f;

        [Header("Movimiento básico (no es la mecánica de esta lección)")]
        [Tooltip("Sin esto no puedes colocarte cerca del enemigo. El Rigidbody ya trae su propia gravedad, solo falta X/Z.")]
        [SerializeField] private float velocidadMovimiento = 6f;

        private Rigidbody _rb;
        private CombatState _estado = CombatState.Idle;
        private float _parryStartTime;
        private float _recoverStartTime;

        public CombatState Estado => _estado;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            // Un Rigidbody sin congelar rotación acaba tumbando la cápsula al
            // primer roce con el arma enemiga, igual que en el gancho.
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void Update()
        {
            // Movimiento libre en todo momento: el parry es puramente reactivo
            // (no fija velocidad ni te inmoviliza), así que no hace falta
            // gatear esto contra ningún estado de la FSM.
            Vector2 input = LeerInputMovimiento();
            Vector3 mover = (transform.right * input.x + transform.forward * input.y) * velocidadMovimiento;
            _rb.linearVelocity = new Vector3(mover.x, _rb.linearVelocity.y, mover.z);

            // Solo Idle acepta iniciar un parry.
            if (_estado == CombatState.Idle && ParryPulsado())
                IniciarParry();

            // La ventana de parry expira sola si no llega ningún golpe (o si llegó
            // tarde y solo produjo un Block, que tampoco cambia de estado por sí
            // mismo): sin esto, _estado se quedaría "pegado" en Parrying para
            // siempre y IniciarParry() nunca volvería a aceptar otra pulsación.
            if (_estado == CombatState.Parrying && Time.time - _parryStartTime > parryWindow)
            {
                _estado = CombatState.Recovering;
                _recoverStartTime = Time.time;
            }

            // La recuperación devuelve el control tras recoverTime.
            if (_estado == CombatState.Recovering && Time.time - _recoverStartTime >= recoverTime)
                _estado = CombatState.Idle;
        }

        private void IniciarParry()
        {
            _estado = CombatState.Parrying;
            _parryStartTime = Time.time;
        }

        // El arma enemiga (collider en modo trigger) golpea al defensor.
        // Funciona si el collider golpeado está en este mismo GameObject.
        private void OnTriggerEnter(Collider armaEnemiga) => RecibirImpacto(armaEnemiga);

        /// <summary>
        /// Punto de entrada público para cuando el collider golpeado está en un
        /// hijo (el Escudo) en vez de en este GameObject: ese hijo llama aquí
        /// desde su propio OnTriggerEnter, porque Unity no burbujea el evento
        /// a los padres por sí solo.
        /// </summary>
        public void RecibirImpacto(Collider armaEnemiga)
        {
            if (!armaEnemiga.CompareTag("EnemyWeapon")) return;
            HitResult resultado = ResolveHit();
            AplicarResultado(resultado, armaEnemiga);
        }

        /// <summary>
        /// Núcleo conceptual de la mecánica: lee el estado de la FSM, calcula el
        /// delta desde el inicio del parry y decide entre desvío, bloqueo o golpe.
        /// </summary>
        private HitResult ResolveHit()
        {
            if (_estado != CombatState.Parrying)
                return HitResult.Hit; // no estabas parando: recibes daño

            float delta = Time.time - _parryStartTime;
            if (delta <= parryWindow)
                return HitResult.Deflect;   // timing perfecto: desvío + castigo a la postura enemiga

            return HitResult.Block;         // tarde: bloqueas pero sin premio
        }

        private void AplicarResultado(HitResult resultado, Collider arma)
        {
            switch (resultado)
            {
                case HitResult.Deflect:
                    Debug.Log("¡Deflect! Postura enemiga rota.");
                    // El arma es hija del enemigo con EnemyAttack: le avisamos
                    // para que cambie de material y pause su ataque un rato.
                    arma.GetComponentInParent<EnemyAttack>()?.Aturdir();
                    _estado = CombatState.Recovering;
                    _recoverStartTime = Time.time;
                    break;
                case HitResult.Block:
                    Debug.Log("Bloqueo tardío: sin daño pero sin premio.");
                    break;
                case HitResult.Hit:
                    Debug.Log("Golpe recibido: aplica daño.");
                    break;
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

        private bool ParryPulsado()
        {
            var m = UnityEngine.InputSystem.Mouse.current;
            return m != null && m.rightButton.wasPressedThisFrame;
        }
    }
}
