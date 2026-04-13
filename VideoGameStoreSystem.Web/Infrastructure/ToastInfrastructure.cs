using Microsoft.AspNetCore.Mvc;

namespace VideoGameStoreSystem.Web.Infrastructure;

public static class ToastInfrastructure
{
    public static void SetToastSuccess(this Controller controller, string message)
    {
        controller.TempData["ToastType"] = "success";
        controller.TempData["ToastTitle"] = "Успешно";
        controller.TempData["ToastMessage"] = message;
    }

    public static void SetToastError(this Controller controller, string message)
    {
        controller.TempData["ToastType"] = "danger";
        controller.TempData["ToastTitle"] = "Ошибка";
        controller.TempData["ToastMessage"] = message;
    }

    public static void SetToastInfo(this Controller controller, string message)
    {
        controller.TempData["ToastType"] = "info";
        controller.TempData["ToastTitle"] = "Информация";
        controller.TempData["ToastMessage"] = message;
    }
}
