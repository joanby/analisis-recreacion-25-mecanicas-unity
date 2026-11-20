using System.Collections.Generic;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Empuje de bloques en rejilla con deshacer, al estilo Sokoban / Baba Is You:
    /// modelo discreto por diccionario, detección de cadenas de empuje y una pila
    /// de undo con copias del estado.
    ///
    /// Referencia de la lección 02-impl-sokoban. El modelo es discreto (Vector2Int);
    /// el movimiento visual se interpola aparte para que se vea suave sin romperlo.
    /// </summary>
    public class GridController : MonoBehaviour
    {
        [SerializeField] private List<Transform> bloques = new();
        [SerializeField] private List<Vector2Int> muros = new();
        [SerializeField] private Transform jugadorVisual;
        [SerializeField] private float velocidadInterpolacion = 10f;

        // Modelo: qué bloque hay en cada casilla.
        private readonly Dictionary<Vector2Int, Transform> _ocupacion = new();
        private readonly HashSet<Vector2Int> _muros = new();
        private readonly Stack<EstadoTablero> _undoStack = new();
        private Vector2Int _posJugador;

        private void Awake()
        {
            foreach (Vector2Int m in muros) _muros.Add(m);
            foreach (Transform b in bloques)
                _ocupacion[ARejilla(b.position)] = b;
            _posJugador = ARejilla(jugadorVisual.position);
        }

        private void Update()
        {
            Vector2Int dir = LeerDireccion();
            if (dir != Vector2Int.zero) IntentarMover(dir);

            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb != null && kb.zKey.wasPressedThisFrame) Deshacer();

            // Interpolación visual hacia la casilla del modelo.
            jugadorVisual.position = Vector3.MoveTowards(jugadorVisual.position,
                AMundo(_posJugador), velocidadInterpolacion * Time.deltaTime);
        }

        /// <summary>Devuelve si el movimiento fue posible; si lo es, registra undo.</summary>
        public bool IntentarMover(Vector2Int dir)
        {
            Vector2Int destino = _posJugador + dir;
            if (_muros.Contains(destino)) return false;

            // Cadena de empuje: acumular bloques consecutivos en la dirección.
            var cadena = new List<Vector2Int>();
            Vector2Int cursor = destino;
            while (_ocupacion.ContainsKey(cursor))
            {
                cadena.Add(cursor);
                cursor += dir;
            }
            // La casilla tras el último bloque debe estar libre y sin muro.
            if (_muros.Contains(cursor) || _ocupacion.ContainsKey(cursor)) return false;

            GuardarUndo();

            // Empujar la cadena de atrás hacia delante para no pisarse.
            for (int i = cadena.Count - 1; i >= 0; i--)
            {
                Vector2Int from = cadena[i];
                Vector2Int to = from + dir;
                Transform bloque = _ocupacion[from];
                _ocupacion.Remove(from);
                _ocupacion[to] = bloque;
                bloque.position = AMundo(to);
            }
            _posJugador = destino;
            return true;
        }

        private void GuardarUndo()
        {
            var snap = new EstadoTablero
            {
                jugador = _posJugador,
                bloques = new Dictionary<Vector2Int, Transform>(_ocupacion)
            };
            _undoStack.Push(snap);
        }

        private void Deshacer()
        {
            if (_undoStack.Count == 0) return;
            EstadoTablero prev = _undoStack.Pop();
            _posJugador = prev.jugador;
            _ocupacion.Clear();
            foreach (var kv in prev.bloques)
            {
                _ocupacion[kv.Key] = kv.Value;
                kv.Value.position = AMundo(kv.Key);
            }
        }

        private Vector2Int LeerDireccion()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb == null) return Vector2Int.zero;
            if (kb.upArrowKey.wasPressedThisFrame) return Vector2Int.up;
            if (kb.downArrowKey.wasPressedThisFrame) return Vector2Int.down;
            if (kb.leftArrowKey.wasPressedThisFrame) return Vector2Int.left;
            if (kb.rightArrowKey.wasPressedThisFrame) return Vector2Int.right;
            return Vector2Int.zero;
        }

        private static Vector2Int ARejilla(Vector3 p) =>
            new(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
        private static Vector3 AMundo(Vector2Int c) => new(c.x, c.y, 0f);

        private struct EstadoTablero
        {
            public Vector2Int jugador;
            public Dictionary<Vector2Int, Transform> bloques;
        }
    }
}
