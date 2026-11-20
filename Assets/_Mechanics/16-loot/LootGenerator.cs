using System.Collections.Generic;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Generación de loot por rareza y afijos al estilo Diablo: tirada ponderada
    /// para elegir la rareza, y afijos aleatorios dentro de rangos según ella.
    ///
    /// Referencia de la lección 08-impl-loot-tiers. Nota de diseño responsable:
    /// podría añadirse un "pity timer" que garantice rareza alta tras N drops sin ella.
    /// </summary>
    public class LootGenerator : MonoBehaviour
    {
        [SerializeField] private List<Rarity> rarezas = new();
        [SerializeField] private List<Affix> afijosPosibles = new();

        private int _pesoTotal;

        private void Awake()
        {
            foreach (Rarity r in rarezas) _pesoTotal += r.peso;
        }

        /// <summary>Dispara el flujo completo: rareza + afijos.</summary>
        public LootItem GenerarItem()
        {
            Rarity rareza = TirarRareza();
            var afijos = new List<AffixRoll>();
            var disponibles = new List<Affix>(afijosPosibles);

            int n = Mathf.Min(rareza.numAfijos, disponibles.Count);
            for (int i = 0; i < n; i++)
            {
                int idx = Random.Range(0, disponibles.Count);
                Affix a = disponibles[idx];
                disponibles.RemoveAt(idx); // sin afijos repetidos
                afijos.Add(new AffixRoll { nombre = a.nombre, valor = Random.Range(a.min, a.max) });
            }

            return new LootItem { rareza = rareza, afijos = afijos };
        }

        /// <summary>
        /// Tirada ponderada: un número en [0, pesoTotal) recorre las rarezas
        /// restando pesos hasta cruzar el umbral. Los legendarios (peso muy bajo)
        /// salen rara vez.
        /// </summary>
        private Rarity TirarRareza()
        {
            int r = Random.Range(0, _pesoTotal);
            foreach (Rarity rar in rarezas)
            {
                if (r < rar.peso) return rar;
                r -= rar.peso;
            }
            return rarezas[0]; // fallback: la común
        }
    }

    [CreateAssetMenu(menuName = "Frogames/Loot/Rarity")]
    public class Rarity : ScriptableObject
    {
        public string nombre;          // Común, Rara, Épica, Legendaria
        public int peso = 100;         // mayor peso = más frecuente
        public Color color = Color.white;
        public int numAfijos = 1;
    }

    [CreateAssetMenu(menuName = "Frogames/Loot/Affix")]
    public class Affix : ScriptableObject
    {
        public string nombre;          // "+X fuerza", "+Y% velocidad"
        public float min = 1f;
        public float max = 10f;
    }

    /// <summary>Ítem construido en runtime, no un asset.</summary>
    public class LootItem
    {
        public Rarity rareza;
        public List<AffixRoll> afijos;
    }

    public struct AffixRoll { public string nombre; public float valor; }
}
