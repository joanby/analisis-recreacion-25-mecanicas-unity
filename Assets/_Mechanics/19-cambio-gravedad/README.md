# 19 — Cambio de gravedad (estilo VVVVVV)

Invierte la `gravityScale` del `Rigidbody2D` con una sola tecla (solo si el personaje está apoyado), gira al personaje 180° para que quede "de pie" respecto al nuevo suelo, y el ground check pasa a detectar el techo en vez del suelo.

## Escena mínima

- 2D. Una sala con **suelo y techo**, ambos con `Collider2D` en una misma capa (p. ej. "SueloTecho") — la lección apunta a un único `LayerMask` que sirve para las dos superficies.
- Personaje: cápsula/cuadrado con `Rigidbody2D` (`[RequireComponent]`, `gravityScale = 1` por defecto).
- Un hijo `Transform` vacío como `groundCheck`, colocado justo bajo los "pies" del personaje.
- Deja algo de distancia entre suelo y techo para poder cruzar la sala en el aire durante el clip.

## Cableado en el Inspector

- `groundCheck` (`Transform`): el hijo vacío bajo los pies del personaje.
- `radioCheck` (`float`, por defecto `0.15`): radio del `OverlapCircle` de detección.
- `capaSueloTecho` (`LayerMask`): la capa común de suelo y techo.
- Input hardcodeado: **barra espaciadora** (`Keyboard.current.spaceKey.wasPressedThisFrame`), solo actúa si `Apoyado()` es verdadero.

## Cómo probarlo

1. Entra en Play con el personaje sobre el suelo.
2. Pulsa **Espacio**: el personaje debe invertir su caída (empieza a "caer" hacia el techo) y girar 180° visualmente.
3. Comprueba que, en pleno vuelo (sin contacto), pulsar Espacio **no** hace nada — la inversión solo se dispara apoyado (`Apoyado()` vía `OverlapCircle` en `groundCheck`).
4. Al aterrizar en el techo, pulsa Espacio de nuevo para volver a invertir y caer hacia el suelo original.
5. Usa `OnDrawGizmosSelected` (círculo cian en la vista Scene) para verificar visualmente que el `groundCheck` sigue al personaje tras cada giro.

## Qué grabar

Clip de 4-6 s: con el personaje en el suelo, pulsa Espacio y muéstralo cayendo hacia el techo, aterrizando "de pie" en él; pulsa Espacio otra vez para que caiga de vuelta al suelo. Resultado esperado en pantalla: el personaje gira 180° en cada inversión y la dirección de caída se invierte visiblemente, cruzando la sala solo mediante inversiones de gravedad — sin usar salto.
