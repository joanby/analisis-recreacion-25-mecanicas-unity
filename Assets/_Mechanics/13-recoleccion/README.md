# 13 — Recolección / farming (estilo Stardew Valley)

Un huerto de tiles donde plantas semillas, avanzas el día con un botón "Dormir" para que maduren y cosechas lo listo; el inventario y el estado del huerto se persisten a JSON entre sesiones.

## Escena mínima

2D. Un grid de GameObjects `FarmTile`, cada uno con `SpriteRenderer` (requerido por `[RequireComponent]`) y un `Collider2D` (necesario para que `OnMouseDown` detecte el clic). Un GameObject `FarmingSystem` en la escena (el manager) con un botón UI "Dormir" enlazado a su método `Dormir()`. Placeholders CC0 para los sprites de semilla/cultivo maduro (o cuadrados de color).

## Cableado en el Inspector

En `FarmingSystem`:
- `Tiles` (`FarmTile[]`, privado serializado): arrastra todos los `FarmTile` de la escena.
- `Fichero Guardado` (string, por defecto `"farm_save.json"`): nombre del archivo dentro de `Application.persistentDataPath`.

En cada `FarmTile`:
- `Cultivo` (`CropData`, privado serializado): el asset de cultivo que planta ese tile.
- `Sistema` (`FarmingSystem`, privado serializado): referencia al manager de la escena.

Crea los ScriptableObjects vía `Assets → Create → Frogames → Farming`:
- **CropData**: `nombre`, `spriteSemilla`, `spriteMaduro`, `diasParaMadurar` (por defecto `3`), `cosecha` (referencia a un `ItemData`).
- **ItemData**: `id`, `nombre`, `sprite`, `valorVenta` (por defecto `10`).

## Cómo probarlo

1. Entra en Play; `Awake` carga el JSON si ya existe (`Cargar()`).
2. Haz clic sobre un tile vacío: pasa a "Plantado" y muestra `spriteSemilla`.
3. Pulsa el botón "Dormir": `Dormir()` incrementa `Dia`, llama a `Avanzar()` en cada tile (que suma edad y madura si `edad >= diasParaMadurar`) y guarda el JSON.
4. Pulsa "Dormir" tantas veces como `diasParaMadurar` indique; el tile pasa a "Maduro" y muestra `spriteMaduro`.
5. Haz clic sobre el tile maduro: se cosecha (`Recolectar`), añade el ítem al inventario interno y el tile vuelve a "Vacío".

## Qué grabar

Clip de 3-8 s: clic en un tile vacío para plantar (cambia a sprite de semilla) → pulsar "Dormir" una o varias veces hasta que el sprite cambie a maduro → clic para cosechar (el tile vuelve a vacío). Resultado esperado: el sprite del tile cambia en cada paso del ciclo (semilla → maduro → vacío) y, si se para y reinicia Play, el estado se restaura desde `farm_save.json` en `Application.persistentDataPath`.
