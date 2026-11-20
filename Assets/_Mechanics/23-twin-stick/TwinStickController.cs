using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Control twin-stick al estilo Enter the Gungeon: movimiento y apuntado en
    /// ejes independientes con el Input System, y disparo con object pooling de
    /// balas (nunca Destroy).
    ///
    /// Referencia de la lección 04-impl-twin-stick. Las acciones Move y Aim se
    /// serializan como InputAction (stick izquierdo / stick derecho o ratón).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class TwinStickController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputAction move;
        [SerializeField] private InputAction aim;
        [SerializeField] private InputAction fire;

        [Header("Movimiento")]
        [SerializeField] private float velocidad = 6f;
        [SerializeField] private Transform arma;

        [Header("Disparo")]
        [SerializeField] private BulletPool pool;
        [SerializeField] private Transform boca;
        [SerializeField] private float velocidadBala = 14f;
        [SerializeField] private float cooldown = 0.15f;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private Vector2 _aimInput;
        private float _proximoDisparo;

        private void Awake() => _rb = GetComponent<Rigidbody2D>();

        private void OnEnable() { move.Enable(); aim.Enable(); fire.Enable(); }
        private void OnDisable() { move.Disable(); aim.Disable(); fire.Disable(); }

        private void Update()
        {
            _moveInput = move.ReadValue<Vector2>();
            _aimInput = aim.ReadValue<Vector2>();

            // Rotar el arma hacia la dirección de apuntado.
            if (_aimInput.sqrMagnitude > 0.01f)
            {
                float angulo = Mathf.Atan2(_aimInput.y, _aimInput.x) * Mathf.Rad2Deg;
                arma.rotation = Quaternion.Euler(0f, 0f, angulo);
            }

            if (fire.IsPressed() && Time.time >= _proximoDisparo)
                Disparar();
        }

        private void FixedUpdate()
        {
            // El movimiento del cuerpo va en FixedUpdate (física).
            _rb.linearVelocity = _moveInput * velocidad;
        }

        private void Disparar()
        {
            _proximoDisparo = Time.time + cooldown;
            Bullet bala = pool.GetBullet();
            bala.transform.SetPositionAndRotation(boca.position, arma.rotation);
            bala.Lanzar(arma.right * velocidadBala);
        }
    }

    /// <summary>Pool de balas reutilizables: reactiva libres, nunca instancia en caliente.</summary>
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private Bullet prefab;
        [SerializeField] private int inicial = 20;

        private readonly Queue<Bullet> _libres = new();

        private void Awake()
        {
            for (int i = 0; i < inicial; i++) CrearNueva();
        }

        private Bullet CrearNueva()
        {
            Bullet b = Instantiate(prefab, transform);
            b.Init(this);
            b.gameObject.SetActive(false);
            _libres.Enqueue(b);
            return b;
        }

        public Bullet GetBullet()
        {
            Bullet b = _libres.Count > 0 ? _libres.Dequeue() : CrearNueva();
            b.gameObject.SetActive(true);
            return b;
        }

        public void ReturnBullet(Bullet b)
        {
            b.gameObject.SetActive(false);
            _libres.Enqueue(b);
        }
    }

    /// <summary>Bala que se devuelve al pool al chocar o al expirar su vida.</summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float vida = 3f;
        private Rigidbody2D _rb;
        private BulletPool _pool;
        private float _muerteEn;

        public void Init(BulletPool pool) { _pool = pool; _rb = GetComponent<Rigidbody2D>(); }

        public void Lanzar(Vector2 velocidad)
        {
            _rb.linearVelocity = velocidad;
            _muerteEn = Time.time + vida;
        }

        private void Update()
        {
            if (Time.time >= _muerteEn) _pool.ReturnBullet(this);
        }

        private void OnTriggerEnter2D(Collider2D other) => _pool.ReturnBullet(this);
    }
}
