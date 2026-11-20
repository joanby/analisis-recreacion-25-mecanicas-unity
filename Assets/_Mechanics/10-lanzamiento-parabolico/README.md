# 10 — Lanzamiento parabólico con predicción de trayectoria (estilo Angry Birds)

Arrastra para apuntar, dibuja una línea de predicción que **simula** la física paso a paso con la misma gravedad y el mismo `dt` que usará el proyectil real, y lanza al soltar reproduciendo exactamente esa curva.

## Escena mínima

- Escena 2D con un punto de lanzamiento fijo (`Transform` vacío) y varias cajas CC0 como objetivos.
- Prefab de proyectil con `Rigidbody2D` (gravityScale = 1) y `Collider2D`.
- Un `LineRenderer` en la escena (o en el propio objeto lanzador) para pintar la trayectoria prevista.
- Objeto lanzador con el script `ProjectileLauncher.cs`.

## Cableado en el Inspector

- `Lanzamiento > proyectilPrefab` (Rigidbody2D): arrastra el prefab del proyectil.
- `Lanzamiento > puntoLanzamiento` (Transform): el punto desde el que se instancia y desde el que se calcula la trayectoria.
- `Lanzamiento > fuerzaMax` (float, por defecto `12`): magnitud máxima del vector de lanzamiento (clamp del arrastre).
- `Predicción > linea` (LineRenderer): arrastra el componente para dibujar la trayectoria.
- `Predicción > puntosSimulados` (int, por defecto `30`): número de puntos que integra la simulación paso a paso.

## Cómo probarlo

1. Entra en Play mode.
2. El input es el **ratón**: `Mouse.current.leftButton` (pulsar, mantener y soltar).
3. Pulsa el botón izquierdo sobre la escena: se fija `_origenArrastre` en coordenadas de mundo y se activa la línea (`linea.enabled = true`).
4. Mantén pulsado y mueve el ratón: la velocidad se calcula como el vector "tira y afloja" (`_origenArrastre - actual`, limitado a `fuerzaMax`) y la línea se redibuja cada frame integrando gravedad y velocidad con `Time.fixedDeltaTime`.
5. Suelta el botón: se instancia el proyectil en `puntoLanzamiento` y se le asigna `linearVelocity` con el mismo vector calculado; la línea se oculta.
6. Verifica que la trayectoria real del proyectil coincide con la línea punteada previa (misma gravedad, mismo `dt`).

## Qué grabar

Clip de 5-7 s: haz clic y arrastra hacia atrás para mostrar cómo aparece y se actualiza la línea punteada de predicción según el gesto, suelta para lanzar el proyectil y deja que golpee una de las cajas objetivo. Resultado esperado: la línea de predicción sigue el arrastre del ratón en tiempo real, desaparece al soltar, y el proyectil vuela describiendo exactamente la parábola que mostraba la línea hasta impactar la caja.
