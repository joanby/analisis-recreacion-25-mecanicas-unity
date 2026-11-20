# 14 — Árbol de habilidades (estilo Borderlands)

Cada habilidad es un nodo `ScriptableObject` con sus prerrequisitos declarados como asset; un manager mantiene el conjunto de nodos desbloqueados y aplica una única regla de validación: puntos suficientes y todos los prerrequisitos ya desbloqueados.

## Escena mínima

Un GameObject `SkillTreeManager` con el componente del mismo nombre. UI (uGUI o UI Toolkit) con un elemento por nodo que se suscribe a `OnCambio` para recolorearse, más un botón "Subir de nivel" que llame a `GanarPunto()` (para probar el flujo sin un sistema de experiencia todavía). No hace falta escena 3D: esto es lógica + UI pura.

## Cableado en el Inspector

En `SkillTreeManager`:
- `Puntos Disponibles` (int, privado serializado): puntos de partida; puedes dejarlo en `0` y usar el botón "Subir de nivel" para sumarlos en Play.

Crea 5-6 assets `SkillNode` vía `Assets → Create → Frogames → SkillTree → SkillNode`, formando un árbol con **un nodo raíz sin prerrequisitos**:
- `nombre`, `descripcion` (TextArea), `icono` (Sprite).
- `coste` (int, por defecto `1`): puntos que consume desbloquearlo.
- `prerrequisitos` (`List<SkillNode>`): arrastra los nodos que deben estar desbloqueados antes. Vacío en el nodo raíz.

## Cómo probarlo

1. Entra en Play; `puntosDisponibles` arranca en el valor del Inspector (o `0`).
2. Pulsa "Subir de nivel" para llamar a `GanarPunto()` y sumar un punto (dispara `OnCambio`).
3. Intenta desbloquear el nodo raíz: `PuedeDesbloquear` comprueba `puntosDisponibles >= coste` y que `prerrequisitos.All(...)` esté vacío o cumplido; `Desbloquear` resta el coste, añade el nodo al `HashSet` y dispara `OnCambio`.
4. Comprueba que el siguiente nodo (el que tenía la raíz como prerrequisito) pasa a "disponible" tras el paso anterior.
5. Intenta desbloquear un nodo con prerrequisitos incumplidos: `Desbloquear` devuelve `false` y no descuenta puntos.

## Qué grabar

Clip de 3-8 s: pulsar "Subir de nivel" para ganar un punto → clic sobre el nodo raíz para desbloquearlo (cambia de color/estado) → el siguiente nodo pasa de "bloqueado" a "disponible" automáticamente. Resultado esperado: la UI recolorea el nodo desbloqueado y habilita visualmente el siguiente en la cadena, sin que haga falta refrescar nada a mano (todo pasa por el evento `OnCambio`).
