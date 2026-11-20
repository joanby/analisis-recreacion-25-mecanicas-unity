# 16 — Loot con rareza y afijos (estilo Diablo)

Generador de ítems por tirada ponderada: cada rareza tiene un peso (la común pesa mucho más que la legendaria) y, una vez elegida, se le asignan afijos aleatorios cuya cantidad y magnitud escalan con esa rareza.

## Escena mínima

- Un `GameObject` vacío `LootGenerator` con el componente `LootGenerator.cs`.
- Un Canvas con una **tarjeta UI** (texto de nombre + color de fondo/borde según rareza) y un botón **"Derrotar enemigo"**.
- El script no toca la UI directamente: engancha el botón a un método propio (o a un wrapper) que llame a `GenerarItem()` y pinte el resultado (nombre de rareza, su `color`, y la lista de afijos con su valor tirado).

## Cableado en el Inspector

- `rarezas` (`List<Rarity>`): crea 4 assets vía **Assets → Create → Frogames → Loot → Rarity** y arrástralos en orden. Ejemplo de pesos (de la lección): Común `peso=70`, Rara `peso=20`, Épica `peso=8`, Legendaria `peso=2`. Cada `Rarity` tiene `nombre`, `peso` (por defecto `100`), `color` (por defecto blanco) y `numAfijos` (por defecto `1`) — sube `numAfijos` en las rarezas altas.
- `afijosPosibles` (`List<Affix>`): crea varios assets vía **Assets → Create → Frogames → Loot → Affix**, cada uno con `nombre` (p. ej. "+X fuerza"), `min` (por defecto `1f`) y `max` (por defecto `10f`).
- No hay más campos: el `_pesoTotal` se calcula solo en `Awake()` sumando los pesos de `rarezas`.

## Cómo probarlo

1. Entra en Play.
2. Pulsa el botón **"Derrotar enemigo"** varias veces seguidas (20-30 veces).
3. Observa que la mayoría de ítems salen de rareza común/rara y que legendaria aparece muy poco — confirma que la tirada es ponderada, no uniforme.
4. Revisa que cada ítem trae el número de afijos correcto para su rareza y que los valores caen dentro del rango `min`-`max` de cada `Affix`.

## Qué grabar

Clip de 5-8 s: pulsa el botón "Derrotar enemigo" varias veces seguidas en rápida sucesión y deja que se vean 4-5 tarjetas de ítem aparecer con distintos colores de rareza (gris/común predominando, y si sale, un flash del color legendario). Resultado esperado en pantalla: la tarjeta UI cambia de color según la rareza tirada y muestra los afijos con sus valores numéricos — evidencia visual de que el peso controla la escasez.
