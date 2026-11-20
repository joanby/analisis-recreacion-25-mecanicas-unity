using System.Collections.Generic;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Rebobinado del tiempo al estilo Braid: graba el estado físico en un buffer
    /// circular y, al pulsar, congela la simulación y reproduce los estados hacia
    /// atrás.
    ///
    /// Referencia de la lección 04-impl-rewind. La grabación y la reproducción van
    /// en FixedUpdate para casar con el paso de física (≈50 fps).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Rewind : MonoBehaviour
    {
        [Tooltip("Segundos de historia que se pueden rebobinar.")]
        [SerializeField] private float segundosMemoria = 5f;

        private struct PointInTime
        {
            public Vector2 posicion;
            public Vector2 velocidad;
            public float rotacion;
        }

        private readonly List<PointInTime> _historial = new();
        private Rigidbody2D _rb;
        private bool _rebobinando;
        private int _maxFrames;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            // N segundos * pasos de física por segundo.
            _maxFrames = Mathf.CeilToInt(segundosMemoria / Time.fixedDeltaTime);
        }

        private void Update()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb == null) return;
            if (kb.qKey.wasPressedThisFrame) IniciarRebobinado();
            if (kb.qKey.wasReleasedThisFrame) DetenerRebobinado();
        }

        private void FixedUpdate()
        {
            if (_rebobinando) Rebobinar();
            else Grabar();
        }

        private void Grabar()
        {
            // Buffer circular: descarta el más antiguo si nos pasamos.
            if (_historial.Count >= _maxFrames)
                _historial.RemoveAt(0);
            _historial.Add(new PointInTime
            {
                posicion = _rb.position,
                velocidad = _rb.linearVelocity,
                rotacion = _rb.rotation
            });
        }

        private void Rebobinar()
        {
            if (_historial.Count == 0) { DetenerRebobinado(); return; }
            int ultimo = _historial.Count - 1;
            PointInTime p = _historial[ultimo];
            _historial.RemoveAt(ultimo);
            _rb.position = p.posicion;
            _rb.rotation = p.rotacion;
        }

        private void IniciarRebobinado()
        {
            _rebobinando = true;
            // En Unity 6 el modo se controla con bodyType (no con isKinematic).
            _rb.bodyType = RigidbodyType2D.Kinematic; // congela la física durante el rebobinado
        }

        private void DetenerRebobinado()
        {
            _rebobinando = false;
            _rb.bodyType = RigidbodyType2D.Dynamic;
            // Restaura la velocidad del último estado leído para reanudar con inercia.
            if (_historial.Count > 0)
                _rb.linearVelocity = _historial[^1].velocidad;
        }

        private void OnDisable()
        {
            // Si el objeto se desactiva a mitad del rebobinado, nada más vuelve a
            // llamar a DetenerRebobinado(): sin esto, el Rigidbody2D se quedaría en
            // Kinematic (sin física) para siempre.
            if (_rebobinando) DetenerRebobinado();
        }
    }
}
