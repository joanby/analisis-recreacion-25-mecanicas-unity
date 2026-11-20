using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Portales que conservan el momento ("fling") al estilo Portal: transformación
    /// relativa entre el portal de entrada y el de salida para reubicar posición,
    /// rotación y velocidad del cuerpo que cruza.
    ///
    /// Referencia de la lección 06-impl-portales. El render con RenderTexture es el
    /// reto opcional; el mínimo obligatorio es el teletransporte con conservación
    /// de la velocidad. Cada portal referencia a su pareja en el Inspector.
    /// </summary>
    public class Portal : MonoBehaviour
    {
        [Tooltip("El otro portal por el que sale el cuerpo.")]
        [SerializeField] private Portal portalDestino;
        [Tooltip("Instante de gracia para que el portal de salida no recapture al cuerpo (s).")]
        [SerializeField] private float tiempoGracia = 0.2f;

        // Rotación de 180° en Y: el cuerpo "sale de frente" del portal destino.
        private static readonly Quaternion GiroSalida = Quaternion.Euler(0f, 180f, 0f);
        private float _recapturaBloqueadaHasta;

        private void OnTriggerEnter(Collider other)
        {
            if (Time.time < _recapturaBloqueadaHasta) return;
            if (!other.attachedRigidbody) return;
            Teletransportar(other.attachedRigidbody);
        }

        /// <summary>
        /// Construye la relación entrada→salida y la aplica a posición, rotación y
        /// velocidad. La magnitud de la velocidad se conserva; solo gira su dirección.
        /// </summary>
        private void Teletransportar(Rigidbody cuerpo)
        {
            // matriz relativa = salida * giro180 * inversa(entrada)
            Matrix4x4 relativa = portalDestino.transform.localToWorldMatrix
                                 * Matrix4x4.Rotate(GiroSalida)
                                 * transform.worldToLocalMatrix;

            // Posición y rotación en el espacio del portal destino.
            cuerpo.position = relativa.MultiplyPoint3x4(cuerpo.position);

            Quaternion giroRelativo = portalDestino.transform.rotation * GiroSalida
                                      * Quaternion.Inverse(transform.rotation);
            cuerpo.rotation = giroRelativo * cuerpo.rotation;

            // Solo la parte de rotación reorienta la velocidad (conserva el módulo).
            cuerpo.linearVelocity = giroRelativo * cuerpo.linearVelocity;

            // Evita el rebote inmediato en el portal de salida.
            portalDestino.BloquearRecaptura(tiempoGracia);
        }

        public void BloquearRecaptura(float segundos) =>
            _recapturaBloqueadaHasta = Time.time + segundos;
    }
}
