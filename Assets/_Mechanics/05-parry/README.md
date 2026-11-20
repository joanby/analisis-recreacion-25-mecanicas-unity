# 05 — Parry (estilo Sekiro)

Deflect por ventana temporal: una FSM de tres estados (`Idle → Parrying → Recovering`) que compara el instante del clic de parry contra el instante del impacto para decidir entre desvío, bloqueo tardío o golpe recibido. El atacante (`EnemyAttack`) golpea solo, en bucle, para que haya algo real que parrear.

## Escena mínima

- Defensor: Player (cápsula u otro placeholder CC0) con `ParrySystem`.
  - **Si el Player tiene un objeto Escudo aparte** (hijo, con su propio `Collider` no-trigger recibiendo el impacto): añade también `ShieldHitReceiver` en el Escudo. `OnTriggerEnter` no burbujea a los padres — si el golpe físico ocurre en el Escudo y `ParrySystem` vive en el Player, su `OnTriggerEnter` nunca se enteraría sin este relevo.
  - Si no hay Escudo y el `Collider` está en el propio Player, no hace falta `ShieldHitReceiver`: el `OnTriggerEnter` de `ParrySystem` ya recibe el golpe directamente.
- Atacante: cubo con el componente `EnemyAttack`, más:
  - Un hijo "arma" (p. ej. una esfera o cubo pequeño) con `Collider` en modo **Is Trigger** y el tag `EnemyWeapon` (imprescindible: la resolución del impacto comprueba `armaEnemiga.CompareTag("EnemyWeapon")` y descarta cualquier otro trigger).
  - Dos `Transform` vacíos como referencia de posición: `puntoReposo` (donde el arma descansa) y `puntoAtaque` (hacia el defensor), asignados en `EnemyAttack`.
- Para que Unity dispare el evento de trigger hace falta un `Rigidbody` en alguno de los dos objetos; `ParrySystem` ya trae el suyo vía `[RequireComponent]`.
- Ni `EnemyAttack` ni `ShieldHitReceiver` tienen `[RequireComponent]`: monta tú los colliders.

## Cableado en el Inspector

### ParrySystem (defensor)
- `parryWindow` = `0.2` — margen tras iniciar el parry en el que un golpe cuenta como `Deflect`.
- `recoverTime` = `0.3` — duración del estado `Recovering` antes de volver a `Idle` (durante ese tiempo no se puede iniciar otro parry, porque solo `Idle` lo acepta).
- `velocidadMovimiento` = `6` — movimiento horizontal del defensor (no es la mecánica de esta lección, pero sin él no llegas hasta el enemigo). Libre en todo momento: el parry no fija velocidad ni te inmoviliza.

### EnemyAttack (atacante)
- `arma` → el Transform del hijo "arma".
- `puntoReposo` / `puntoAtaque` → los dos Transform vacíos de referencia.
- `cooldownAtaque` = `1.5` — espera entre ataques.
- `tiempoTelegraph` = `0.6` — aviso antes de golpear; es tu ventana real de reacción, no relleno.
- `duracionGolpe` = `0.15` — cuánto tarda el arma en ir de reposo a impacto (y de vuelta).
- `rendererEnemigo` → el `Renderer` del cubo enemigo.
- `materialNormal` / `materialAturdido` → dos materiales para el cambio visual al recibir un Deflect.
- `duracionAturdido` = `1` — cuánto dura la postura rota antes de volver a atacar.

## Cómo probarlo

1. Dale a Play. El defensor arranca en `Idle`; el atacante empieza su bucle solo (no hace falta nada manual). Muévete con `WASD` hasta colocarte cerca del enemigo.
2. Espera el telegraph del atacante y, justo antes de que el arma llegue, clic derecho (`Mouse.current.rightButton`, `ParryPulsado()`): si el impacto llega dentro de `parryWindow` (0,2 s) verás en consola `"¡Deflect! Postura enemiga rota."`, el cubo cambia a `materialAturdido` y deja de atacar durante `duracionAturdido`.
3. Repite sin pulsar a tiempo (o parry tardío, fuera de la ventana pero en estado `Parrying`): consola muestra `"Bloqueo tardío: sin daño pero sin premio."`, sin cambio de material.
4. Deja que el arma golpee sin haber iniciado parry (`Idle`): consola muestra `"Golpe recibido: aplica daño."`.
5. Intenta un segundo parry mientras sigues en `Recovering` (dentro de los 0,3 s tras un deflect): no se inicia, porque `IniciarParry()` solo se dispara desde `Idle`.

## Qué grabar

GIF de 4-6 s con dos intentos consecutivos: primer intento, parry a tiempo → el cubo cambia de material y se aturde; segundo intento, sin parry o tarde → el golpe llega. El resultado esperado: el mismo ataque produce dos resultados visualmente distintos según únicamente el instante del clic, demostrando que la mecánica depende solo del timing.
