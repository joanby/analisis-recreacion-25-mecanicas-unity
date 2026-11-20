# 21 — FSM de alerta con NavMesh (estilo Commandos)

FSM de guardia con cuatro estados (Patrulla → Sospecha → Alerta → Búsqueda) que nunca calcula si ve al jugador: solo consulta la señal booleana del `FieldOfView` y navega el nivel con `NavMeshAgent`, recordando el último punto donde lo vio.

## Escena mínima

- NavMesh horneado: `Window > AI > Navigation` (paquete **AI Navigation** en Unity 6). Marca suelo y muros como estáticos (`Navigation Static`) y pulsa **Bake**.
- Guardia (cápsula) con `NavMeshAgent` (obligatorio, `[RequireComponent]`), `FieldOfView` (mecánica 20) y `GuardAI`, todos en el mismo GameObject.
- Jugador (cápsula) en una capa dedicada, p. ej. `Target`, para que `FieldOfView` lo detecte.
- Un muro de prueba en una capa `Obstacle` para comprobar la oclusión del cono.
- 3-4 GameObjects vacíos como waypoints, colocados sobre el suelo horneado, formando un recorrido.

## Cableado en el Inspector

`GuardAI`:
- `fov` (FieldOfView): arrastra el componente `FieldOfView` del propio guardia.
- `jugador` (Transform): la cápsula del jugador.
- `waypoints` (Transform[]): los 3-4 vacíos del recorrido, en el orden en que quieres que los visite.
- `tiempoSospecha` (float, por defecto `2`): segundos detenido en Sospecha antes de confirmar o descartar.
- `tiempoBusqueda` (float, por defecto `4`): segundos buscando en el último punto conocido antes de rendirse.

`FieldOfView` (mismo GameObject):
- `viewRadius` (float, por defecto `8`) y `viewAngle` (0-360, por defecto `90`).
- `targetMask` (LayerMask): capa `Target` del jugador.
- `obstacleMask` (LayerMask): capa `Obstacle` de los muros.
- `subida` (float, por defecto `1.5`) y `decaimiento` (float, por defecto `0.5`): alimentan el medidor `Alerta`, que `GuardAI` no usa, pero cablealos igual para que el componente funcione.

## Cómo probarlo

1. Bakea el NavMesh antes de entrar en Play.
2. Entra en Play mode: el guardia patrulla solo, encadenando waypoints vía `SetDestination` (no hay input que lo dispare).
3. Mueve al jugador (arrastra su Transform en la vista Scene durante Play, o usa un controlador WASD simple) hasta el cono del guardia: el gizmo del `FieldOfView` pasa de amarillo a rojo y el guardia se detiene (Sospecha).
4. Mantente visible `tiempoSospecha` segundos: pasa a Alerta y el `NavMeshAgent` te persigue directamente.
5. Sal del cono: entra en Búsqueda, camina hasta `ultimoPuntoConocido`, espera `tiempoBusqueda` y, si no reapareces, vuelve a Patrulla.

## Qué grabar

Clip de 6-8 s: el guardia patrullando entre waypoints, el jugador entra en el cono (gizmo se pone rojo, el guardia se detiene un instante), luego lo persigue por el NavMesh esquivando el muro, el jugador se esconde tras el `Obstacle`, y el guardia camina hasta el último punto conocido antes de volver a su ruta. Resultado esperado: el gizmo del cono cambia de color según `JugadorVisible` y el guardia recorre visiblemente los cuatro estados sin quedarse pegado en ninguno.
