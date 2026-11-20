# 12 — Ragdoll conmutable (estilo Human: Fall Flat)

Un personaje que alterna entre estado animado (rígido, controlado por `Animator`) y estado físico (ragdoll suelto, controlado por la cadena de `Rigidbody`+`ConfigurableJoint`). La torpeza característica no se programa: emerge de los joints simulándose.

## Escena mínima

3D. Humanoide CC0 (o cápsulas encadenadas simulando pelvis/torso/cabeza/brazos/piernas) con un `Rigidbody` + `Collider` en cada hueso principal, unidos a su padre por un `ConfigurableJoint` con límites angulares razonables (puedes partir del asistente Ragdoll de Unity). Reparte masas con sentido: torso y pelvis pesados, extremidades ligeras (un mal reparto es la causa nº1 de temblores). Añade un `Animator` con una animación de "de pie". El componente `RagdollController` va en la raíz del personaje.

## Cableado en el Inspector

- `Animator` (privado serializado): el `Animator` del personaje.
- `Torso` (`Rigidbody`, privado serializado): el `Rigidbody` del hueso de torso; es el que recibe el empujón de `AddForce`.
- `Fuerza Empujon` (float, por defecto `8f`): magnitud del impulso aplicado al torso.

El script recopila automáticamente en `Awake` (`GetComponentsInChildren<Rigidbody>()`) todos los `Rigidbody` de los huesos bajo el GameObject — no hace falta asignarlos a mano, solo asegúrate de que cuelguen todos de esta jerarquía.

## Cómo probarlo

1. Entra en Play; el personaje arranca "de pie" (huesos `isKinematic = true`, `Animator` activo).
2. Pulsa **R** (`Keyboard.current.rKey`) para alternar a ragdoll: el `Animator` se desactiva, los huesos pasan a `isKinematic = false` y el cuerpo se desploma por gravedad.
3. Con el personaje en ragdoll, pulsa **F** (`Keyboard.current.fKey`) para empujar el torso con `AddForce(transform.forward * fuerzaEmpujon, ForceMode.Impulse)` y verlo tambalearse o rodar.
4. Pulsa **R** de nuevo para volver al estado animado (si `torso` sigue activo, el personaje recupera el control).

## Qué grabar

Clip de 3-8 s: personaje de pie → pulsa R → colapsa como ragdoll físico → pulsa F para empujar el torso y verlo tambalearse/rodar de forma creíble → pulsa R otra vez para volver al estado animado. Resultado esperado: transición limpia sin vibración ni explosión de física, y el empujón produce un movimiento reactivo visible en el torso y las extremidades encadenadas.
