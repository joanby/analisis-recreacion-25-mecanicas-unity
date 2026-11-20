using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Bullet-time al estilo Max Payne: escala global de la simulación con
    /// transición suave, ajuste del paso de física, pitch grave del audio y un
    /// recurso que se consume mientras el efecto está activo.
    ///
    /// Referencia de la lección 02-impl-bullet-time. Clave: la interpolación del
    /// factor usa Time.unscaledDeltaTime, o la propia transición se ralentizaría
    /// a sí misma.
    /// </summary>
    public class BulletTimeController : MonoBehaviour
    {
        [Header("Factor de tiempo")]
        [Range(0.05f, 1f)][SerializeField] private float factorLento = 0.3f;
        [Tooltip("Velocidad de interpolación entre normal y lento (unidades/s en tiempo real).")]
        [SerializeField] private float velocidadTransicion = 4f;

        [Header("Recurso (0-100)")]
        [SerializeField] private float recurso = 100f;
        [SerializeField] private float consumoPorSeg = 40f;
        [SerializeField] private float regenPorSeg = 20f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioAmbiente;
        [SerializeField] private float pitchGrave = 0.6f;

        private float _baseFixed;      // fixedDeltaTime original
        private float _factorActual = 1f;
        private float _factorObjetivo = 1f;

        public float Recurso => recurso;

        private void Awake() => _baseFixed = Time.fixedDeltaTime;

        private void Update()
        {
            GestionarRecurso();

            // Interpolación en TIEMPO REAL: si usáramos deltaTime, al frenar el
            // mundo la transición también se frenaría.
            _factorActual = Mathf.MoveTowards(_factorActual, _factorObjetivo,
                velocidadTransicion * Time.unscaledDeltaTime);

            Time.timeScale = _factorActual;
            Time.fixedDeltaTime = _baseFixed * _factorActual; // conserva precisión de FixedUpdate

            if (audioAmbiente != null)
                audioAmbiente.pitch = Mathf.Lerp(1f, pitchGrave, 1f - _factorActual);
        }

        private void GestionarRecurso()
        {
            bool quiereLento = ActivacionMantenida() && recurso > 0f;
            _factorObjetivo = quiereLento ? factorLento : 1f;

            if (quiereLento)
                recurso = Mathf.Max(0f, recurso - consumoPorSeg * Time.unscaledDeltaTime);
            else
                recurso = Mathf.Min(100f, recurso + regenPorSeg * Time.unscaledDeltaTime);
        }

        private bool ActivacionMantenida()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            return kb != null && kb.leftShiftKey.isPressed;
        }

        private void OnDisable()
        {
            // Restaurar el tiempo por si se desactiva a mitad de efecto.
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _baseFixed;
        }
    }
}
