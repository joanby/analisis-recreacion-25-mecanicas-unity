# 11 — Portales (estilo Portal)

Dos portales que teletransportan un cuerpo conservando su momento ("fling"): la posición, la rotación y la velocidad se reorientan con la matriz relativa entre el portal de entrada y el de salida, no solo se copian.

## Escena mínima

3D. Un jugador con `Rigidbody` (no kinematic, para que se le pueda leer/reescribir `linearVelocity`) y `Collider`. Dos portales placeholder (cubos o planos CC0) cada uno con un `Collider` en modo **Is Trigger** y el componente `Portal`. Añade una rampa o un hueco de caída para que el jugador llegue a un portal con velocidad apreciable y así comprobar el "fling" al salir por el otro. Jerarquía sugerida: `Player` (Rigidbody+Collider), `PortalA` (Collider trigger + Portal.cs), `PortalB` (Collider trigger + Portal.cs).

## Cableado en el Inspector

- `Portal Destino` (`Portal`, privado serializado): en `PortalA` arrastra `PortalB`, y en `PortalB` arrastra `PortalA`. Cada portal debe apuntar a su pareja.
- `Tiempo Gracia` (float, por defecto `0.2f`): instante tras salir por un portal en el que ese mismo portal no puede recapturar al cuerpo (evita el teletransporte en bucle). No suele hacer falta tocarlo.

El script no expone ningún `Collider` público: el trigger que detecta el paso es el `Collider` (Is Trigger = true) que tú añadas al mismo GameObject del `Portal`.

## Cómo probarlo

1. Entra en Play.
2. Deja caer o lanza al jugador contra `PortalA` con velocidad (desde una rampa o una caída).
3. Comprueba que aparece junto a `PortalB`, girado 180° respecto a la orientación de salida, y que sigue moviéndose con una velocidad de magnitud similar a la de entrada pero reorientada al eje de `PortalB`.
4. Verifica que no vuelve a teletransportarse de inmediato hacia atrás (gracias a `tiempoGracia`).

## Qué grabar

Clip de 3-8 s: el jugador cae por un hueco o rampa hacia `PortalA` con velocidad visible, cruza el trigger y sale disparado por `PortalB` conservando esa velocidad (el "fling"), aterrizando o volando más allá del portal de salida. Resultado esperado en pantalla: el cuerpo cambia de posición instantáneamente entre los dos portales y su trayectoria post-salida muestra la misma "fuerza" con la que entró, sin frenazo ni bucle de teletransporte.
