# 06 — Combos con buffer de input y cancels (estilo Devil May Cry)

Encadena tres golpes (`Attack1 → Attack2 → Attack3`) usando un buffer que recuerda la pulsación anticipada del jugador y solo avanza el combo cuando el `Animator` abre una ventana de cancel.

## Escena mínima

- Cápsula jugadora con componente `Animator` **y** `ComboSystem.cs` en el **mismo GameObject** (los animation events llaman a los métodos por `SendMessage`, que solo llega a componentes del objeto que lleva el Animator — si el Animator acaba en un hijo distinto, `ComboSystem` nunca se entera).
- Un hijo "Espada" (cubo estirado o cápsula fina) — es lo que animas, no la cápsula entera.
- Enemigo estático de prueba (cubo CC0) solo como referencia visual; el script no gestiona daño ni colisiones (no hace falta collider especial en el arma para esta lección).

### Los 3 clips de ataque — sin assets externos

Con la **cápsula** seleccionada (no la Espada): `Window > Animation > Animation` → botón **Create**, nómbralo `Attack1`. En modo grabación, selecciona el hijo Espada y anima su rotación local en unos pocos keyframes (reposo → armado → swing → recuperación). Repite para `Attack2` y `Attack3` variando el eje del giro. Desmarca **Loop Time** en los tres: son ataques de un solo disparo, no idles en bucle.

## El Animator Controller

- Un `Animator Controller` con tres estados `Attack1`, `Attack2`, `Attack3` (se crean solos al usar "Create" en el paso anterior) controlados por el parámetro **Int** `attackStep`.
- *Animation events* en cada clip: fotograma 0 → `CloseCancelWindow()`; ~65-70% del clip → `OpenCancelWindow()`; último fotograma → `ResetCombo()`.
- Transiciones `Idle → Attack1/2/3` por `attackStep`, y **`AttackX → Idle` con condición `attackStep == 0`, `Has Exit Time` desmarcado, duración 0**. Sin esto, el Animator se queda congelado en el último fotograma del ataque en vez de volver a `Idle` — no hay error en consola, simplemente no pasa nada al pulsar de nuevo.

## Cableado en el Inspector

- `Buffer > bufferTime` (float, por defecto `0.25`): segundos que se recuerda una pulsación de ataque antes de descartarla.
- No hay más campos serializados: el `Animator` se obtiene automáticamente en `Awake()` vía `GetComponent<Animator>()`.

## Cómo probarlo

1. Entra en Play mode.
2. El input es el **botón izquierdo del ratón** (`Mouse.current.leftButton.wasPressedThisFrame`), no teclado.
3. Pulsa el clic izquierdo a ritmo: cada pulsación dentro de la ventana de cancel abierta avanza `attackStep` (None → One → Two → Three).
4. Prueba pulsar el segundo golpe **antes** de que se abra la ventana de cancel del primero: el buffer guarda el timestamp y lo consume en cuanto `OpenCancelWindow()` se dispara, sin perder la pulsación.
5. Deja pasar más de `bufferTime` (0.25 s) sin pulsar tras el último golpe: el buffer caduca y no se dispara ningún avance.
6. **Ojo con el "combo fantasma":** si pulsas justo cerca del final de `Attack3` y `ResetCombo()` no limpia también `lastAttackTime`, ese clic residual se reprocesa en cuanto la ventana se reabre y arranca un combo nuevo solo (verás `attackStep` saltar a 1 sin que hayas vuelto a pulsar, y no volver nunca a 0). `ResetCombo()` debe poner `lastAttackTime = -99f` además de resetear `attackStep`.

## Qué grabar

Clip de 5-8 s: encadena los tres golpes pulsando clic izquierdo al ritmo del combo (Attack1 → Attack2 → Attack3), y en una segunda toma pulsa el siguiente golpe ligeramente antes de que acabe la animación actual para demostrar que el buffer conserva la pulsación. Resultado esperado: el parámetro `attackStep` del Animator sube de 0 a 3 en la ventana Animator (o en un log de consola si lo añades), y las tres animaciones de ataque se reproducen sin cortes ni pulsaciones perdidas.
