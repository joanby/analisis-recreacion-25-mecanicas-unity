# 03 — Air dash (estilo Hollow Knight)

Impulso direccional corto orquestado con una corrutina: bloquea la velocidad y anula la gravedad durante el dash, da i-frames cambiando de capa, y frena el spam con un cooldown.

## Escena mínima

- Personaje: cápsula 2D con `Rigidbody2D` (requerido vía `[RequireComponent]`) + `AirDash`.
- Un obstáculo/"peligro" (placeholder CC0) en una capa `Peligros`.
- Una capa extra dedicada a la invulnerabilidad (por defecto **índice 8** en `capaInvulnerable`; créala/renómbrala en Project Settings → Tags and Layers si el índice 8 está libre).
- En **Project Settings → Physics 2D → Collision Matrix**: desmarca la casilla que cruza esa capa de invulnerabilidad con `Peligros`, para que el dash atraviese el obstáculo de verdad.

## Cableado en el Inspector

- `velocidadDash` = `20` — velocidad fija durante el dash (no acelerada).
- `duracionDash` = `0.15` — cuánto dura el impulso (`WaitForSeconds`).
- `cooldown` = `0.6` — espera tras el dash antes de permitir el siguiente.
- `capaInvulnerable` = `8` — índice de capa con i-frames (debe coincidir con la capa desmarcada en la matriz de colisiones).
- `velocidadMovimiento` = `6` — movimiento horizontal fuera del dash (no es la mecánica de esta lección, pero sin él no llegas al peligro).

## Cómo probarlo

1. Dale a Play y muévete con `A`/`D` hacia el obstáculo (movimiento básico, no el dash).
2. Mantén pulsadas `W`/`A`/`S`/`D` para fijar la dirección del dash (o suéltalas: sin input direccional el dash sale horizontal según la orientación en `transform.localScale.x`).
3. Pulsa `Shift izquierdo` (`leftShiftKey`) hacia el obstáculo de la capa `Peligros`: el personaje lo atraviesa sin recibir daño durante `duracionDash`.
3. Intenta dashear otra vez inmediatamente: no ocurre nada (`_enCooldown = true`).
4. Espera `cooldown` (0,6 s) y repite el dash en otra dirección.

## Qué grabar

GIF de 3-5 s: el personaje se acerca al obstáculo, pulsa `Shift izquierdo` y lo atraviesa limpiamente sin detenerse ni recibir daño; a continuación un intento fallido de re-dash inmediato (no pasa nada) seguido, tras la espera, de un segundo dash válido. El resultado esperado en pantalla: el sprite cruza el peligro durante el primer dash sin reacción de daño, y el segundo intento de dash queda "mudo" hasta cumplirse el cooldown.
