# 04 — Gancho / grappling hook (estilo Zelda BOTW)

Gancho que ancla por raycast y ofrece dos filosofías de atracción intercambiables por Inspector: columpio físico con `SpringJoint` o tirón directo aplicando fuerza cada `FixedUpdate`.

## Escena mínima

- Personaje: cuerpo 3D con `Rigidbody` (requerido vía `[RequireComponent]`) + `GrapplingHook`.
- Un `Transform` "boca del gancho" (puede ser el propio personaje o una cámara/hijo apuntando hacia delante) para asignar a `origen`.
- Uno o más anclajes elevados (cubos/plataformas CC0) en una capa dedicada `Anclable`, a menos de `rangoMax` unidades.
- Un `LineRenderer` en el propio GameObject (o hijo) para la cuerda; el script lo deja `enabled = false` en `Awake` y lo activa/desactiva él mismo.

## Cableado en el Inspector

- `origen` → el Transform que dispara el raycast (`origen.forward`).
- `capaAnclable` → marca la capa `Anclable`.
- `rangoMax` = `25` — alcance máximo del raycast de anclaje.
- `modo` = `Columpio` (por defecto; alternativa `TironDirecto`) — enum que decide la física de atracción.
- `fuerzaTiron` = `30` — solo se usa en modo `TironDirecto` (`AddForce` en `ForceMode.Acceleration`).
- `springFuerza` = `4.5` y `springDamper` = `7` — solo modo `Columpio`, configuran el `SpringJoint` (además `massScale = 4.5` fijo en código).
- `cuerda` → arrastra el `LineRenderer`. **Asígnale un material con shader URP** (`Universal Render Pipeline/Unlit` o `/Lit`) en su slot de Materials — sin material se ve rosa (shader no compatible). El grosor por defecto (`Width` = 1) suele quedar demasiado grueso; baja la curva a algo entre `0.05` y `0.1`.
- `velocidadMovimiento` = `6` — movimiento horizontal fuera del gancho (no es la mecánica de esta lección, pero sin él no llegas a un anclaje).
- `fuerzaBalanceo` = `15` — solo modo `Columpio`: empujón lateral con WASD que se **suma** al `SpringJoint` (vía `AddForce`, no fija la velocidad), para poder ganar altura en el columpio en vez de ser un péndulo puramente pasivo.

## Cómo probarlo

1. Dale a Play y camina con `WASD` hasta quedar orientado hacia un anclaje de la capa `Anclable` dentro de `rangoMax` (movimiento básico, no el gancho).
2. Clic izquierdo del ratón (`Mouse.current.leftButton`, `Lanzar()`): si el raycast impacta, se ancla, aparece la cuerda (`LineRenderer` con 2 puntos) y, en modo `Columpio`, se añade un `SpringJoint`.
3. Observa el balanceo físico (Columpio) — prueba a mantener `WASD` para empujar y ganar altura — o, si cambias `modo` a `TironDirecto`, el tirón constante hacia el punto en cada `FixedUpdate`.
4. Suelta el clic izquierdo (`Soltar()`): se destruye el `SpringJoint` (o cesa la fuerza) y la cuerda se desactiva; el `WASD` vuelve a responder de inmediato.

## Qué grabar

GIF de 5-8 s en modo `Columpio`: apunta a un anclaje elevado, clic izquierdo para engancharse, deja que el personaje balancee sobre el hueco y suelta el clic para caer con el impulso ganado. Opcionalmente un segundo clip corto en `TironDirecto` mostrando el ascenso directo hacia el anclaje. El resultado esperado en pantalla: la cuerda (`LineRenderer`) visible entre `origen` y el punto de anclaje mientras dura el enganche, y el personaje sigue una trayectoria de péndulo (o de tirón recto) claramente distinta al movimiento normal.
