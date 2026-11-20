using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Cámara de tercera persona con anti-clipping al estilo God of War (2018).
    /// Es la alternativa "de entendimiento" al Cinemachine Deoccluder: un SphereCast
    /// desde el objetivo hacia la cámara acerca la vista cuando hay una pared.
    ///
    /// Referencia de la lección 02-impl-camara-3p. En producción se recomienda la
    /// CinemachineCamera con órbita + extensión de colisión; este script explica el
    /// principio subyacente.
    /// </summary>
    public class CameraCollision : MonoBehaviour
    {
        [Header("Objetivo")]
        [SerializeField] private Transform objetivo;      // el personaje / punto de mira
        [SerializeField] private float distanciaDeseada = 5f;
        [SerializeField] private float radioSphereCast = 0.3f;
        [SerializeField] private LayerMask capaObstaculos;

        [Header("Damping")]
        [SerializeField] private float velocidadDamping = 10f;

        private float _distanciaActual;

        private void Start() => _distanciaActual = distanciaDeseada;

        private void LateUpdate()
        {
            // Dirección desde el objetivo hacia la cámara (hacia atrás del rig).
            Vector3 dir = (transform.position - objetivo.position).normalized;

            float objetivoDist = distanciaDeseada;
            if (Physics.SphereCast(objetivo.position, radioSphereCast, dir,
                    out RaycastHit hit, distanciaDeseada, capaObstaculos))
            {
                // Hay pared: acercamos la cámara hasta el punto de impacto.
                objetivoDist = hit.distance;
            }

            // Interpolamos la distancia para un movimiento amortiguado.
            _distanciaActual = Mathf.Lerp(_distanciaActual, objetivoDist,
                velocidadDamping * Time.deltaTime);

            transform.position = objetivo.position + dir * _distanciaActual;
            transform.LookAt(objetivo);
        }
    }
}
