using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Ragdoll conmutable al estilo Human: Fall Flat. Alterna entre un cuerpo
    /// animado (Animator activo, huesos kinematic) y un cuerpo físico (Animator
    /// apagado, huesos simulándose y cayendo).
    ///
    /// Referencia de la lección 08-impl-ragdoll. El "comportamiento gracioso" no se
    /// programa: emerge de la cadena de ConfigurableJoint. La estabilidad se gana
    /// en el Inspector (límites angulares y masas), no en el código.
    /// </summary>
    public class RagdollController : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Animator animator;
        [Tooltip("Rigidbody del torso: recibe el empujón de AddForce.")]
        [SerializeField] private Rigidbody torso;

        [Header("Empujón de prueba")]
        [SerializeField] private float fuerzaEmpujon = 8f;

        private Rigidbody[] _huesos;
        private bool _enRagdoll;

        private void Awake()
        {
            // Recopila todos los Rigidbody de los huesos bajo este GameObject.
            _huesos = GetComponentsInChildren<Rigidbody>();
            DesactivarRagdoll(); // arrancamos "de pie", controlados por animación
        }

        private void Update()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb == null) return;

            if (kb.rKey.wasPressedThisFrame)
                Toggle();

            if (kb.fKey.wasPressedThisFrame && _enRagdoll && torso != null)
                torso.AddForce(transform.forward * fuerzaEmpujon, ForceMode.Impulse);
        }

        private void Toggle()
        {
            if (_enRagdoll) DesactivarRagdoll();
            else ActivarRagdoll();
        }

        /// <summary>Física al mando: el cuerpo se desploma.</summary>
        private void ActivarRagdoll()
        {
            if (animator != null) animator.enabled = false;
            foreach (Rigidbody hueso in _huesos)
                hueso.isKinematic = false;
            _enRagdoll = true;
        }

        /// <summary>Animación al mando: el cuerpo está "de pie".</summary>
        private void DesactivarRagdoll()
        {
            foreach (Rigidbody hueso in _huesos)
                hueso.isKinematic = true;
            if (animator != null) animator.enabled = true;
            _enRagdoll = false;
        }
    }
}
