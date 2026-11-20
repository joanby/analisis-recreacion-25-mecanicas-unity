# 15 — Crafting por recetas (estilo Minecraft)

Ítems y recetas como `ScriptableObject`, indexados en un diccionario mediante una clave normalizada (ids de ingredientes ordenados y unidos), que permite validar cualquier combinación en O(1) sin cadenas de `if`.

## Escena mínima

UI de "mesa de crafteo": un grid de ranuras (uGUI `GridLayoutGroup` o UI Toolkit) donde el jugador coloca ingredientes, y un botón "Craftear" enlazado a `Craftear(ranuras)`. Un GameObject `CraftingSystem` en la escena con el componente. No requiere escena 3D ni físicas.

## Cableado en el Inspector

En `CraftingSystem`:
- `Recetas` (`List<Recipe>`, privado serializado): arrastra todos los assets `Recipe` que quieras que el sistema reconozca. Se indexan en `Awake` por su clave normalizada (`_porClave`).

Crea los ScriptableObjects vía `Assets → Create → Frogames → Crafting`:
- **Item** (`CraftItem`): `id`, `nombre`, `sprite`. Crea 2-3 ingredientes y 1-2 resultados.
- **Recipe**: `ingredientes` (`List<CraftItem>`) y `resultado` (`CraftItem`). Ejemplo: `ingredientes = [Palo, Piedra]`, `resultado = Hacha`.

## Cómo probarlo

1. Entra en Play; `Awake` construye `_porClave` recorriendo `recetas` y llamando a `Clave(r.ingredientes)` por cada una.
2. Coloca en las ranuras los `CraftItem` de una receta válida (en cualquier orden: `Clave` los ordena por `id`, así "palo+piedra" y "piedra+palo" son la misma entrada).
3. Pulsa "Craftear": `Craftear(ranuras)` calcula `Clave(ranuras)` y la busca en `_porClave`; si existe, devuelve el `CraftItem` resultado (el consumo de ingredientes y la entrada al inventario son responsabilidad de quien llama al método, como indica el comentario del código).
4. Coloca una combinación no registrada y pulsa "Craftear": debe devolver `null` sin efecto alguno.

## Qué grabar

Clip de 3-8 s: arrastrar dos ingredientes válidos a las ranuras de la mesa → pulsar "Craftear" → aparece el ítem resultado en pantalla/inventario. Luego, opcionalmente, mostrar una combinación inválida donde pulsar "Craftear" no produce nada. Resultado esperado: la combinación válida entrega el `CraftItem` correcto (visible por su sprite/nombre en UI), y la inválida no cambia el estado ni consume ingredientes.
