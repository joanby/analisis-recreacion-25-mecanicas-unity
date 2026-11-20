# 25 — Meta-progresión persistente (estilo Dead Cells)

Separa el estado efímero de una run (la moneda ganada en la partida) del estado permanente (moneda total y desbloqueos), que se serializa a JSON en disco y sobrevive a cerrar y reabrir el juego.

## Escena mínima

Un GameObject (p. ej. `MetaProgression`) con el script `MetaProgression`. No expone campos `[SerializeField]`: toda la interacción es por código o UI. Para probarlo en Play mode sin escribir un test aparte, añade 3-4 `Button` de UI (Canvas + TextMeshPro) cuyos `OnClick()` llamen a los métodos públicos del componente: `RecogerMoneda(int)`, `AlMorir()`, `Comprar(string, int)` e `IniciarRun()`. Opcional: un texto TMP que muestre `MonedaTotal` para ver el resultado sin abrir la consola.

## Cableado en el Inspector

`MetaProgression` no tiene campos serializados que cablear: al entrar en Play, `Awake()` llama a `MetaSaveSystem.Cargar()` y carga (o crea) el estado desde disco automáticamente. Lo único que se cablea son los botones de UI:
- Botón "Recoger moneda" → `OnClick()` → `MetaProgression.RecogerMoneda(10)` (o el valor que prefieras).
- Botón "Morir" → `OnClick()` → `MetaProgression.AlMorir()`.
- Botón "Comprar desbloqueo" → `OnClick()` → `MetaProgression.Comprar("armaExtra", 5)` (ajusta id y coste).
- Botón "Nueva run" → `OnClick()` → `MetaProgression.IniciarRun()`.

## Cómo probarlo

1. Entra en Play mode: `Awake()` carga `meta.json` si existe, o crea un `MetaData` vacío (`monedaTotal = 0`, sin desbloqueos).
2. Pulsa "Recoger moneda" varias veces: se suma a `_monedaDeLaRun`, en memoria, sin persistir todavía.
3. Pulsa "Morir": `AlMorir()` traspasa `_monedaDeLaRun` a `_meta.monedaTotal`, resetea la run a 0 y guarda vía `MetaSaveSystem.Guardar`, que escribe `meta.json` en `Application.persistentDataPath` (haz `Debug.Log(Application.persistentDataPath)` una vez para localizarlo, p. ej. en macOS `~/Library/Application Support/<Company>/<Product>/meta.json`).
4. Pulsa "Comprar desbloqueo": si `monedaTotal` cubre el coste y el id aún no estaba desbloqueado, lo resta, añade el id a `desbloqueos` y guarda de nuevo.
5. Sal de Play mode y vuelve a entrar (o cierra y reabre el editor): `Comprar` devuelve `false` la segunda vez sobre el mismo id (ya está en la lista) y `MonedaTotal` sigue reflejando lo guardado, confirmando que el JSON persistió en disco.

## Qué grabar

Clip de 6-8 s: pulsa "Recoger moneda" un par de veces, luego "Morir" (la moneda pasa de run a total), "Comprar desbloqueo" (baja el total, se añade el desbloqueo) y "Nueva run" (log en consola "Desbloqueo activo en esta run: ..."). Resultado esperado: el contador de moneda total en pantalla/consola sube al morir y baja al comprar, y tras salir y reentrar en Play mode los valores no se resetean a cero.
