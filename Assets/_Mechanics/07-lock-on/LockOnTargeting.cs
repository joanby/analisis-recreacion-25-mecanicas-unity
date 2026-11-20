using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine; // Cinemachine 3.x que acompaña a Unity 6

namespace Frogames.Mechanics
{
    /// <summary>
    /// Lock-on targeting al estilo Dark Souls: detecta candidatos con
    /// Physics.OverlapSphere, selecciona por menor ángulo respecto a la mirada de
    /// la cámara y encuadra al objetivo con un CinemachineTargetGroup.
    ///
    /// Referencia de la lección 06-impl-lock-on. Una segunda pulsación con lock
    /// activo salta al siguiente candidato por ángulo.
    ///
    /// El movimiento no es la mecánica de esta lección, pero sin él no hay
    /// forma de acercarse a los enemigos ni de ver el strafe orbital que la
    /// lección promete: sin lock, WASD normal; con lock, el actor mira siempre
    /// al objetivo y el input lateral orbita alrededor de él.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class LockOnTargeting : MonoBehaviour
    {
        [Header("Detección")]
        [SerializeField] private float lockRange = 15f;
        [SerializeField] private LayerMask enemyMask;
        [Tooltip("Ángulo máximo respecto a la mirada de la cámara para fijar un objetivo.")]
        [SerializeField] private float maxLockAngle = 60f;

        [Header("Cámara")]
        [SerializeField] private Transform camara;
        [Tooltip("TargetGroup que la cámara de combate sigue/encuadra.")]
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private float pesoJugador = 1f;
        [SerializeField] private float pesoObjetivo = 1f;

        [Header("Movimiento básico (no es la mecánica de esta lección)")]
        [SerializeField] private float velocidadMovimiento = 6f;
        [Tooltip("Grados/segundo al girar hacia el objetivo con lock activo.")]
        [SerializeField] private float velocidadGiro = 720f;

        private Rigidbody _rb;
        private Transform _objetivo;
        private bool _isLocked;
        private readonly List<Transform> _candidatosOrdenados = new();

        public Transform Objetivo => _objetivo;
        public bool IsLocked => _isLocked;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeRotation; // giramos por script, no por física

            // Si no se arrastró nada al campo "camara" en el Inspector, la
            // leemos sola: Camera.main es la cámara física que CinemachineBrain
            // mueve cada frame, así que su forward ya refleja hacia dónde
            // mira de verdad el jugador, con o sin lock.
            if (camara == null && Camera.main != null)
                camara = Camera.main.transform;
        }

        private void Start()
        {
            // Sin esto, targetGroup empieza vacío y la cámara no tiene nada
            // que seguir hasta el primer lock/unlock. SoltarLock() ya hace
            // justo lo que hace falta: deja el grupo con el jugador solo.
            SoltarLock();
        }

        private void Update()
        {
            // Q es un toggle real: sin lock, fija; con lock, suelta. Cambiar de
            // candidato es una tecla aparte (E), igual que en el juego de
            // referencia el stick de cámara cicla objetivos y el propio botón
            // de lock solo activa/desactiva.
            if (LockPulsado())
            {
                if (!_isLocked) AcquireTarget();
                else SoltarLock();
            }
            else if (_isLocked && CambiarCandidatoPulsado())
            {
                SiguienteCandidato();
            }

            // El objetivo puede morir o alejarse: soltamos el lock si se invalida.
            if (_isLocked && (_objetivo == null || FueraDeRango(_objetivo)))
                SoltarLock();

            MoverJugador();
        }

        /// <summary>
        /// Sin lock: WASD normal relativo al propio jugador. Con lock: el
        /// jugador siempre mira al objetivo y el input lateral se convierte en
        /// strafe orbital alrededor de él — el corazón visual de la mecánica.
        /// </summary>
        private void MoverJugador()
        {
            Vector2 input = LeerInputMovimiento();
            Vector3 mover;

            if (_isLocked && _objetivo != null)
            {
                Vector3 haciaObjetivo = _objetivo.position - transform.position;
                haciaObjetivo.y = 0f;
                haciaObjetivo.Normalize();

                Quaternion mirarObjetivo = Quaternion.LookRotation(haciaObjetivo);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, mirarObjetivo, velocidadGiro * Time.deltaTime);

                Vector3 lateral = Vector3.Cross(Vector3.up, haciaObjetivo); // eje de strafe, perpendicular al objetivo
                mover = (lateral * input.x + haciaObjetivo * input.y) * velocidadMovimiento;
            }
            else
            {
                mover = (transform.right * input.x + transform.forward * input.y) * velocidadMovimiento;
            }

            _rb.linearVelocity = new Vector3(mover.x, _rb.linearVelocity.y, mover.z);
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

        /// <summary>
        /// Núcleo conceptual: OverlapSphere para candidatos, filtra por ángulo con
        /// la mirada de la cámara y elige el de menor ángulo bajo el umbral.
        /// </summary>
        private void AcquireTarget()
        {
            _candidatosOrdenados.Clear();
            Collider[] hits = Physics.OverlapSphere(transform.position, lockRange, enemyMask);

            float mejorAngulo = maxLockAngle;
            Transform mejor = null;

            foreach (Collider hit in hits)
            {
                Vector3 dir = hit.transform.position - transform.position;
                float angulo = Vector3.Angle(camara.forward, dir);
                if (angulo > maxLockAngle) continue;

                _candidatosOrdenados.Add(hit.transform);
                if (angulo < mejorAngulo)
                {
                    mejorAngulo = angulo;
                    mejor = hit.transform;
                }
            }

            // Ordenamos para poder ciclar con la siguiente pulsación.
            _candidatosOrdenados.Sort((a, b) =>
                Vector3.Angle(camara.forward, a.position - transform.position)
                .CompareTo(Vector3.Angle(camara.forward, b.position - transform.position)));

            if (mejor != null) FijarObjetivo(mejor);
        }

        private void SiguienteCandidato()
        {
            if (_candidatosOrdenados.Count == 0) { SoltarLock(); return; }
            int idx = _candidatosOrdenados.IndexOf(_objetivo);
            int siguiente = (idx + 1) % _candidatosOrdenados.Count;
            FijarObjetivo(_candidatosOrdenados[siguiente]);
        }

        private void FijarObjetivo(Transform objetivo)
        {
            _objetivo = objetivo;
            _isLocked = true;

            targetGroup.Targets.Clear();
            targetGroup.AddMember(transform, pesoJugador, 1f);
            targetGroup.AddMember(_objetivo, pesoObjetivo, 1.5f);
        }

        private void SoltarLock()
        {
            _isLocked = false;
            _objetivo = null;
            targetGroup.Targets.Clear();
            targetGroup.AddMember(transform, pesoJugador, 1f);
        }

        private bool FueraDeRango(Transform t) =>
            Vector3.Distance(transform.position, t.position) > lockRange * 1.2f;

        private bool LockPulsado()
        {
            // Tecla Q: no todos los ratones tienen botón central, y es la
            // convención habitual en PC para fijar objetivo en este género.
            var kb = UnityEngine.InputSystem.Keyboard.current;
            return kb != null && kb.qKey.wasPressedThisFrame;
        }

        private bool CambiarCandidatoPulsado()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            return kb != null && kb.eKey.wasPressedThisFrame;
        }
    }
}
