using UnityEngine;
using UnityEngine.AI;

namespace Frogames.Mechanics
{
    /// <summary>
    /// IA de guardia con FSM y NavMesh al estilo Commandos: patrulla, sospecha,
    /// alerta y búsqueda. Un switch sobre el estado actúa y decide transiciones.
    ///
    /// Referencia de la lección 04-impl-fsm-alerta-navmesh. Clave de diseño: la FSM
    /// NUNCA calcula si ve al jugador; solo consulta la señal booleana del
    /// FieldOfView, y ultimoPuntoConocido solo se refresca mientras esa señal es true.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class GuardAI : MonoBehaviour
    {
        public enum EstadoGuardia { Patrulla, Sospecha, Alerta, Busqueda }

        [Header("Referencias")]
        [SerializeField] private FieldOfView fov;      // provee la señal de visión
        [SerializeField] private Transform jugador;
        [SerializeField] private Transform[] waypoints;

        [Header("Tiempos (s)")]
        [SerializeField] private float tiempoSospecha = 2f;
        [SerializeField] private float tiempoBusqueda = 4f;

        private NavMeshAgent _agent;
        private EstadoGuardia _estado = EstadoGuardia.Patrulla;
        private Vector3 _ultimoPuntoConocido;
        private int _waypointActual;
        private float _timer;

        private void Awake() => _agent = GetComponent<NavMeshAgent>();

        private void Update()
        {
            bool visible = fov.JugadorVisible;
            if (visible) _ultimoPuntoConocido = jugador.position; // solo se refresca al ver

            // Cada case actúa (SetDestination) y evalúa sus transiciones.
            switch (_estado)
            {
                case EstadoGuardia.Patrulla:
                    Patrullar();
                    if (visible) Transicion(EstadoGuardia.Sospecha);
                    break;

                case EstadoGuardia.Sospecha:
                    _agent.ResetPath(); // detenerse y observar
                    _timer -= Time.deltaTime;
                    if (visible) Transicion(EstadoGuardia.Alerta);
                    else if (_timer <= 0f) Transicion(EstadoGuardia.Patrulla);
                    break;

                case EstadoGuardia.Alerta:
                    _agent.SetDestination(_ultimoPuntoConocido);
                    if (!visible) Transicion(EstadoGuardia.Busqueda);
                    break;

                case EstadoGuardia.Busqueda:
                    _agent.SetDestination(_ultimoPuntoConocido);
                    _timer -= Time.deltaTime;
                    if (visible) Transicion(EstadoGuardia.Alerta);
                    else if (_timer <= 0f) Transicion(EstadoGuardia.Patrulla);
                    break;
            }
        }

        private void Transicion(EstadoGuardia nuevo)
        {
            _estado = nuevo;
            if (nuevo == EstadoGuardia.Sospecha) _timer = tiempoSospecha;
            if (nuevo == EstadoGuardia.Busqueda) _timer = tiempoBusqueda;
        }

        private void Patrullar()
        {
            if (waypoints.Length == 0) return;
            if (!_agent.pathPending && _agent.remainingDistance < 0.3f)
                _waypointActual = (_waypointActual + 1) % waypoints.Length;
            _agent.SetDestination(waypoints[_waypointActual].position);
        }
    }
}
