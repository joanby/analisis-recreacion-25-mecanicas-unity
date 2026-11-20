using System.Collections;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Enemigo mínimo con un ataque telegrafiado en bucle: espera, avisa,
    /// golpea moviendo el arma de reposo a impacto, y vuelve a reposo.
    ///
    /// Existe para que ParrySystem tenga algo real contra lo que reaccionar —
    /// sin un atacante, no hay manera de ver el parry en acción. Referencia de
    /// la lección 02-impl-parry. Al recibir un Deflect (ParrySystem llama a
    /// <see cref="Aturdir"/>), la postura se rompe: el material del enemigo
    /// cambia y el bucle de ataque se pausa.
    /// </summary>
    public class EnemyAttack : MonoBehaviour
    {
        [Header("Arma")]
        [Tooltip("El objeto con el Collider (Is Trigger, tag EnemyWeapon) que golpea al defensor.")]
        [SerializeField] private Transform arma;
        [SerializeField] private Transform puntoReposo;
        [SerializeField] private Transform puntoAtaque;

        [Header("Ritmo del ataque")]
        [SerializeField] private float cooldownAtaque = 1.5f;
        [Tooltip("Aviso antes de golpear: la ventana real que tienes para reaccionar y parrear.")]
        [SerializeField] private float tiempoTelegraph = 0.6f;
        [SerializeField] private float duracionGolpe = 0.15f;

        [Header("Postura rota (feedback del Deflect)")]
        [SerializeField] private Renderer rendererEnemigo;
        [SerializeField] private Material materialNormal;
        [SerializeField] private Material materialAturdido;
        [SerializeField] private float duracionAturdido = 1f;

        private bool _aturdido;

        private void Start()
        {
            if (arma != null && puntoReposo != null) arma.position = puntoReposo.position;
            StartCoroutine(BucleAtaque());
        }

        private IEnumerator BucleAtaque()
        {
            while (true)
            {
                yield return new WaitForSeconds(cooldownAtaque);
                while (_aturdido) yield return null; // pausado mientras está aturdido

                yield return new WaitForSeconds(tiempoTelegraph); // el aviso que da tiempo a parrear
                if (_aturdido) continue; // se aturdió durante el propio aviso

                yield return MoverArma(puntoReposo.position, puntoAtaque.position, duracionGolpe);
                yield return MoverArma(puntoAtaque.position, puntoReposo.position, duracionGolpe);
            }
        }

        private IEnumerator MoverArma(Vector3 desde, Vector3 hasta, float duracion)
        {
            float t = 0f;
            while (t < duracion)
            {
                arma.position = Vector3.Lerp(desde, hasta, t / duracion);
                t += Time.deltaTime;
                yield return null;
            }
            arma.position = hasta;
        }

        /// <summary>Llamado por ParrySystem cuando el jugador consigue un Deflect.</summary>
        public void Aturdir()
        {
            if (_aturdido) return;
            StartCoroutine(RutinaAturdido());
        }

        private IEnumerator RutinaAturdido()
        {
            _aturdido = true;
            if (rendererEnemigo != null && materialAturdido != null)
                rendererEnemigo.material = materialAturdido;

            // Un enemigo aturdido no se queda a mitad de golpe: retira el arma.
            if (arma != null && puntoReposo != null) arma.position = puntoReposo.position;

            yield return new WaitForSeconds(duracionAturdido);

            if (rendererEnemigo != null && materialNormal != null)
                rendererEnemigo.material = materialNormal;
            _aturdido = false;
        }
    }
}
