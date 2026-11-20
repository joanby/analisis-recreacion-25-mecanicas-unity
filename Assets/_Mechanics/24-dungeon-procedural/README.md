# 24 — Dungeon procedural por semilla (estilo The Binding of Isaac)

Genera el layout de una mazmorra en una rejilla a partir de una única semilla (`System.Random`): misma semilla, mismo mapa. Localiza además la sala más alejada del origen por BFS para marcarla como sala final.

## Escena mínima

Un GameObject vacío (p. ej. `DungeonGenerator`) con el script `DungeonGenerator`. Un `prefabSala` placeholder CC0 (un cubo/plano escalado, o un prefab de sala Kenney) que se instancia una vez por celda ocupada. No hace falta preparar nada más: el propio script limpia (`Destroy`) e instancia las salas en `Start()`.

## Cableado en el Inspector

- `seed` (int, por defecto `12345`): semilla de `System.Random`; misma semilla = mismo layout.
- `numSalas` (int, por defecto `10`): número de celdas a ocupar.
- `tamanoSala` (float, por defecto `12`): separación en world-space entre celdas contiguas (ejes X/Z).
- `prefabSala` (GameObject): arrastra el placeholder de sala.

## Cómo probarlo

1. Entra en Play mode. En `Start()` se genera automáticamente el layout con la `seed` del Inspector y aparece en consola: `Sala inicial (0,0), sala final (x,y). Semilla N.`
2. Pulsa **barra espaciadora** (lee `Keyboard.current.spaceKey.wasPressedThisFrame`): incrementa `seed` en 1 y regenera (`Limpiar()` + `Generar()`), sustituyendo las salas anteriores por un layout nuevo.
3. Observa en la vista Scene cómo las salas ocupan celdas contiguas partiendo de (0,0), con poda ocasional (25% de probabilidad al chocar con una celda ya ocupada) para no ramificar en exceso.
4. Reinicia Play mode dejando la misma `seed` en el Inspector y comprueba que el layout generado es idéntico al anterior.

## Qué grabar

Clip de 4-6 s: pulsa espacio varias veces seguidas y muestra cómo cada pulsación borra las salas anteriores e instancia un layout distinto (cámara alejada en planta para ver toda la rejilla), con la consola mostrando la sala inicial/final y la semilla de cada regeneración. Resultado esperado: cada pulsación cambia visiblemente el número y disposición de salas, y el log confirma la nueva semilla junto con la sala final calculada por BFS.
