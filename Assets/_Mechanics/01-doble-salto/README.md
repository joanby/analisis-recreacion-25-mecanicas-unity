# 01 — Doble salto (estilo Celeste)

Doble salto en 2D que "perdona" el timing del jugador con coyote time y jump buffer, en vez de exigir precisión de píxel.

## Escena mínima

- Personaje: cápsula 2D con `Rigidbody2D` (gravedad activa) + el componente `PlayerJump` (requiere `Rigidbody2D` vía `[RequireComponent]`).
- Un `Transform` hijo vacío llamado `groundCheck`, colocado a los pies de la cápsula.
- Dos plataformas (placeholders CC0: `SpriteRenderer` + `Collider2D`, o cubos aplastados) en una capa dedicada, p. ej. `Suelo`, separadas por un hueco saltable.
- Nada de tags ni colisiones manuales: el suelo se detecta solo por `LayerMask`.

## Cableado en el Inspector

- `velocidadSalto` = `12` — impulso vertical de cada salto (se fija directo en `linearVelocity.y`, no con `AddForce`).
- `saltosMaximos` = `2` — salto normal + doble salto.
- `groundCheck` → arrastra el Transform hijo de los pies.
- `radioGroundCheck` = `0.15` — radio del `Physics2D.OverlapCircle`.
- `capaSuelo` → marca la capa `Suelo` (el `LayerMask` usado por el overlap).
- `coyoteTime` = `0.1` — ventana de gracia tras dejar el borde.
- `jumpBuffer` = `0.1` — cuánto se recuerda una pulsación anticipada.

## Cómo probarlo

1. Dale a Play sobre una plataforma: pulsa `Espacio` (o botón sur del mando) → primer salto.
2. En el aire, pulsa `Espacio` otra vez → segundo salto (se consume `_saltosRestantes`).
3. Aterriza: el contador se resetea a `saltosMaximos` automáticamente.
4. Corre hasta el borde de una plataforma y pulsa `Espacio` justo después de caer (dentro de ~0,1 s): el coyote time deja saltar igual.
5. Pulsa `Espacio` justo antes de aterrizar sobre la otra plataforma: el jump buffer ejecuta el salto en cuanto toca suelo.

## Qué grabar

GIF de 4-6 s: personaje en la primera plataforma → salto → doble salto en el aire → aterriza en la segunda plataforma. Como cierre, un segundo intento breve mostrando el coyote time (salta justo tras abandonar el borde sin haber saltado antes). El resultado esperado en pantalla: dos impulsos verticales claramente distintos antes de tocar la segunda plataforma, sin ningún salto "fallado" por mal timing.
