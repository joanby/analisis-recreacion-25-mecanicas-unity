# 08 — Hitstop y feedback de impacto (estilo Hades)

Desde un único punto de entrada `OnHit(Vector3)`, orquesta cuatro capas de "juiciness" simultáneas: hitstop (pausa de `Time.timeScale`), screen shake (Cinemachine Impulse), flash de emisión del material y partículas de impacto.

## Escena mínima

- Reutiliza un golpe ya funcional de otra mecánica del módulo **con impacto real**. De parry y combos, solo el **parry** vale tal cual: `ComboSystem.cs` no gestiona colisión ni daño en absoluto, así que no hay ningún "impacto resuelto" al que enganchar `OnHit()` sin añadir antes detección de colisión. En `ParrySystem.cs`, las ramas `Deflect` y `Hit` de `AplicarResultado()` son el punto natural. Como `OnTriggerEnter` no da un punto de contacto exacto (a diferencia de `OnCollisionEnter`), aproxímalo con `arma.ClosestPoint(transform.position)`.
- Un `Renderer` en el objetivo (enemigo/cubo) cuyo material tenga **Emission activado** — no basta con que el shader lo soporte. En un material URP/Lit, marca la casilla **Emission** en su Inspector; si no, `MaterialPropertyBlock.SetColor("_EmissionColor", ...)` no hace nada visible aunque el código se ejecute sin errores.
- Un `CinemachineImpulseSource` en el actor o en la cámara, **y un `CinemachineImpulseListener` en la propia `CinemachineCamera`** — sin el Listener, `GenerateImpulse()` no produce ningún temblor: Impulse es "uno emite, varios escuchan". Comprueba que el **Channel Mask** del Listener incluye el canal del Source (ambos usan el canal 1 por defecto, pero confírmalo).
- Un `ParticleSystem` de chispas CC0 colocado como prefab o hijo de escena.

## Cableado en el Inspector

- `Hitstop > hitstopTimeScale` (float, por defecto `0.05`): `Time.timeScale` durante el congelado.
- `Hitstop > hitstopDuracion` (float, por defecto `0.08`): duración en segundos reales del congelado.
- `Screen shake > impulseSource` (CinemachineImpulseSource): arrastra el componente del actor/cámara.
- `Screen shake > fuerzaShake` (float, por defecto `0.3`): fuerza pasada a `GenerateImpulse()`.
- `Flash de material > objetivoRenderer` (Renderer): el renderer del enemigo golpeado. Recuerda: con Emission desactivada en el material, este campo no producirá ningún flash visible.
- `Flash de material > colorFlash` (Color, por defecto blanco): color de emisión durante el flash.
- `Flash de material > flashDuracion` (float, por defecto `0.06`): duración del flash.
- `Partículas > chispas` (ParticleSystem): el sistema de partículas de impacto.

## Cómo probarlo

1. Entra en Play mode y conecta el golpe existente (el deflect/hit del parry) contra el objetivo.
2. En el momento de la resolución del golpe (dentro de `AplicarResultado()` en `ParrySystem.cs`), llama manualmente a `hitFeedback.OnHit(puntoDeContacto)` (no hay input propio: este script no lee teclado/ratón, se dispara desde otro sistema).
3. Observa que los cuatro efectos ocurren a la vez: el juego se congela ~0.08 s, la cámara tiembla, el objetivo emite un flash blanco y saltan chispas en el punto de contacto.
4. Compara desactivando el componente `HitFeedback`: el golpe sigue conectando pero sin ningún efecto — la lógica de daño es idéntica, solo cambia la percepción.

## Qué grabar

Clip de 3-5 s: dispara el mismo golpe dos veces, una con `HitFeedback` desactivado y otra activado, para mostrar el contraste "golpe blando vs. golpe contundente". Resultado esperado: en la segunda toma se ve la micro-pausa del hitstop, el temblor de cámara, el destello blanco en el objetivo y el burst de chispas en el punto de contacto exacto del golpe.
