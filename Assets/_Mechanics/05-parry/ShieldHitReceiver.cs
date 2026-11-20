using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Va en el Escudo (hijo del Player), no en ParrySystem. Unity llama a
    /// OnTriggerEnter en el GameObject cuyo propio Collider recibió el golpe,
    /// sin burbujearlo a los padres — así que si el Escudo es quien tiene el
    /// Collider que de verdad toca el arma enemiga, hace falta este relevo
    /// mínimo para que ParrySystem (en el Player) se entere.
    /// </summary>
    public class ShieldHitReceiver : MonoBehaviour
    {
        private void OnTriggerEnter(Collider armaEnemiga)
        {
            GetComponentInParent<ParrySystem>()?.RecibirImpacto(armaEnemiga);
        }
    }
}
