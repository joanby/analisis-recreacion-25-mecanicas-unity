using UnityEngine;
using UnityEngine.InputSystem;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Movimiento horizontal mínimo (izquierda/derecha). No es la mecánica de
    /// esta lección —el doble salto lo es—, pero hace falta para poder
    /// desplazarte entre plataformas y grabar el GIF del entregable.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMove : MonoBehaviour
    {
        [Tooltip("Velocidad horizontal en unidades/segundo.")]
        [SerializeField] private float velocidad = 5f;

        private Rigidbody2D _rb;

        private void Awake() => _rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            float input = LeerInputHorizontal();
            _rb.linearVelocity = new Vector2(input * velocidad, _rb.linearVelocity.y);
        }

        private float LeerInputHorizontal()
        {
            var teclado = Keyboard.current;
            var mando = Gamepad.current;

            float teclas = 0f;
            if (teclado != null)
            {
                if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) teclas -= 1f;
                if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) teclas += 1f;
            }

            float stick = mando != null ? mando.leftStick.x.ReadValue() : 0f;
            return Mathf.Clamp(teclas + stick, -1f, 1f);
        }
    }
}
