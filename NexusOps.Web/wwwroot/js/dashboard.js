// Dashboard: dibuja los gráficos con los datos reales que entrega la API.
(function () {
    var origen = document.getElementById("dashData");
    if (!origen || typeof Chart === "undefined") return;

    var datos = JSON.parse(origen.textContent);

    Chart.defaults.font.family = "'Segoe UI', system-ui, sans-serif";
    Chart.defaults.color = "#475569";

    var colores = {
        prioridad: { "Baja": "#94a3b8", "Media": "#38bdf8", "Alta": "#f59e0b", "Critica": "#ef4444" },
        activo: { "Operativo": "#22c55e", "En mantenimiento": "#f59e0b", "Dañado": "#ef4444", "Inactivo": "#94a3b8", "Dado de baja": "#334155" },
        tarea: { "Pendiente": "#94a3b8", "En proceso": "#3b82f6", "Completada": "#22c55e", "Cancelada": "#cbd5e1", "Vencida": "#ef4444" }
    };
    var paleta = ["#2563eb", "#06b6d4", "#8b5cf6", "#f59e0b", "#ec4899", "#14b8a6", "#f97316", "#64748b"];

    function etiquetas(lista) { return lista.map(function (x) { return x.etiqueta === "Critica" ? "Crítica" : x.etiqueta; }); }
    function valores(lista) { return lista.map(function (x) { return x.valor; }); }
    function porMapa(lista, mapa) { return lista.map(function (x, i) { return mapa[x.etiqueta] || paleta[i % paleta.length]; }); }
    function suma(lista) { return lista.reduce(function (a, x) { return a + x.valor; }, 0); }

    function lienzo(id, lista) {
        var el = document.getElementById(id);
        if (!el) return null;
        if (!lista || suma(lista) === 0) {
            el.parentElement.innerHTML = '<div class="dash-vacio"><i class="bi bi-inbox fs-3"></i><br>Sin datos para mostrar</div>';
            return null;
        }
        return el.getContext("2d");
    }

    function dona(id, lista, mapa) {
        var ctx = lienzo(id, lista);
        if (!ctx) return;
        new Chart(ctx, {
            type: "doughnut",
            data: { labels: etiquetas(lista), datasets: [{ data: valores(lista), backgroundColor: porMapa(lista, mapa), borderWidth: 2, borderColor: "#fff" }] },
            options: { maintainAspectRatio: false, cutout: "62%", plugins: { legend: { position: "bottom" } } }
        });
    }

    function barras(id, lista, color, horizontal, prefijo) {
        var ctx = lienzo(id, lista);
        if (!ctx) return;
        new Chart(ctx, {
            type: "bar",
            data: { labels: etiquetas(lista), datasets: [{ data: valores(lista), backgroundColor: color, borderRadius: 6, maxBarThickness: 38 }] },
            options: {
                maintainAspectRatio: false,
                indexAxis: horizontal ? "y" : "x",
                plugins: { legend: { display: false }, tooltip: { callbacks: { label: function (c) { return (prefijo || "") + c.parsed[horizontal ? "x" : "y"].toLocaleString("en-US", prefijo ? { minimumFractionDigits: 2 } : {}); } } } },
                scales: { y: { beginAtZero: true, ticks: { precision: 0 } }, x: { beginAtZero: true, ticks: { precision: 0 } } }
            }
        });
    }

    dona("chartPrioridad", datos.incidenciasPorPrioridad, colores.prioridad);
    dona("chartActivos", datos.activosPorEstado, colores.activo);
    dona("chartTareas", datos.tareasPorEstado, colores.tarea);
    barras("chartAreas", datos.incidenciasPorArea, "#2563eb", true);
    barras("chartMantenimientos", datos.mantenimientosPorMes, "#06b6d4", false);

    // Costos de mantenimiento por mes + acumulado
    var ctxCostos = lienzo("chartCostos", datos.costosPorMes);
    if (ctxCostos) {
        var acumulado = 0;
        var acum = datos.costosPorMes.map(function (x) { acumulado += x.valor; return acumulado; });
        new Chart(ctxCostos, {
            data: {
                labels: etiquetas(datos.costosPorMes),
                datasets: [
                    { type: "bar", label: "Costo mensual (S/)", data: valores(datos.costosPorMes), backgroundColor: "#8b5cf6", borderRadius: 6, maxBarThickness: 38 },
                    { type: "line", label: "Acumulado (S/)", data: acum, borderColor: "#ef4444", backgroundColor: "#ef4444", tension: .3, pointRadius: 3 }
                ]
            },
            options: {
                maintainAspectRatio: false,
                plugins: { legend: { position: "bottom" } },
                scales: { y: { beginAtZero: true, ticks: { callback: function (v) { return "S/ " + v.toLocaleString("en-US"); } } } }
            }
        });
    }
})();
