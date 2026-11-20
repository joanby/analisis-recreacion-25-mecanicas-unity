using System.Collections.Generic;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Generación procedural de mazmorra al estilo The Binding of Isaac: layout por
    /// rejilla con una semilla, colocación de salas según sus conexiones y elección
    /// de sala final por distancia (BFS).
    ///
    /// Referencia de la lección 06-impl-dungeon-procedural. TODO el azar sale de un
    /// único System.Random(seed): misma semilla = misma mazmorra (reproducible).
    /// </summary>
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private int seed = 12345;
        [SerializeField] private int numSalas = 10;
        [SerializeField] private float tamanoSala = 12f;
        [SerializeField] private GameObject prefabSala; // placeholder CC0

        private static readonly Vector2Int[] Direcciones =
            { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private readonly Dictionary<Vector2Int, GameObject> _salas = new();

        private void Start() => Generar();

        private void Update()
        {
            // Regenerar con nueva semilla para comprobar la variedad.
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb != null && kb.spaceKey.wasPressedThisFrame)
            {
                seed++;
                Generar();
            }
        }

        public void Generar()
        {
            Limpiar();
            var rng = new System.Random(seed); // única fuente de azar

            // 1) Layout: partir de (0,0) y avanzar a vecinos libres hasta numSalas.
            var ocupadas = new HashSet<Vector2Int> { Vector2Int.zero };
            var frontera = new List<Vector2Int> { Vector2Int.zero };

            while (ocupadas.Count < numSalas && frontera.Count > 0)
            {
                Vector2Int desde = frontera[rng.Next(frontera.Count)];
                Vector2Int dir = Direcciones[rng.Next(Direcciones.Length)];
                Vector2Int destino = desde + dir;

                if (ocupadas.Add(destino))
                    frontera.Add(destino);
                else if (rng.Next(4) == 0)
                    frontera.Remove(desde); // poda ocasional para no ramificar en exceso
            }

            // 2) Instanciar cada celda ocupada en world-space.
            foreach (Vector2Int celda in ocupadas)
            {
                Vector3 pos = new(celda.x * tamanoSala, 0f, celda.y * tamanoSala);
                _salas[celda] = Instantiate(prefabSala, pos, Quaternion.identity, transform);
            }

            // 3) Sala final = la más lejana de (0,0) por BFS.
            Vector2Int final = SalaMasLejana(Vector2Int.zero, ocupadas);
            Debug.Log($"Sala inicial (0,0), sala final {final}. Semilla {seed}.");
        }

        /// <summary>BFS sobre el grafo de salas adyacentes para la distancia máxima.</summary>
        private Vector2Int SalaMasLejana(Vector2Int origen, HashSet<Vector2Int> ocupadas)
        {
            var visitadas = new HashSet<Vector2Int> { origen };
            var cola = new Queue<Vector2Int>();
            cola.Enqueue(origen);
            Vector2Int lejana = origen;

            while (cola.Count > 0)
            {
                Vector2Int actual = cola.Dequeue();
                lejana = actual; // BFS visita por capas: la última es la más lejana
                foreach (Vector2Int dir in Direcciones)
                {
                    Vector2Int vecino = actual + dir;
                    if (ocupadas.Contains(vecino) && visitadas.Add(vecino))
                        cola.Enqueue(vecino);
                }
            }
            return lejana;
        }

        private void Limpiar()
        {
            foreach (var kv in _salas)
                if (kv.Value != null) Destroy(kv.Value);
            _salas.Clear();
        }
    }
}
