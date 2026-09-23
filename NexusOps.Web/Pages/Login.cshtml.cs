using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _auth;

        public LoginModel(AuthService auth)
        {
            _auth = auth;
        }

        [BindProperty]
        public LoginRequestDto Login { get; set; } = new();

        public string? MensajeError { get; set; }

        public bool Expirada { get; set; }

        public IActionResult OnGet(bool expirada = false)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToPage("/Index");

            Expirada = expirada;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid) return Page();

            var resultado = await _auth.LoginAsync(Login);

            if (!resultado.Exito || resultado.Datos == null)
            {
                MensajeError = resultado.Error ?? "Usuario o contraseña incorrectos.";
                Login.Password = string.Empty;
                return Page();
            }

            await _auth.IniciarSesionAsync(HttpContext, resultado.Datos);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                return LocalRedirect(returnUrl);

            return RedirectToPage("/Index");
        }
    }
}
