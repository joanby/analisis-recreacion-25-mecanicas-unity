using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Crafteo por recetas al estilo Minecraft (banco sin forma): ítems y recetas
    /// como ScriptableObject, y una clave de combinación normalizada que permite
    /// buscar la receta en un diccionario en O(1).
    ///
    /// Referencia de la lección 06-impl-crafting. Ampliación: para recetas CON
    /// forma, se guardaría la disposición 2D en vez de un conjunto ordenado.
    /// </summary>
    public class CraftingSystem : MonoBehaviour
    {
        [SerializeField] private List<Recipe> recetas = new();

        // clave normalizada -> ítem resultante
        private readonly Dictionary<string, CraftItem> _porClave = new();

        private void Awake()
        {
            // Indexa todas las recetas por su clave normalizada.
            foreach (Recipe r in recetas)
                _porClave[Clave(r.ingredientes)] = r.resultado;
        }

        /// <summary>
        /// Clave estable: ordena los ids de los ingredientes y los une. Así
        /// "palo+piedra" y "piedra+palo" producen la misma clave.
        /// </summary>
        public static string Clave(IEnumerable<CraftItem> items) =>
            string.Join("+", items.Where(i => i != null).Select(i => i.id).OrderBy(x => x));

        /// <summary>
        /// Intenta craftear con los ingredientes de las ranuras. Devuelve el
        /// resultado o null si no hay receta.
        /// </summary>
        public CraftItem Craftear(IEnumerable<CraftItem> ranuras)
        {
            if (_porClave.TryGetValue(Clave(ranuras), out CraftItem resultado))
            {
                // Aquí se consumirían los ingredientes de las ranuras y se
                // añadiría 'resultado' al inventario.
                return resultado;
            }
            return null; // combinación inválida: no pasa nada
        }
    }

    [CreateAssetMenu(menuName = "Frogames/Crafting/Item")]
    public class CraftItem : ScriptableObject
    {
        public string id;
        public string nombre;
        public Sprite sprite;
    }

    [CreateAssetMenu(menuName = "Frogames/Crafting/Recipe")]
    public class Recipe : ScriptableObject
    {
        public List<CraftItem> ingredientes = new();
        public CraftItem resultado;
    }
}
