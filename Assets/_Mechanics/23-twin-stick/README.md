# 23 — Twin-stick shooter (estilo Enter the Gungeon)

Movimiento y apuntado en ejes independientes (kiting) leídos con el nuevo Input System, y disparo respaldado por un pool de balas que nunca instancia ni destruye en caliente.

## Escena mínima

2D. Jugador (cuadrado/cápsula CC0) con `Rigidbody2D` + `Collider2D` y el script `TwinStickController`; como hijo, un Transform `arma` que rota hacia el apuntado, y colgando de este un Transform `boca` en la punta del cañón. Un GameObject `BulletPool` (puede ser el propio jugador u otro aparte) con el script `BulletPool` y un prefab `Bullet` (círculo/cápsula CC0 con `Rigidbody2D` + `Collider2D` en modo **Is Trigger**). Añade algún objeto con `Collider2D` (muro, enemigo placeholder) para comprobar que las balas se reciclan al chocar: `OnTriggerEnter2D` las devuelve al pool contra cualquier trigger, sin filtrar por tag ni capa.

## Cableado en el Inspector

`TwinStickController`:
- `move`, `aim`, `fire` (InputAction, sin bindings por defecto en el código): configúralos tú directamente en el campo del Inspector. Ejemplo: `move` con un "2D Vector Composite" (WASD o flechas), `aim` con Pointer/position del ratón (o el stick derecho de un mando), `fire` con el clic izquierdo del ratón (o el gatillo derecho).
- `velocidad` (float, por defecto `6`).
- `arma` (Transform): el hijo que rota hacia el apuntado.
- `pool` (BulletPool): arrastra el GameObject con `BulletPool`.
- `boca` (Transform): punto de salida de la bala.
- `velocidadBala` (float, por defecto `14`).
- `cooldown` (float, por defecto `0.15`): segundos mínimos entre disparos.

`BulletPool`:
- `prefab` (Bullet): arrastra el prefab de bala.
- `inicial` (int, por defecto `20`): balas precreadas y desactivadas al arrancar (`Awake`).

`Bullet` (en el prefab):
- `vida` (float, por defecto `3`): segundos antes de volver al pool si no ha chocado con nada.

## Cómo probarlo

1. Entra en Play mode.
2. Mueve con la acción `move` que hayas bindeado (p. ej. WASD): el `Rigidbody2D` se desplaza en `FixedUpdate` vía `linearVelocity`.
3. Apunta con `aim` (ratón o stick derecho): el `arma` rota hacia esa dirección vía `Atan2`, sin relación con hacia dónde te mueves.
4. Mantén pulsado `fire`: dispara balas desde `boca` cada `cooldown` segundos mientras `Time.time >= _proximoDisparo`.
5. Comprueba que las balas desaparecen (se desactivan, nunca `Destroy`) al tocar cualquier `Collider2D` trigger o tras `vida` segundos sin impactar.

## Qué grabar

Clip de 5-8 s: el jugador moviéndose en una dirección (p. ej. hacia la izquierda) mientras dispara hacia otra (p. ej. hacia arriba), para que se vea la independencia de ejes; sigue disparando con cadencia constante para mostrar el pool reutilizando balas sin caídas de frame. Resultado esperado: el arma siempre apunta al cursor/stick mientras el cuerpo se mueve según `move`, y las balas se reciclan en vez de acumularse en la escena.
