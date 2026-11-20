using System.Collections;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Air dash con i-frames al estilo Hollow Knight / Celeste: corrutina que
    /// anula la gravedad, fija una velocidad de dash, cambia la capa a
    /// invulnerable durante la ventana y gestiona el cooldown.
    ///
    /// Referencia de la lección 06-impl-air-dash. La invulnerabilidad se resuelve
    /// cambiando el GameObject a una capa que, en la matriz de colisiones de
    /// Physics2D (Project Settings), NO colisiona con la capa "peligros".
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class AirDash : MonoBehaviour
    {
        [Header("Dash")]
        [SerializeField] private float velocidadDash = 20f;
        [SerializeField] private float duracionDash = 0.15f;
        [SerializeField] private float cooldown = 0.6f;

        [Header("Capas")]
        [Tooltip("Índice de la capa con i-frames (no colisiona con peligros).")]
        [SerializeField] private int capaInvulnerable = 8;

        [Header("Movimiento básico (no es la mecánica de esta lección)")]
        [Tooltip("Sin esto el personaje no puede acercarse al peligro para dashear a través. Rigidbody2D ya trae su propia gravedad, solo falta el X.")]
        [SerializeField] private float velocidadMovimiento = 6f;

        private Rigidbody2D _rb;
        private int _capaOriginal;
        private bool _enCooldown;
        private bool _enDash;
        private float _gravedadPreviaDash;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _capaOriginal = gameObject.layer;
        }

        private void Update()
        {
            if (DashPulsado() && !_enDash && !_enCooldown)
                StartCoroutine(DashCo(DireccionDash()));

            // Fuera del dash, movimiento horizontal normal. Durante el dash no
            // tocamos la velocidad: la fija la corrutina, y el Rigidbody2D ya
            // trae su propia gravedad, así que solo hace falta el eje X.
            if (!_enDash)
            {
                float input = LeerInputHorizontal();
                _rb.linearVelocity = new Vector2(input * velocidadMovimiento, _rb.linearVelocity.y);
            }
        }

        private float LeerInputHorizontal()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb == null) return 0f;
            float izquierda = kb.aKey.isPressed ? -1f : 0f;
            float derecha = kb.dKey.isPressed ? 1f : 0f;
            return izquierda + derecha;
        }

        private IEnumerator DashCo(Vector2 direccion)
        {
            _enDash = true;

            // 1) Guardar y anular la gravedad; fijar la velocidad del dash.
            _gravedadPreviaDash = _rb.gravityScale;
            _rb.gravityScale = 0f;
            _rb.linearVelocity = direccion * velocidadDash;

            // 2) Activar i-frames cambiando de capa.
            gameObject.layer = capaInvulnerable;

            yield return new WaitForSeconds(duracionDash);

            // 3) Restaurar gravedad, capa y frenar el impulso residual.
            _rb.gravityScale = _gravedadPreviaDash;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * 0.25f, _rb.linearVelocity.y);
            gameObject.layer = _capaOriginal;
            _enDash = false;

            // 4) Cooldown antes de permitir el siguiente dash.
            _enCooldown = true;
            yield return new WaitForSeconds(cooldown);
            _enCooldown = false;
        }

        private void OnDisable()
        {
            // Unity corta la corrutina en curso al desactivar el objeto sin ejecutar
            // el resto del método: sin esto, un dash interrumpido a mitad (p. ej. el
            // jugador muere o el objeto vuelve a un pool) deja la gravedad a 0 y la
            // capa de invulnerabilidad activa para siempre.
            if (_enDash)
            {
                _rb.gravityScale = _gravedadPreviaDash;
                gameObject.layer = _capaOriginal;
                _enDash = false;
            }
        }

        private Vector2 DireccionDash()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            Vector2 dir = Vector2.zero;
            if (kb != null)
            {
                dir.x = (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0);
                dir.y = (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0);
            }
            // Sin input direccional: dash horizontal según la orientación (escala X).
            if (dir == Vector2.zero) dir = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
            return dir.normalized;
        }

        private bool DashPulsado()
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            return kb != null && kb.leftShiftKey.wasPressedThisFrame;
        }
    }
}
