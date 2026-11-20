using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Combos con cancels y buffer de input al estilo Devil May Cry: un buffer
    /// que recuerda la pulsación anticipada y una máquina de pasos de ataque que
    /// solo avanza si la ventana de cancel está abierta.
    ///
    /// Referencia de la lección 04-impl-combos. Las ventanas de cancel las abren
    /// y cierran los animation events del Animator llamando a
    /// OpenCancelWindow / CloseCancelWindow.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ComboSystem : MonoBehaviour
    {
        public enum AttackStep { None, One, Two, Three }

        [Header("Buffer")]
        [Tooltip("Cuánto tiempo se recuerda una pulsación de ataque (s).")]
        [SerializeField] private float bufferTime = 0.25f;

        private Animator _animator;
        private AttackStep _paso = AttackStep.None;
        private float _lastAttackTime = -999f;
        // Empieza abierta: en Idle no hay ningún clip reproduciéndose que pueda
        // llamar a OpenCancelWindow(), así que el primer golpe de un combo debe
        // aceptarse sin depender de un animation event.
        private bool _ventanaCancelAbierta = true;

        private void Awake() => _animator = GetComponent<Animator>();

        private void Update()
        {
            // 1) Todo input nuevo entra al buffer (guarda el timestamp).
            if (AtaquePulsado())
                _lastAttackTime = Time.time;

            // 2) Si la ventana de cancel está abierta y hay un ataque bufferizado,
            //    avanzamos al siguiente paso, lo pasamos al Animator y limpiamos el buffer.
            if (_ventanaCancelAbierta && HasBufferedAttack())
            {
                Avanzar();
                _lastAttackTime = -999f; // consume el buffer
            }
        }

        private bool HasBufferedAttack() => Time.time - _lastAttackTime <= bufferTime;

        private void Avanzar()
        {
            _paso = _paso switch
            {
                AttackStep.None => AttackStep.One,
                AttackStep.One => AttackStep.Two,
                AttackStep.Two => AttackStep.Three,
                _ => AttackStep.Three // el tercer golpe no encadena más
            };
            _animator.SetInteger("attackStep", (int)_paso);
            _ventanaCancelAbierta = false; // se reabrirá vía animation event del nuevo golpe
        }

        // ---- Llamados por animation events desde los clips del Animator ----
        public void OpenCancelWindow() => _ventanaCancelAbierta = true;
        public void CloseCancelWindow() => _ventanaCancelAbierta = false;

        /// <summary>Se invoca al final del último golpe o cuando el combo caduca.</summary>
        public void ResetCombo()
        {
            _paso = AttackStep.None;
            // Vuelve a Idle: reabre la ventana para poder iniciar el siguiente
            // combo (ver comentario en el campo _ventanaCancelAbierta).
            _ventanaCancelAbierta = true;
            _animator.SetInteger("attackStep", 0);

            // Limpia también el buffer: sin esto, un clic cerca del final del
            // último golpe queda "vivo" en el buffer, y en cuanto la ventana se
            // reabre aquí mismo se reprocesa como si fuera un clic nuevo,
            // arrancando un combo fantasma (None -> One) sin que hayas vuelto
            // a pulsar. Es la misma clase de bug que el "nunca se para" del
            // wall-run: resetear el estado sin resetear también el input.
            _lastAttackTime = -999f;
        }

        private bool AtaquePulsado()
        {
            var m = UnityEngine.InputSystem.Mouse.current;
            return m != null && m.leftButton.wasPressedThisFrame;
        }
    }
}
