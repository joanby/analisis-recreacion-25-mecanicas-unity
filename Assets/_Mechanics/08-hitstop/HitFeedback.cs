using System.Collections;
using UnityEngine;
using Unity.Cinemachine; // Cinemachine 3.x (Unity 6)

namespace Frogames.Mechanics
{
    /// <summary>
    /// Capa de "juiciness" al estilo Hades: orquesta cuatro efectos de impacto —
    /// hitstop (pausa breve de timeScale), screen shake (Cinemachine Impulse),
    /// flash de material y partículas— desde un único punto de entrada OnHit().
    ///
    /// Referencia de la lección 08-impl-hitstop-feedback. Todos los parámetros son
    /// serializados; OnHit se llama desde la resolución del golpe, nunca desde Update.
    /// </summary>
    public class HitFeedback : MonoBehaviour
    {
        [Header("Hitstop")]
        [Tooltip("timeScale durante el congelado (≈0.05 = casi parado).")]
        [SerializeField] private float hitstopTimeScale = 0.05f;
        [SerializeField] private float hitstopDuracion = 0.08f;

        [Header("Screen shake")]
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private float fuerzaShake = 0.3f;

        [Header("Flash de material")]
        [SerializeField] private Renderer objetivoRenderer;
        [SerializeField] private Color colorFlash = Color.white;
        [SerializeField] private float flashDuracion = 0.06f;

        [Header("Partículas")]
        [SerializeField] private ParticleSystem chispas;

        // MaterialPropertyBlock evita instanciar materiales por objeto.
        private MaterialPropertyBlock _mpb;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private float _timeScalePrevia = 1f;
        private bool _hitstopActivo;

        private void Awake() => _mpb = new MaterialPropertyBlock();

        /// <summary>
        /// Punto de entrada único: dispara los cuatro efectos en el impacto.
        /// </summary>
        public void OnHit(Vector3 puntoContacto)
        {
            StartCoroutine(HitStopCo());

            if (impulseSource != null)
                impulseSource.GenerateImpulse(fuerzaShake);

            if (objetivoRenderer != null)
                StartCoroutine(FlashCo());

            if (chispas != null)
            {
                chispas.transform.position = puntoContacto;
                chispas.Play();
            }
        }

        private IEnumerator HitStopCo()
        {
            _timeScalePrevia = Time.timeScale;
            _hitstopActivo = true;
            Time.timeScale = hitstopTimeScale;
            // Imprescindible tiempo REAL: si esperásemos en tiempo escalado, la
            // espera casi nunca terminaría (el propio timeScale está congelado).
            yield return new WaitForSecondsRealtime(hitstopDuracion);
            Time.timeScale = _timeScalePrevia;
            _hitstopActivo = false;
        }

        private void OnDisable()
        {
            // Si el objeto se desactiva/destruye a mitad del hitstop, Unity corta la
            // corrutina antes de su línea de restauración: sin esto, Time.timeScale
            // se quedaría "pegado" en hitstopTimeScale para todo el juego.
            if (_hitstopActivo)
            {
                Time.timeScale = _timeScalePrevia;
                _hitstopActivo = false;
            }
        }

        private IEnumerator FlashCo()
        {
            objetivoRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor(EmissionColor, colorFlash);
            objetivoRenderer.SetPropertyBlock(_mpb);

            yield return new WaitForSecondsRealtime(flashDuracion);

            _mpb.SetColor(EmissionColor, Color.black);
            objetivoRenderer.SetPropertyBlock(_mpb);
        }
    }
}
