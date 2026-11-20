using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Recolección / farming al estilo Stardew Valley: datos como ScriptableObject,
    /// tiles interactuables con estados, ciclo de día que hace madurar los cultivos
    /// e inventario persistido a JSON.
    ///
    /// Referencia de la lección 02-impl-recoleccion. Este fichero agrupa los datos
    /// (CropData/ItemData), el tile (FarmTile) y el manager para tenerlo todo a la
    /// vista; en producción cada clase iría en su propio archivo.
    /// </summary>
    public class FarmingSystem : MonoBehaviour
    {
        [SerializeField] private FarmTile[] tiles;
        [SerializeField] private string ficheroGuardado = "farm_save.json";

        private readonly List<InventarioEntrada> _inventario = new();
        public int Dia { get; private set; }

        private void Awake() => Cargar();

        /// <summary>El botón "Dormir" avanza el día y madura los cultivos.</summary>
        public void Dormir()
        {
            Dia++;
            foreach (FarmTile tile in tiles)
                tile.Avanzar();
            Guardar();
        }

        public void Recolectar(ItemData item)
        {
            InventarioEntrada e = _inventario.Find(x => x.itemId == item.id);
            if (e == null) _inventario.Add(new InventarioEntrada { itemId = item.id, cantidad = 1 });
            else e.cantidad++;
        }

        // ---- Persistencia con JsonUtility ----
        private string Ruta => Path.Combine(Application.persistentDataPath, ficheroGuardado);

        private void Guardar()
        {
            var data = new SaveData { dia = Dia, inventario = _inventario };
            data.tiles = new List<TileState>();
            foreach (FarmTile t in tiles) data.tiles.Add(t.Capturar());
            File.WriteAllText(Ruta, JsonUtility.ToJson(data, true));
        }

        private void Cargar()
        {
            if (!File.Exists(Ruta)) return;
            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(Ruta));
            Dia = data.dia;
            _inventario.Clear();
            _inventario.AddRange(data.inventario);
            for (int i = 0; i < tiles.Length && i < data.tiles.Count; i++)
                tiles[i].Restaurar(data.tiles[i]);
        }

        [Serializable]
        private class SaveData
        {
            public int dia;
            public List<TileState> tiles;
            public List<InventarioEntrada> inventario;
        }

        [Serializable] public class InventarioEntrada { public string itemId; public int cantidad; }
        [Serializable] public struct TileState { public int estado; public int edad; }
    }

    /// <summary>Cultivo definido como asset reutilizable.</summary>
    [CreateAssetMenu(menuName = "Frogames/Farming/CropData")]
    public class CropData : ScriptableObject
    {
        public string nombre;
        public Sprite spriteSemilla;
        public Sprite spriteMaduro;
        public int diasParaMadurar = 3;
        public ItemData cosecha;
    }

    /// <summary>Ítem genérico (cosecha, semilla, etc.).</summary>
    [CreateAssetMenu(menuName = "Frogames/Farming/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public string nombre;
        public Sprite sprite;
        public int valorVenta = 10;
    }

    /// <summary>Casilla de cultivo: guarda su estado y su edad en días.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class FarmTile : MonoBehaviour
    {
        public enum Estado { Vacio, Plantado, Maduro }

        [SerializeField] private CropData cultivo;
        [SerializeField] private FarmingSystem sistema;

        private Estado _estado = Estado.Vacio;
        private int _edad;
        private SpriteRenderer _sr;

        private void Awake() => _sr = GetComponent<SpriteRenderer>();

        private void OnMouseDown()
        {
            if (_estado == Estado.Vacio) Plantar();
            else if (_estado == Estado.Maduro) Cosechar();
        }

        private void Plantar()
        {
            _estado = Estado.Plantado;
            _edad = 0;
            _sr.sprite = cultivo.spriteSemilla;
        }

        private void Cosechar()
        {
            sistema.Recolectar(cultivo.cosecha);
            _estado = Estado.Vacio;
            _sr.sprite = null;
        }

        /// <summary>Un día más: madura si toca.</summary>
        public void Avanzar()
        {
            if (_estado != Estado.Plantado) return;
            _edad++;
            if (_edad >= cultivo.diasParaMadurar)
            {
                _estado = Estado.Maduro;
                _sr.sprite = cultivo.spriteMaduro;
            }
        }

        public FarmingSystem.TileState Capturar() =>
            new() { estado = (int)_estado, edad = _edad };

        public void Restaurar(FarmingSystem.TileState s)
        {
            _estado = (Estado)s.estado;
            _edad = s.edad;
            _sr.sprite = _estado == Estado.Maduro ? cultivo.spriteMaduro
                        : _estado == Estado.Plantado ? cultivo.spriteSemilla : null;
        }
    }
}
