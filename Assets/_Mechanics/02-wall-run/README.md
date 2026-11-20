# 02 — Wall-run (estilo Titanfall)

Carrera por pared en 3D: detecta la pared con raycasts laterales y redirige la velocidad a lo largo de su superficie para conservar el momento, en vez de tratarla como un muro que frena.

## Escena mínima

- Personaje: cápsula 3D con `CharacterController` (requerido vía `[RequireComponent]`) + `WallRun`.
- Dos paredes verticales (cubos CC0 estirados) enfrentadas, separadas más de `distanciaCheck` para dejar pasillo, en una capa dedicada `Pared`.
- Suelo para poder tomar impulso/salto hacia la pared (el wall-run solo entra si `!_cc.isGrounded`, es decir, el personaje debe estar en el aire al tocar la pared).

## Cableado en el Inspector

- `capaPared` → marca la capa `Pared`.
- `distanciaCheck` = `0.7` — alcance de los `Physics.Raycast` laterales (`transform.right` / `-transform.right`).
- `velocidadCarrera` = `8` — velocidad a lo largo de la pared.
- `duracionMax` = `1.5` — segundos de adherencia antes de caer (temporizador `_timer`).
- `gravedad` = `-20` — gravedad aplicada a mano cuando NO se está en wall-run (el `CharacterController` no tiene gravedad propia).
- `empujeLateral` = `8` — componente del salto de pared hacia fuera (a lo largo de la normal).
- `empujeVertical` = `7` — componente vertical del salto de pared.

## Cómo probarlo

1. Dale a Play, sitúa el personaje en el aire (saltando o cayendo) junto a una pared de la capa `Pared`.
2. Mantén pulsada `W` (única tecla de avance que lee el script, `InputAvance()`) mientras estás a menos de `distanciaCheck` de la pared y no tocas suelo: entra en wall-run (`_corriendoPared = true`).
3. Observa que no cae mientras corre por la pared (gravedad "apagada").
4. Pulsa `Espacio` mientras corre por la pared: salto de pared, impulso combinando la normal de la pared y el eje vertical.
5. Deja pasar `duracionMax` (1,5 s) sin saltar: sale del wall-run por temporizador y retoma la gravedad normal.

## Qué grabar

GIF de 5-8 s: el personaje salta hacia la primera pared, corre por ella manteniendo `W`, pulsa `Espacio` para saltar hacia la pared opuesta y encadena una segunda carrera. El resultado esperado: el personaje se desplaza pegado a la superficie vertical sin caer durante el wall-run, y el salto de pared lo lanza claramente hacia fuera y hacia arriba antes de alcanzar la otra pared.
