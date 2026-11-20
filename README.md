# Proyecto Unity — Análisis y Recreación de 25 Mecánicas

Repositorio-plantilla del curso **Análisis y Recreación de 25 Mecánicas de Videojuegos en Unity** (Frogames Formación). Contiene **un script C# de referencia por mecánica**: el snippet conceptual de cada lección de implementación, formalizado y comentado en español.

> ⚠️ **Código de referencia, no un juego jugable.** Estos scripts son el "solucionario" técnico de cada mecánica. Para verlos en acción hay que montar la escena mínima que describe cada lección (personaje/cápsula, suelo, prefabs placeholder) y cablear los campos serializados en el Inspector. La idea del curso es que **tú** reconstruyas cada mecánica; consulta estos scripts cuando te atasques o para comparar.

## Cómo abrir

1. Instala **Unity Hub** y, desde él, **Unity 6** (LTS, versión `6000.x` o superior) con los módulos de tu plataforma de destino. El proyecto se creó con `6000.3.22f1`; si tienes otra versión de Unity 6, el Hub te ofrecerá abrirlo con la tuya.
2. Clona el repo: `git clone https://github.com/joanby/analisis-recreacion-25-mecanicas-unity.git`
3. En Unity Hub → **Add** → **Add project from disk** → elige la carpeta clonada.
4. La primera vez Unity genera `Library/` (tarda unos minutos). Si te pregunta por el **backend de input**, acepta y deja que reinicie.
5. Verifica que el proyecto **abre y compila sin errores** en la consola y que `Assets/Scenes/EscenaEjemplo` entra en Play.

## Paquetes

Input System, Cinemachine 3 y AI Navigation **ya vienen instalados** (`Packages/manifest.json`). URP y TextMeshPro se añaden cuando una lección los pida, vía **Window → Package Manager → Unity Registry**:

| Paquete | Package name | Para qué se usa |
| --- | --- | --- |
| **Input System** | `com.unity.inputsystem` | Todo el input (movimiento, combate, twin-stick). Al activarlo, acepta el reinicio del editor para el nuevo backend. |
| **Cinemachine** (3.x) | `com.unity.cinemachine` | Cámaras: lock-on (`CinemachineTargetGroup`), hitstop/shake (`CinemachineImpulseSource`), 3ª persona. Namespace `Unity.Cinemachine`. |
| **Universal Render Pipeline (URP)** | `com.unity.render-pipelines.universal` | Base de render 2D y 3D. |
| **AI Navigation** | `com.unity.ai.navigation` | NavMesh para la IA de guardia (FSM de alerta). |
| **TextMeshPro** | (Essentials) | Texto de UI donde el proyecto lo pida. |

> **Nota de vigencia (Unity 7):** anunciado en Unite Seoul 2026 (beta dic-2026, release Q1 2027) como continuación directa de Unity 6 con upgrade sin rebuild. El material sigue vigente al migrar. Documenta la versión exacta de Input System y Cinemachine en tu propio repo por si hay deriva.

### API de Unity 6 usada aquí

Los scripts usan las **últimas** APIs. En particular, la velocidad de un `Rigidbody`/`Rigidbody2D` se lee y escribe con **`linearVelocity`** (la antigua `velocity` está obsoleta en Unity 6). El input se lee con el **nuevo Input System** (`Keyboard.current`, `Mouse.current`, `Gamepad.current`, `InputAction`), no con el `Input` manager clásico.

## Estructura

```
analisis-recreacion-25-mecanicas-unity/
├── README.md                 ← este fichero
├── Packages/                 ← paquetes del proyecto
├── ProjectSettings/
└── Assets/
    ├── Scenes/EscenaEjemplo.unity   ← escena de ejemplo (suelo + cápsula)
    ├── _ProjectStructure.md   ← organización de escenas (una por mecánica) + capstone
    └── _Mechanics/
        ├── 01-doble-salto/        PlayerJump.cs
        ├── 02-wall-run/           WallRun.cs
        ├── ...                    (una carpeta por mecánica)
        └── 25-meta-progresion/    MetaProgression.cs
```

Cada carpeta `NN-nombre/` contiene el script de referencia de esa mecánica, con namespace `Frogames.Mechanics`. Ver la lista completa en `Assets/_ProjectStructure.md`.

## Aviso de propiedad intelectual (IP)

Este proyecto **analiza mecánicas y las reconstruye desde cero**; **no clona ningún juego**. Reglas obligatorias:

- **Nunca** redistribuyas ni incluyas arte, audio, código o assets de los juegos referidos (*Celeste*, *Sekiro*, *Portal*, *Diablo*, *Dead Cells*, etc.). Se citan solo como **referencia de análisis**.
- Usa exclusivamente **placeholders CC0**: primitivas de Unity (cápsulas, cubos, esferas), sprites/paquetes de **Kenney** u otros assets de dominio público / CC0.
- Los nombres de juegos aparecen únicamente en comentarios y documentación con fin educativo; la mecánica se implementa a partir de su **comportamiento observable**, no de sus fuentes.

## Qué NO está versionado

- Binarios ni carpetas generadas (`Library/`, `Temp/`, `Obj/`, `Build/`, `Logs/`, `UserSettings/`): los crea Unity al abrir el proyecto.

Las escenas de cada mecánica las montas tú siguiendo `Assets/_ProjectStructure.md`.
