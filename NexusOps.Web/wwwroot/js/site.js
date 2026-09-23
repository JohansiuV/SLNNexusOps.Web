// NEXUS OPS - comportamiento común de la interfaz
document.addEventListener("DOMContentLoaded", function () {

    // ---------- Menú lateral en pantallas pequeñas ----------
    var sidebar = document.getElementById("nxSidebar");
    var overlay = document.getElementById("nxOverlay");
    var btnMenu = document.getElementById("btnMenu");

    function alternarMenu(abrir) {
        if (!sidebar) return;
        sidebar.classList.toggle("open", abrir);
        overlay.classList.toggle("show", abrir);
    }
    if (btnMenu) btnMenu.addEventListener("click", function () { alternarMenu(!sidebar.classList.contains("open")); });
    if (overlay) overlay.addEventListener("click", function () { alternarMenu(false); });

    // ---------- Mensajes de éxito que se cierran solos ----------
    document.querySelectorAll(".alert[data-autodismiss]").forEach(function (alerta) {
        var ms = parseInt(alerta.getAttribute("data-autodismiss"), 10) || 5000;
        setTimeout(function () {
            if (window.bootstrap && document.body.contains(alerta)) {
                bootstrap.Alert.getOrCreateInstance(alerta).close();
            }
        }, ms);
    });

    // ---------- Confirmación de operaciones importantes ----------
    // Uso: <button type="submit" data-confirm="¿Desea registrar esta incidencia?" data-confirm-ok="Registrar">
    var modalEl = document.getElementById("modalConfirmar");
    if (modalEl && window.bootstrap) {
        var modal = new bootstrap.Modal(modalEl);
        var pendiente = null;

        document.addEventListener("click", function (e) {
            var boton = e.target.closest("[data-confirm]");
            if (!boton) return;

            if (boton.dataset.confirmado === "1") {
                delete boton.dataset.confirmado;
                return; // segunda pasada: deja continuar el envío
            }

            var form = boton.form || boton.closest("form");

            // Si el formulario tiene errores de validación, se muestran primero.
            if (form && window.jQuery && jQuery.fn.valid && !boton.hasAttribute("formnovalidate") && !jQuery(form).valid()) {
                return;
            }

            e.preventDefault();
            pendiente = boton;

            document.getElementById("modalConfirmarTexto").textContent = boton.dataset.confirm;
            document.getElementById("modalConfirmarTitulo").textContent = boton.dataset.confirmTitulo || "Confirmar operación";
            var ok = document.getElementById("modalConfirmarOk");
            ok.textContent = boton.dataset.confirmOk || "Aceptar";
            ok.className = "btn " + (boton.dataset.confirmClase || "btn-primary");
            modal.show();
        });

        document.getElementById("modalConfirmarOk").addEventListener("click", function () {
            if (!pendiente) return;
            var boton = pendiente;
            pendiente = null;
            modal.hide();
            boton.dataset.confirmado = "1";
            var form = boton.form || boton.closest("form");
            if (form && form.requestSubmit) form.requestSubmit(boton.type === "submit" ? boton : undefined);
            else boton.click();
        });
    }

    // ---------- Modal de cambio de estado (incidencias, tareas, mantenimientos, activos) ----------
    var modalEstado = document.getElementById("modalEstado");
    if (modalEstado) {
        modalEstado.addEventListener("show.bs.modal", function (e) {
            var b = e.relatedTarget;
            if (!b) return;

            document.getElementById("modalEstadoId").value = b.dataset.id || "";
            document.getElementById("modalEstadoValor").value = b.dataset.estado || "";
            document.getElementById("modalEstadoTitulo").textContent = b.dataset.titulo || "Cambiar estado";
            document.getElementById("modalEstadoTexto").textContent = b.dataset.texto || "";

            var comentario = document.getElementById("modalEstadoComentario");
            comentario.value = "";
            comentario.required = b.dataset.requiereComentario === "1";
            document.getElementById("modalEstadoEtiqueta").textContent =
                comentario.required ? "Motivo (obligatorio)" : "Comentario (opcional)";

            var asignar = document.getElementById("modalEstadoAsignar");
            if (asignar) {
                var pedir = b.dataset.asignar === "1";
                asignar.classList.toggle("d-none", !pedir);
                var sel = document.getElementById("modalEstadoUsuario");
                sel.required = pedir && b.dataset.asignarObligatorio === "1";
                sel.value = b.dataset.usuario || "";
            }

            var ok = document.getElementById("modalEstadoOk");
            ok.textContent = b.dataset.boton || "Confirmar";
            ok.className = "btn " + (b.dataset.clase || "btn-primary");
        });
    }

    // ---------- Repuestos dinámicos del formulario de mantenimiento ----------
    var tabla = document.getElementById("tablaRepuestos");
    if (tabla) {
        var cuerpo = tabla.querySelector("tbody");
        var plantilla = document.getElementById("plantillaRepuesto");

        function reindexar() {
            cuerpo.querySelectorAll("tr.linea").forEach(function (fila, i) {
                fila.querySelectorAll("[data-campo]").forEach(function (campo) {
                    campo.name = "Input.Detalles[" + i + "]." + campo.dataset.campo;
                });
            });
            var vacio = document.getElementById("sinRepuestos");
            if (vacio) vacio.classList.toggle("d-none", cuerpo.querySelectorAll("tr.linea").length > 0);
        }

        document.getElementById("btnAgregarRepuesto").addEventListener("click", function () {
            cuerpo.appendChild(plantilla.content.cloneNode(true));
            reindexar();
        });

        cuerpo.addEventListener("click", function (e) {
            var quitar = e.target.closest(".btn-quitar");
            if (quitar) {
                quitar.closest("tr").remove();
                reindexar();
            }
        });

        reindexar();
    }
});
