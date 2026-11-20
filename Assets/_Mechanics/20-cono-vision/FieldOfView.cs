using System.Collections;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Cono de visión al estilo Metal Gear Solid: candidatos por rango
    /// (OverlapSphere), filtro por ángulo y comprobación de oclusión por raycast.
    /// Alimenta un medidor de alerta y expone la señal booleana JugadorVisible.
    ///
    /// Referencia de la lección 02-impl-cono-de-vision. La detección corre en una
    /// corrutina cada ~0,2 s por rendimiento, no en cada frame.
    /// </summary>
    public class FieldOfView : MonoBehaviour
    {
        [Header("Cono")]
        [SerializeField] private float viewRadius = 8f;
        [Range(0f, 360f)][SerializeField] private float viewAngle = 90f;
        [SerializeField] private LayerMask targetMask;    // el jugador
        [SerializeField] private LayerMask obstacleMask;  // muros

        [Header("Alerta")]
        [SerializeField] private float subida = 1.5f;
        [SerializeField] private float decaimiento = 0.5f;

        public bool JugadorVisible { get; private set; }
        public float Alerta { get; private set; }
        public float ViewRadius => viewRadius;
        public float ViewAngle => viewAngle;

        private void Start() => StartCoroutine(BuscarObjetivosLoop());

        private IEnumerator BuscarObjetivosLoop()
        {
            var espera = new WaitForSeconds(0.2f);
            while (true)
            {
                BuscarObjetivos();
                yield return espera;
            }
        }

        /// <summary>
        /// Núcleo conceptual: OverlapSphere sobre targetMask; para cada candidato,
        /// filtra por ángulo y, solo entonces, lanza el raycast de oclusión.
        /// </summary>
        private void BuscarObjetivos()
        {
            JugadorVisible = false;
            Collider[] candidatos = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            foreach (Collider c in candidatos)
            {
                Vector3 dirToTarget = (c.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) >= viewAngle / 2f)
                    continue; // fuera del cono

                float dst = Vector3.Distance(transform.position, c.transform.position);
                // Si el rayo NO golpea obstáculo, hay línea de visión.
                if (!Physics.Raycast(transform.position, dirToTarget, dst, obstacleMask))
                {
                    JugadorVisible = true;
                    break;
                }
            }
        }

        private void Update()
        {
            // El medidor de alerta sube al ver y decae al perder de vista.
            Alerta = Mathf.Clamp01(Alerta + (JugadorVisible ? subida : -decaimiento) * Time.deltaTime);
        }

        // Dibuja el radio y los dos vectores límite del cono.
        private void OnDrawGizmos()
        {
            Gizmos.color = JugadorVisible ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
            Vector3 a = DirDesdeAngulo(-viewAngle / 2f);
            Vector3 b = DirDesdeAngulo(viewAngle / 2f);
            Gizmos.DrawLine(transform.position, transform.position + a * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + b * viewRadius);
        }

        private Vector3 DirDesdeAngulo(float anguloGrados)
        {
            float rad = (anguloGrados + transform.eulerAngles.y) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
        }
    }
}
