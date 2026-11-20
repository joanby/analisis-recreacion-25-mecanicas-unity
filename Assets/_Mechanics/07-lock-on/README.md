# 07 — Lock-on targeting (estilo Dark Souls)

Fija como objetivo al enemigo más centrado respecto a la mirada de la cámara dentro de un radio, y encuadra jugador+objetivo con un `CinemachineTargetGroup`; una segunda pulsación cicla al siguiente candidato.

## Escena mínima

- Jugador (cápsula) con el script `LockOnTargeting.cs`.
- 3-4 cubos CC0 como enemigos, todos en una capa dedicada (p. ej. `Enemy`) con `Collider`.
- Un `Transform` de cámara (o el propio rig de cámara) para calcular ángulos.
- Un `CinemachineTargetGroup` (paquete `com.unity.cinemachine`, namespace `Unity.Cinemachine`) más una virtual camera de combate que lo siga/encuadre.

## Cableado en el Inspector

- `Detección > lockRange` (float, por defecto `15`): radio del `Physics.OverlapSphere`.
- `Detección > enemyMask` (LayerMask): asigna la capa de los cubos enemigo.
- `Detección > maxLockAngle` (float, por defecto `60`): ángulo máximo respecto a `camara.forward` para aceptar un candidato.
- `Cámara > camara` (Transform): arrastra el transform de la cámara activa, o déjalo vacío — si no hay nada asignado, el script coge `Camera.main` solo en `Awake`.
- `Cámara > targetGroup` (CinemachineTargetGroup): arrastra el target group de la cámara de combate.
- `Cámara > pesoJugador` (float, por defecto `1`) y `pesoObjetivo` (float, por defecto `1`): pesos de encuadre en el target group (el objetivo se añade además con radio `1.5`, el jugador con radio `1`).
- `Movimiento básico > velocidadMovimiento` (float, por defecto `6`): WASD sin lock, y velocidad del strafe orbital con lock. No es la mecánica de esta lección, pero sin ella no hay forma de llegar hasta los enemigos ni de ver el strafe.
- `Movimiento básico > velocidadGiro` (float, por defecto `720`, grados/segundo): qué tan rápido gira el jugador para mirar al objetivo con lock activo.

## Cómo probarlo

1. Entra en Play mode y camina con `WASD` hasta quedar cerca de los cubos enemigo (dentro de `lockRange`).
2. El input de lock es la **tecla Q** (`Keyboard.current.qKey.wasPressedThisFrame`) — no el ratón: no todos los ratones tienen botón central, y Q es la convención habitual en PC para este tipo de mecánica.
3. Pulsa Q: se fija el enemigo con menor ángulo respecto a la mirada de la cámara, siempre que esté por debajo de `maxLockAngle`. El jugador empieza a girar para mirarlo, y el `WASD` lateral pasa a orbitar alrededor de él (strafe).
4. Con el lock activo, pulsa Q otra vez: salta al siguiente candidato en la lista ordenada por ángulo.
5. Aléjate del objetivo más de `lockRange * 1.2` o destrúyelo: el script suelta el lock automáticamente (`SoltarLock()`), y el `WASD` vuelve a moverte relativo a ti mismo en vez de al objetivo.

## Qué grabar

Clip de 6-8 s: sitúa 3 enemigos en pantalla, pulsa el botón central para fijar el más centrado (la cámara reencuadra vía `CinemachineTargetGroup`), pulsa de nuevo para saltar al siguiente candidato, y termina alejándote hasta que el lock se suelte solo. Resultado esperado: la cámara pasa de composición libre a encuadrar jugador+enemigo, cambia de objetivo al segundo clic y vuelve a composición libre al perder el rango.
