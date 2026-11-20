using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Inversión de gravedad al estilo VVVVVV: al pulsar (y solo apoyado), niega la
    /// gravityScale, reorienta al personaje 180° y recoloca el ground check al
    /// suelo o al techo según el signo de la gravedad.
    ///
    /// Referencia de la lección 06-impl-cambio-de-gravedad. La inversión se
    /// restringe a cuando hay contacto para evitar flips en pleno vuelo.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class GravityFlip : MonoBehaviour
    {
        [Header("Ground / ceiling check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float radioCheck = 0.15f;
        [SerializeField] private LayerMask capaSueloTecho;

        private Rigidbody2D _rb;

        private void Awake() => _rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            if (InvertirPulsado() && Apoyado())
                Invertir();
        }

        /// <summary>Niega la gravedad, gira al personaje y recoloca el check.</summary>
        private void Invertir()
        {
            _rb.gravityScale *= -1f;

            // Rotar 180° en Z para quedar "de pie" respecto al nuevo suelo.
            transform.Rotate(0f, 0f, 180f);

            // El groundCheck (hijo) se voltea con el Transform: apuntará al techo
            // cuando la gravedad sea negativa. Nada más que hacer aquí.
        }

        private bool Apoyado() =>
            Physics2D.OverlapCircle(groundCheck.position, radioCheck, capaSueloTecho);

        private bool InvertirPulsado()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            return kb != null && kb.spaceKey.wasPressedThisFrame;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, radioCheck);
        }
    }
}
