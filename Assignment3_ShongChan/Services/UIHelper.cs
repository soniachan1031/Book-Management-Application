using Microsoft.AspNetCore.Mvc;

namespace Assignment3_ShongChan.Services
{
    public static class UIHelper
    {
        // method to create a card
        public static string Card(IUrlHelper urlHelper, string title, string description, string linkText, string linkClass, string action, string controller)
        {
            var url = urlHelper.Action(action, controller);
            return $@"
                <div class='col-md-3'>
                    <div class='card h-100 text-center'>
                        <div class='card-body'>
                            <h5 class='card-title'>{title}</h5>
                            <p class='card-text'>{description}</p>
                            <a href='{url}' class='btn {linkClass}'>{linkText}</a>
                        </div>
                    </div>
                </div>";
        }
    }
}
