# Estructura del proyecto — escenas y scripts

Cada mecánica es un **micro-proyecto autocontenido**: **una escena + su script de referencia**, "jugable en 60 segundos". No hay un juego completo hasta el capstone. Todo el arte son **placeholders CC0** (primitivas de Unity, sprites Kenney).

Las escenas (`.scene`) **no** están versionadas: móntalas tú siguiendo la columna "Escena mínima". El script de cada mecánica ya está en `Assets/_Mechanics/NN-nombre/`.

## Convención

- **Una escena por mecánica**, nombrada igual que su carpeta: `01-doble-salto.unity`, `02-wall-run.unity`, …
- Guárdalas en `Assets/_Scenes/` (créala al montar la primera).
- Namespace común de todos los scripts: `Frogames.Mechanics`.
- Cablea siempre los campos serializados (LayerMask, Transforms, prefabs) en el Inspector.

## Módulo 2 — Movimiento y plataformas

| # | Escena | Script | Escena mínima (placeholders CC0) |
| --- | --- | --- | --- |
| 01 | doble-salto | `PlayerJump.cs` | 2D: cápsula con `Rigidbody2D`, hijo `groundCheck`, dos plataformas, capa Suelo. |
| 02 | wall-run | `WallRun.cs` | 3D: cápsula con `CharacterController` entre dos paredes en la capa Pared. |
| 03 | air-dash | `AirDash.cs` | 2D: cápsula con `Rigidbody2D`; capa i-frames sin colisión con Peligros. |
| 04 | gancho | `GrapplingHook.cs` | 3D: cuerpo con `Rigidbody`, anclajes en la capa Anclable, `LineRenderer`. |

## Módulo 3 — Combate cuerpo a cuerpo

| # | Escena | Script | Escena mínima |
| --- | --- | --- | --- |
| 05 | parry | `ParrySystem.cs` | Defensor (cápsula) + atacante (cubo con arma-trigger, tag `EnemyWeapon`). |
| 06 | combos | `ComboSystem.cs` | Cápsula con `Animator` (estados Attack1/2/3 + animation events) y enemigo estático. |
| 07 | lock-on | `LockOnTargeting.cs` | Jugador + 3-4 cubos (capa Enemy), `CinemachineTargetGroup` + cámara de combate. |
| 08 | hitstop | `HitFeedback.cs` | Reutiliza un golpe del módulo; `CinemachineImpulseSource`, `Renderer`, `ParticleSystem`. |

## Módulo 4 — Físicas y proyectiles

| # | Escena | Script | Escena mínima |
| --- | --- | --- | --- |
| 09 | bullet-time | `BulletTimeController.cs` | Jugador + proyectil `Rigidbody`, dianas CC0, barra de UI del recurso. |
| 10 | lanzamiento-parabolico | `ProjectileLauncher.cs` | 2D: punto de lanzamiento, prefab `Rigidbody2D`, `LineRenderer`, cajas objetivo. |
| 11 | portales | `Portal.cs` | 3D: jugador con `Rigidbody`, dos portales (trigger) emparejados por Inspector. |
| 12 | ragdoll | `RagdollController.cs` | Humanoide CC0 con `Rigidbody`+`ConfigurableJoint` por hueso y `Animator`. |

## Módulo 5 — Progresión y economía

| # | Escena | Script | Escena mínima |
| --- | --- | --- | --- |
| 13 | recoleccion | `FarmingSystem.cs` | Grid de `FarmTile` (SpriteRenderer+Collider2D), assets `CropData`/`ItemData`, botón Dormir. |
| 14 | skill-tree | `SkillTreeManager.cs` | 5-6 assets `SkillNode` (uno raíz), UI de nodos, botón Subir de nivel. |
| 15 | crafting | `CraftingSystem.cs` | Assets `CraftItem` y `Recipe`, grid de ranuras, botón Craftear. |
| 16 | loot | `LootGenerator.cs` | Assets `Rarity` (común→legendaria) y `Affix`, tarjeta UI, botón Derrotar enemigo. |

## Módulo 6 — Puzles y manipulación del mundo

| # | Escena | Script | Escena mínima |
| --- | --- | --- | --- |
| 17 | sokoban | `GridController.cs` | Rejilla con bloques y muros alineados a enteros, al menos un objetivo. |
| 18 | rewind | `Rewind.cs` | 2D: objeto con `Rigidbody2D` que cae; tecla Q para rebobinar. |
| 19 | cambio-gravedad | `GravityFlip.cs` | 2D: sala con suelo y techo (capa Suelo/Techo), cápsula con `Rigidbody2D`. |

## Módulo 7 — Sigilo, percepción e IA

| # | Escena | Script | Escena mínima |
| --- | --- | --- | --- |
| 20 | cono-vision | `FieldOfView.cs` | Guardia + jugador (capa Target), muro de prueba (capa Obstacle). |
| 21 | fsm-alerta | `GuardAI.cs` | NavMesh horneado, guardia con `NavMeshAgent`+`FieldOfView`, waypoints, jugador. |

## Módulo 8 — Cámara, control y roguelike

| # | Escena | Script | Escena mínima |
| --- | --- | --- | --- |
| 22 | camara-3p | `CameraCollision.cs` | 3D: personaje-objetivo, paredes/columnas (capa Obstáculos), cámara con el rig. |
| 23 | twin-stick | `TwinStickController.cs` (+ `BulletPool`, `Bullet`) | 2D: jugador con `Rigidbody2D`+arma, `BulletPool`, InputActions Move/Aim/Fire. |
| 24 | dungeon-procedural | `DungeonGenerator.cs` | Prefab de sala CC0; Espacio regenera con nueva semilla. |
| 25 | meta-progresion | `MetaProgression.cs` (+ `MetaData`, `MetaSaveSystem`) | UI del bucle morir→desbloquear→nueva run; guardado JSON en persistentDataPath. |

## Escena capstone

El **capstone** (módulo 11 del curso) no es una de las 25 mecánicas: el estudiante **elige una mecánica**, la analiza con MDA y la reconstruye en su propia escena, reutilizando estos scripts como referencia y andamiaje.

- Escena: `Assets/_Scenes/capstone-<mecanica-elegida>.unity`.
- Puede combinar varios scripts de `_Mechanics/` (p. ej. twin-stick + loot + meta-progresión para un mini-roguelite).
- Mismo estándar de placeholders CC0 y mismo aviso de IP que el resto del proyecto.
