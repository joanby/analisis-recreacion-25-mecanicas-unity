# 22 — Cámara en tercera persona con colisión (estilo God of War)

Un `SphereCast` desde el objetivo hacia la cámara detecta geometría entre medio y acerca la vista hasta el punto de impacto, con la distancia interpolada (`Lerp`) para que el acercamiento y la vuelta sean suaves — la alternativa "de entendimiento" al Cinemachine Deoccluder.

## Escena mínima

- 3D. Personaje-objetivo (cápsula) sin requisitos especiales de física para este script.
- Cámara con el componente `CameraCollision`, colocada manualmente a la distancia que vayas a usar como `distanciaDeseada` (el script no la posiciona al arrancar; solo fija `_distanciaActual = distanciaDeseada` en `Start`).
- Paredes/columnas placeholder (cubos CC0) en una capa dedicada, p. ej. `Obstaculos`.
- No hace falta `Rigidbody` ni `Collider` en cámara ni objetivo: el `SphereCast` se lanza desde `objetivo.position`.

## Cableado en el Inspector

- `objetivo` (Transform): arrastra el personaje.
- `distanciaDeseada` (float, por defecto `5`): distancia cámara-objetivo sin obstáculos.
- `radioSphereCast` (float, por defecto `0.3`): grosor de la esfera lanzada hacia la cámara.
- `capaObstaculos` (LayerMask): la capa `Obstaculos` de las paredes.
- `velocidadDamping` (float, por defecto `10`): velocidad de interpolación al acercar/alejar.

## Cómo probarlo

1. Coloca la cámara manualmente detrás del personaje, a ~5 unidades (o el valor que pongas en `distanciaDeseada`).
2. Entra en Play mode.
3. El script no lee ningún input: mueve tú el objetivo (arrastrando su Transform en la vista Scene durante Play, o con un controlador WASD simple) hacia una pared de la capa `Obstaculos`.
4. Observa que, en cuanto hay geometría entre el objetivo y la posición actual de la cámara, esta se acerca (`SphereCast` detecta el impacto y usa `hit.distance`) con interpolación suave, sin saltos.
5. Aleja el personaje de la pared y comprueba que la cámara vuelve a `distanciaDeseada`, también con damping.

## Qué grabar

Clip de 5-8 s: el personaje caminando hacia una columna o pared, la cámara acercándose progresivamente sin llegar a atravesar la geometría, y al alejarse el personaje la cámara retrocediendo suave hasta la distancia original. Resultado esperado: nunca se ve el interior de la pared ni un salto brusco en la posición de la cámara; el movimiento de acercamiento/alejamiento es siempre amortiguado.
