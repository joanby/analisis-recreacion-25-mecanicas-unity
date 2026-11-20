# 09 — Bullet-time (estilo Max Payne)

Ralentiza el mundo con transición suave, escala `Time.fixedDeltaTime` en la misma proporción para no perder precisión física, baja el pitch del audio ambiente y consume un recurso limitado mientras el efecto está activo.

## Escena mínima

- Jugador con el script `BulletTimeController.cs`.
- Un disparo/proyectil con `Rigidbody` para comprobar que la física no pierde precisión durante la ralentización (proyectiles que no atraviesan paredes).
- Varias dianas CC0 como objetivos.
- Una barra de UI (Slider o Image tipo *fill*) cableada al valor de `Recurso` para visualizar el consumo.
- Un `AudioSource` con un clip ambiente en bucle.

## Cableado en el Inspector

- `Factor de tiempo > factorLento` (float, rango `0.05`-`1`, por defecto `0.3`): `Time.timeScale` objetivo durante el efecto.
- `Factor de tiempo > velocidadTransicion` (float, por defecto `4`): velocidad de interpolación (unidades/s en tiempo real) entre normal y lento.
- `Recurso (0-100) > recurso` (float, por defecto `100`): valor inicial del recurso.
- `Recurso > consumoPorSeg` (float, por defecto `40`) y `regenPorSeg` (float, por defecto `20`): tasas de consumo/regeneración por segundo real.
- `Audio > audioAmbiente` (AudioSource): arrastra la fuente de audio ambiente.
- `Audio > pitchGrave` (float, por defecto `0.6`): pitch mínimo al que baja el audio en bullet-time completo.

## Cómo probarlo

1. Entra en Play mode.
2. El input es la **tecla Shift izquierda mantenida** (`Keyboard.current.leftShiftKey.isPressed`).
3. Mantén Shift izquierda: el mundo se ralentiza suavemente hasta `factorLento` (0.3), el audio baja de tono y el recurso empieza a bajar a razón de `consumoPorSeg` por segundo real.
4. Suelta Shift izquierda: el factor vuelve a 1 con la misma velocidad de transición, el pitch se recupera y el recurso se regenera a `regenPorSeg`.
5. Mantén Shift izquierda hasta que el recurso llegue a 0: el efecto se desactiva automáticamente (`GestionarRecurso()` deja de pedir `factorLento` cuando `recurso <= 0`).
6. Comprueba que un proyectil disparado durante el bullet-time sigue colisionando correctamente contra las dianas (gracias al escalado de `Time.fixedDeltaTime`).

## Qué grabar

Clip de 6-8 s: dispara un proyectil en tiempo normal, luego mantén Shift izquierda para entrar en bullet-time, dispara de nuevo mostrando la trayectoria lenta y el pitch grave del audio, y suelta la tecla para volver a la normalidad. Resultado esperado: transición suave (no instantánea) de velocidad, la barra de UI del recurso bajando mientras se mantiene Shift y recuperándose al soltar, y el proyectil impactando la diana sin atravesarla.
