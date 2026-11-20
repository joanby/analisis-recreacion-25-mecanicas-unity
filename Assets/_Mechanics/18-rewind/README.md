# 18 — Rebobinado del tiempo (estilo Braid)

Graba en cada paso de física la posición, velocidad y rotación del `Rigidbody2D` en un buffer circular; al mantener la tecla de rewind congela la simulación y reproduce esos estados hacia atrás, restaurando la velocidad al soltar.

## Escena mínima

- 2D. Un objeto con `Rigidbody2D` (el script lo exige vía `[RequireComponent(typeof(Rigidbody2D))]`) y `Collider2D`, por ejemplo una cápsula o cubo placeholder.
- Una plataforma/suelo con `Collider2D` estático para que el objeto caiga y choque contra algo observable.
- Coloca el objeto en alto para que caiga con gravedad normal (`gravityScale` por defecto de Unity = 1) y así tener movimiento que rebobinar.
- No hace falta capa especial ni ground check: la mecánica es agnóstica al terreno.

## Cableado en el Inspector

- `segundosMemoria` (`float`, por defecto `5`): segundos de historia que se pueden rebobinar. `_maxFrames` se calcula solo en `Awake()` como `segundosMemoria / Time.fixedDeltaTime` (buffer circular).
- No hay más campos serializados; el resto (`_rb`, `_historial`) es interno.

## Cómo probarlo

1. Entra en Play y deja que el objeto caiga por gravedad.
2. Mantén pulsada **Q**: el objeto debe congelarse (deja de caer) y empezar a moverse hacia atrás por el camino recorrido.
3. Suelta **Q**: el objeto retoma la física normal, continuando con la velocidad del último punto rebobinado (sin salto brusco de posición).
4. Mantén **Q** más tiempo del que hay historial grabado: al agotarse el buffer, el rebobinado se detiene solo (llama a `DetenerRebobinado()` internamente) sin errores.

## Qué grabar

Clip de 5-7 s: deja caer el objeto desde una plataforma durante 1-2 s, luego mantén pulsada **Q** y muéstralo subiendo de vuelta por el mismo camino hasta cerca del punto de partida, y suelta la tecla para que retome la caída con inercia. Resultado esperado en pantalla: el objeto se mueve limpiamente hacia atrás en el tiempo mientras se mantiene Q, sin temblores ni interferencia de la gravedad (el `bodyType` pasa a `Kinematic` durante el rewind), y retoma una caída natural al soltar.
