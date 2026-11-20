using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Árbol de habilidades al estilo Borderlands: nodos como ScriptableObject con
    /// prerrequisitos, estado en runtime (desbloqueados + puntos) y la regla de
    /// desbloqueo "tienes puntos Y todos los prerrequisitos".
    ///
    /// Referencia de la lección 04-impl-skill-tree. El evento OnCambio permite a la
    /// UI recolorear los nodos que pasan de bloqueados a disponibles.
    /// </summary>
    public class SkillTreeManager : MonoBehaviour
    {
        [SerializeField] private int puntosDisponibles;

        private readonly HashSet<SkillNode> _desbloqueados = new();

        /// <summary>La UI se suscribe para refrescarse tras cada cambio.</summary>
        public event System.Action OnCambio;

        public int Puntos => puntosDisponibles;
        public bool EstaDesbloqueado(SkillNode n) => _desbloqueados.Contains(n);

        /// <summary>Para probar el flujo sin un sistema de experiencia todavía.</summary>
        public void GanarPunto()
        {
            puntosDisponibles++;
            OnCambio?.Invoke();
        }

        /// <summary>
        /// Regla de desbloqueo: puntos suficientes Y todos los prerrequisitos ya
        /// desbloqueados. Un nodo raíz no tiene prerrequisitos.
        /// </summary>
        public bool PuedeDesbloquear(SkillNode n) =>
            !_desbloqueados.Contains(n)
            && puntosDisponibles >= n.coste
            && n.prerrequisitos.All(p => _desbloqueados.Contains(p));

        public bool Desbloquear(SkillNode n)
        {
            if (!PuedeDesbloquear(n)) return false;
            puntosDisponibles -= n.coste;
            _desbloqueados.Add(n);
            OnCambio?.Invoke();
            return true;
        }
    }

    /// <summary>Nodo del árbol definido como asset reutilizable.</summary>
    [CreateAssetMenu(menuName = "Frogames/SkillTree/SkillNode")]
    public class SkillNode : ScriptableObject
    {
        public string nombre;
        [TextArea] public string descripcion;
        public int coste = 1;
        public Sprite icono;
        [Tooltip("Nodos que deben estar desbloqueados antes. Vacío = nodo raíz.")]
        public List<SkillNode> prerrequisitos = new();
    }
}
