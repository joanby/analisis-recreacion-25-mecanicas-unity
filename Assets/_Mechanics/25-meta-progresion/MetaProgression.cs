using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Frogames.Mechanics
{
    /// <summary>
    /// Meta-progresión persistente entre runs al estilo Dead Cells: al morir, la
    /// moneda de la run se suma al total permanente; la tienda gasta ese total en
    /// desbloqueos que se aplican al iniciar cada nueva run.
    ///
    /// Referencia de la lección 08-impl-meta-progresion. La meta se serializa a JSON
    /// en Application.persistentDataPath y sobrevive a cerrar y reabrir el editor.
    /// </summary>
    public class MetaProgression : MonoBehaviour
    {
        private MetaData _meta;
        private int _monedaDeLaRun;

        private void Awake() => _meta = MetaSaveSystem.Cargar();

        // ---- Bucle de run ----
        public void RecogerMoneda(int cantidad) => _monedaDeLaRun += cantidad;

        /// <summary>Al morir: consolidar la moneda de la run en la meta y guardar.</summary>
        public void AlMorir()
        {
            _meta.monedaTotal += _monedaDeLaRun;
            _monedaDeLaRun = 0;
            MetaSaveSystem.Guardar(_meta);
        }

        /// <summary>Tienda: comprar un desbloqueo si hay moneda suficiente.</summary>
        public bool Comprar(string idDesbloqueo, int coste)
        {
            if (_meta.monedaTotal < coste || _meta.desbloqueos.Contains(idDesbloqueo))
                return false;
            _meta.monedaTotal -= coste;
            _meta.desbloqueos.Add(idDesbloqueo);
            MetaSaveSystem.Guardar(_meta);
            return true;
        }

        /// <summary>Al iniciar una run: crear estado nuevo y aplicar los desbloqueos.</summary>
        public void IniciarRun()
        {
            _monedaDeLaRun = 0;
            foreach (string id in _meta.desbloqueos)
                AplicarDesbloqueo(id); // p. ej. arma extra, +vida
        }

        private void AplicarDesbloqueo(string id) =>
            Debug.Log($"Desbloqueo activo en esta run: {id}");

        public int MonedaTotal => _meta.monedaTotal;
    }

    /// <summary>Estado permanente serializable.</summary>
    [Serializable]
    public class MetaData
    {
        public int monedaTotal;
        public List<string> desbloqueos = new();
    }

    /// <summary>Guardado/carga con JsonUtility en persistentDataPath.</summary>
    public static class MetaSaveSystem
    {
        private static string Ruta => Path.Combine(Application.persistentDataPath, "meta.json");

        public static void Guardar(MetaData meta) =>
            File.WriteAllText(Ruta, JsonUtility.ToJson(meta, true));

        public static MetaData Cargar() =>
            File.Exists(Ruta)
                ? JsonUtility.FromJson<MetaData>(File.ReadAllText(Ruta))
                : new MetaData();
    }
}
