using Microsoft.AspNetCore.Authorization;

namespace FitAI.Web.Pages;
[Authorize]  // ← BU SATIRI EKLE

public class IndexModel : FitAIPageModel
{
    public void OnGet()
    {

    }
}
