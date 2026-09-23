// Campana de alertas: consulta periódicamente el contador de alertas no leídas.
(function () {
    var badge = document.getElementById("alertBadge");
    if (!badge) return;

    function actualizar() {
        fetch("/Alertas?handler=Contador", { credentials: "same-origin", headers: { "Accept": "application/json" } })
            .then(function (r) { return r.ok ? r.json() : null; })
            .then(function (d) {
                if (!d) return;
                var total = d.noLeidas || 0;
                badge.textContent = total > 99 ? "99+" : total;
                badge.classList.toggle("d-none", total === 0);
                badge.classList.toggle("advertencia", (d.criticas || 0) === 0);
            })
            .catch(function () { /* sin conexión: se conserva el valor anterior */ });
    }

    actualizar();
    setInterval(actualizar, 60000);
})();
