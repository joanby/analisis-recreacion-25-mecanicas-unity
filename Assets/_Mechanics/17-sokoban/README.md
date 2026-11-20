# 17 — Empuje de bloques en rejilla con deshacer (estilo Baba Is You)

Movimiento discreto sobre coordenadas enteras: el jugador empuja cadenas de bloques casilla a casilla, y cada movimiento válido se apila en una pila de `undo` para poder deshacer con una tecla.

## Escena mínima

- 2D top-down. Un `GameObject` `GridController` con el componente `GridController.cs` (no necesita `Rigidbody2D`: el modelo es puramente `Dictionary`/`Vector2Int`).
- Varios cubos/sprites placeholder como **bloques**, cada uno un `Transform` suelto colocado en coordenadas enteras (`x`, `y` redondos).
- Un `Transform` separado para el **jugador visual** (una cápsula o sprite), distinto del `GameObject` que lleva el script.
- Al menos una casilla objetivo pintada como referencia visual (no forma parte del script, es solo guía para grabar).
- Alinea todo a la rejilla: `ARejilla()`/`AMundo()` asumen 1 unidad = 1 casilla.

## Cableado en el Inspector

- `bloques` (`List<Transform>`): arrastra los `Transform` de cada bloque de la escena. Sus posiciones iniciales (redondeadas a entero) definen la ocupación inicial del tablero.
- `muros` (`List<Vector2Int>`): añade a mano las coordenadas enteras de las casillas bloqueadas (p. ej. `(2,3)`, `(2,4)`).
- `jugadorVisual` (`Transform`): el transform visual del jugador (no el propio `GridController`).
- `velocidadInterpolacion` (`float`, por defecto `10`): velocidad del `Vector3.MoveTowards` que suaviza el salto entre casillas.

## Cómo probarlo

1. Entra en Play.
2. Mueve al jugador con las **flechas** (↑ ↓ ← →) — cada pulsación avanza una sola casilla.
3. Empuja un bloque contra otro bloque o contra un muro: el movimiento debe rechazarse si la casilla tras el último bloque de la cadena está ocupada o es muro.
4. Empuja dos bloques en fila (una cadena) y comprueba que se mueven juntos.
5. Pulsa **Z** para deshacer: jugador y bloques deben volver al estado anterior; deshacer varias veces seguidas debe encadenar pasos hasta el principio.

## Qué grabar

Clip de 6-8 s: empuja un bloque en línea recta 2-3 casillas, empújalo (sin querer) hasta encajarlo contra un muro o esquina, y pulsa **Z** un par de veces para deshacer el atasco. Resultado esperado en pantalla: el bloque avanza casilla a casilla con el salto interpolado suave, se bloquea limpiamente contra el muro, y al pulsar Z el jugador y el bloque saltan de vuelta a sus posiciones anteriores — la prueba visual de que el undo funciona.
