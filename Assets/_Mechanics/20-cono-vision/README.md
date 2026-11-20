# 20 — Cono de visión (estilo Metal Gear Solid)

Detección de percepción encadenando tres filtros de barato a caro: rango (`OverlapSphere`), ángulo (`Vector3.Angle` contra `transform.forward`) y línea de visión limpia (`Raycast` contra obstáculos). Si las tres se cumplen, un medidor de alerta sube; si no, decae.

## Escena mínima

- 3D. Un **guardia** (cápsula) con el componente `FieldOfView.cs`.
- Un **jugador** (cápsula) en una capa dedicada (p. ej. "Target").
- Al menos un **muro** de prueba (cubo) en una capa dedicada (p. ej. "Obstacle") que se pueda interponer entre guardia y jugador.
- No hace falta NavMesh ni Animator para esta mecánica aislada — solo geometría y capas.

## Cableado en el Inspector

- `viewRadius` (`float`, por defecto `8`): radio del `OverlapSphere`.
- `viewAngle` (`float`, `Range(0,360)`, por defecto `90`): ángulo total del cono (se compara la mitad, `viewAngle/2`, contra `Vector3.Angle`).
- `targetMask` (`LayerMask`): la capa del jugador ("Target").
- `obstacleMask` (`LayerMask`): la capa de los muros ("Obstacle").
- `subida` (`float`, por defecto `1.5`): velocidad de subida del medidor `Alerta` cuando `JugadorVisible == true`.
- `decaimiento` (`float`, por defecto `0.5`): velocidad de caída de `Alerta` cuando no hay visión.
- Propiedades públicas de solo lectura para leer desde otro script/UI: `JugadorVisible` (bool), `Alerta` (float, 0-1), `ViewRadius`, `ViewAngle`.

## Cómo probarlo

1. Entra en Play. La búsqueda corre en una corrutina cada ~0,2 s (`BuscarObjetivosLoop`), no cada frame.
2. Mueve al jugador dentro del `viewRadius` y del arco frontal del guardia: el gizmo del cono en la vista Scene debe pasar de amarillo a rojo (`JugadorVisible == true`).
3. Colócate justo detrás del muro de prueba, dentro del cono: la detección debe cortarse por oclusión aunque estés dentro de rango y ángulo.
4. Aléjate fuera de rango o del arco: observa que `Alerta` decae gradualmente en vez de caer a 0 de golpe.

## Qué grabar

Clip de 6-8 s: cruza andando por delante del guardia entrando y saliendo de su cono (gizmo amarillo→rojo→amarillo en la vista Scene o Game con Gizmos activos), luego repite el cruce pero escondiéndote tras el muro de prueba para mostrar cómo la oclusión corta la detección en seco. Resultado esperado en pantalla: el cono cambia de color según `JugadorVisible`, y si hay un medidor de `Alerta` en UI, se le ve subir suavemente al entrar en visión y decaer al perderla o esconderse tras el muro.
